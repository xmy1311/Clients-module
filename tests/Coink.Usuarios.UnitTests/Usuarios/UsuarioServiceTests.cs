using Coink.Usuarios.Application.Comun;
using Coink.Usuarios.Application.Usuarios.Dtos;
using Coink.Usuarios.Application.Usuarios.Modelos;
using Coink.Usuarios.Application.Usuarios;
using Coink.Usuarios.UnitTests.Dobles;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Coink.Usuarios.UnitTests.Usuarios;

public sealed class UsuarioServiceTests
{
    private static RegistrarUsuarioRequest Peticion(string telefono = "+573001234567") =>
        new("Xiomara Zapata", telefono, 1, 1, 1, "Calle 10 # 43-12 Apto 301");

    private static UsuarioService Servicio(RepositorioUsuarioFalso repositorio) =>
        new(repositorio, NullLogger<UsuarioService>.Instance);

    [Fact]
    public async Task Registro_exitoso_devuelve_el_identificador_asignado()
    {
        var fecha = DateTimeOffset.UtcNow;
        var repositorio = new RepositorioUsuarioFalso(new RespuestaRegistro(0, 42, fecha));

        var resultado = await Servicio(repositorio).RegistrarAsync(Peticion(), CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(42, resultado.Valor!.UsuarioId);
        Assert.Equal(fecha, resultado.Valor.FechaRegistro);
        Assert.Equal(CodigoError.Ninguno, resultado.Error);
    }

    [Theory]
    [InlineData(1, CodigoError.PaisNoExiste)]
    [InlineData(2, CodigoError.DepartamentoNoExiste)]
    [InlineData(3, CodigoError.MunicipioNoExiste)]
    [InlineData(4, CodigoError.DepartamentoNoPerteneceAlPais)]
    [InlineData(5, CodigoError.MunicipioNoPerteneceAlDepartamento)]
    [InlineData(6, CodigoError.TelefonoDuplicado)]
    public async Task Traduce_cada_codigo_del_procedimiento_a_su_error_de_negocio(short codigo, CodigoError esperado)
    {
        var repositorio = new RepositorioUsuarioFalso(new RespuestaRegistro(codigo, null, null));

        var resultado = await Servicio(repositorio).RegistrarAsync(Peticion(), CancellationToken.None);

        Assert.False(resultado.EsExitoso);
        Assert.Equal(esperado, resultado.Error);
        Assert.False(string.IsNullOrWhiteSpace(resultado.Mensaje));
    }

    [Fact]
    public async Task Normaliza_los_datos_antes_de_enviarlos_al_repositorio()
    {
        var repositorio = new RepositorioUsuarioFalso();
        var peticion = new RegistrarUsuarioRequest(
            "  Xiomara   Zapata ", "+57 (300) 123-4567", 1, 1, 1, "  Calle 10  #  43-12 ");

        await Servicio(repositorio).RegistrarAsync(peticion, CancellationToken.None);

        Assert.Equal("Xiomara Zapata", repositorio.UltimoUsuarioRecibido!.Nombre);
        Assert.Equal("+573001234567", repositorio.UltimoUsuarioRecibido.Telefono);
        Assert.Equal("Calle 10 # 43-12", repositorio.UltimoUsuarioRecibido.Direccion);
    }

    [Fact]
    public async Task Un_codigo_no_contemplado_falla_de_forma_visible()
    {
        var repositorio = new RepositorioUsuarioFalso(new RespuestaRegistro(99, null, null));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => Servicio(repositorio).RegistrarAsync(Peticion(), CancellationToken.None));
    }

    [Fact]
    public async Task Exito_sin_identificador_es_una_ruptura_de_contrato_y_falla()
    {
        var repositorio = new RepositorioUsuarioFalso(new RespuestaRegistro(0, null, null));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => Servicio(repositorio).RegistrarAsync(Peticion(), CancellationToken.None));
    }

    [Fact]
    public async Task Un_fallo_de_infraestructura_no_se_traga()
    {
        var repositorio = new RepositorioUsuarioFalso(excepcion: new TimeoutException("La base no responde."));

        await Assert.ThrowsAsync<TimeoutException>(
            () => Servicio(repositorio).RegistrarAsync(Peticion(), CancellationToken.None));
    }

    [Fact]
    public async Task Consultar_un_usuario_inexistente_devuelve_no_encontrado()
    {
        var repositorio = new RepositorioUsuarioFalso(detalle: null);

        var resultado = await Servicio(repositorio).ObtenerAsync(999, CancellationToken.None);

        Assert.False(resultado.EsExitoso);
        Assert.Equal(CodigoError.UsuarioNoEncontrado, resultado.Error);
    }

    [Fact]
    public async Task Consultar_un_usuario_existente_devuelve_su_detalle()
    {
        var detalle = new UsuarioDetalleResponse(
            1, "Xiomara Zapata", "+573001234567", "Calle 10 # 43-12",
            "Colombia", "Antioquia", "Medellín", DateTimeOffset.UtcNow);

        var repositorio = new RepositorioUsuarioFalso(detalle: detalle);

        var resultado = await Servicio(repositorio).ObtenerAsync(1, CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        Assert.Equal("Medellín", resultado.Valor!.Municipio);
    }
}
