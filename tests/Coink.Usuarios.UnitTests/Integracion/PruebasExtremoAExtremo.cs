using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Coink.Usuarios.UnitTests.Integracion;

/// <summary>
/// Pruebas de extremo a extremo contra la solución realmente levantada:
/// API, PostgreSQL, procedimiento almacenado y manejo de errores.
/// </summary>
public sealed class PruebasExtremoAExtremo
{
    private static readonly string UrlBase =
        Environment.GetEnvironmentVariable("COINK_URL_API") ?? "http://localhost:8080";

    private static HttpClient Cliente() =>
        new() { BaseAddress = new Uri(UrlBase), Timeout = TimeSpan.FromSeconds(15) };

    private static string TelefonoNuevo() => $"+57300{Random.Shared.Next(1_000_000, 9_999_999)}";

    private static object Peticion(
        string telefono,
        int paisId = 1,
        int departamentoId = 1,
        int municipioId = 1,
        string nombre = "Prueba Integracion",
        string direccion = "Calle 10 # 43-12 Apto 301") =>
        new { nombre, telefono, paisId, departamentoId, municipioId, direccion };

    private static async Task<string?> CodigoDeError(HttpResponseMessage respuesta)
    {
        var cuerpo = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
        return cuerpo.TryGetProperty("codigo", out var codigo) ? codigo.GetString() : null;
    }

    [HechoDeIntegracion]
    public async Task Registro_valido_devuelve_201_y_el_recurso_queda_consultable()
    {
        using var cliente = Cliente();

        var respuesta = await cliente.PostAsJsonAsync("/api/usuarios", Peticion(TelefonoNuevo()));

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        Assert.NotNull(respuesta.Headers.Location);

        var detalle = await cliente.GetFromJsonAsync<JsonElement>(respuesta.Headers.Location!.ToString());

        Assert.Equal("Colombia", detalle.GetProperty("pais").GetString());
        Assert.Equal("Antioquia", detalle.GetProperty("departamento").GetString());
        Assert.Equal("Medellín", detalle.GetProperty("municipio").GetString());
    }

    [HechoDeIntegracion]
    public async Task Telefono_repetido_devuelve_409()
    {
        using var cliente = Cliente();
        var telefono = TelefonoNuevo();

        var primera = await cliente.PostAsJsonAsync("/api/usuarios", Peticion(telefono));
        var segunda = await cliente.PostAsJsonAsync("/api/usuarios", Peticion(telefono));

        Assert.Equal(HttpStatusCode.Created, primera.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, segunda.StatusCode);
        Assert.Equal("TELEFONO_DUPLICADO", await CodigoDeError(segunda));
    }

    [HechoDeIntegracion]
    public async Task Pais_inexistente_devuelve_422()
    {
        using var cliente = Cliente();

        var respuesta = await cliente.PostAsJsonAsync("/api/usuarios", Peticion(TelefonoNuevo(), paisId: 999));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, respuesta.StatusCode);
        Assert.Equal("PAIS_NO_EXISTE", await CodigoDeError(respuesta));
    }

    [HechoDeIntegracion]
    public async Task Departamento_de_otro_pais_devuelve_422()
    {
        using var cliente = Cliente();

        // Colombia (1) con Cusco (6), que pertenece a Perú.
        var respuesta = await cliente.PostAsJsonAsync(
            "/api/usuarios", Peticion(TelefonoNuevo(), paisId: 1, departamentoId: 6, municipioId: 13));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, respuesta.StatusCode);
        Assert.Equal("DEPARTAMENTO_NO_PERTENECE_AL_PAIS", await CodigoDeError(respuesta));
    }

    [HechoDeIntegracion]
    public async Task Municipio_de_otro_departamento_devuelve_422()
    {
        using var cliente = Cliente();

        // Antioquia (1) con Bogotá (7), que pertenece a Bogotá D.C.
        var respuesta = await cliente.PostAsJsonAsync(
            "/api/usuarios", Peticion(TelefonoNuevo(), paisId: 1, departamentoId: 1, municipioId: 7));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, respuesta.StatusCode);
        Assert.Equal("MUNICIPIO_NO_PERTENECE_AL_DEPARTAMENTO", await CodigoDeError(respuesta));
    }

    [HechoDeIntegracion]
    public async Task Telefono_con_formato_invalido_devuelve_400_con_detalle_por_campo()
    {
        using var cliente = Cliente();

        var respuesta = await cliente.PostAsJsonAsync("/api/usuarios", Peticion("3001234567"));

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);

        var cuerpo = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(cuerpo.GetProperty("errors").TryGetProperty("telefono", out _));
        Assert.True(cuerpo.TryGetProperty("traceId", out _));
    }

    [HechoDeIntegracion]
    public async Task Usuario_inexistente_devuelve_404()
    {
        using var cliente = Cliente();

        var respuesta = await cliente.GetAsync("/api/usuarios/999999");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
        Assert.Equal("USUARIO_NO_ENCONTRADO", await CodigoDeError(respuesta));
    }

    [HechoDeIntegracion]
    public async Task Los_catalogos_respetan_la_jerarquia()
    {
        using var cliente = Cliente();

        var paises = await cliente.GetFromJsonAsync<JsonElement>("/api/catalogos/paises");
        var departamentos = await cliente.GetFromJsonAsync<JsonElement>("/api/catalogos/paises/1/departamentos");
        var municipios = await cliente.GetFromJsonAsync<JsonElement>("/api/catalogos/departamentos/1/municipios");

        Assert.Equal(2, paises.GetArrayLength());
        Assert.Equal(5, departamentos.GetArrayLength());   // Colombia
        Assert.Equal(4, municipios.GetArrayLength());      // Antioquia

        // Todo lo que devuelve el catálogo pertenece al padre solicitado.
        foreach (var departamento in departamentos.EnumerateArray())
        {
            Assert.Equal(1, departamento.GetProperty("paisId").GetInt32());
        }
    }

    [HechoDeIntegracion]
    public async Task Un_padre_inexistente_devuelve_una_coleccion_vacia_no_un_404()
    {
        using var cliente = Cliente();

        var respuesta = await cliente.GetAsync("/api/catalogos/paises/999/departamentos");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var cuerpo = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(0, cuerpo.GetArrayLength());
    }
}
