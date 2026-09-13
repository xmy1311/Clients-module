using Coink.Usuarios.Application.Usuarios;
using Coink.Usuarios.Application.Usuarios.Dtos;
using Xunit;

namespace Coink.Usuarios.UnitTests.Usuarios;

public sealed class RegistrarUsuarioValidatorTests
{
    private readonly RegistrarUsuarioValidator _validador = new();

    private static RegistrarUsuarioRequest Valida(
        string nombre = "Xiomara Zapata",
        string telefono = "+573001234567",
        int paisId = 1,
        int departamentoId = 1,
        int municipioId = 1,
        string direccion = "Calle 10 # 43-12 Apto 301") =>
        new(nombre, telefono, paisId, departamentoId, municipioId, direccion);

    [Fact]
    public void Acepta_una_peticion_valida() =>
        Assert.True(_validador.Validate(Valida()).IsValid);

    [Theory]
    [InlineData("María José Ñungo")]
    [InlineData("O'Brien")]
    [InlineData("Jean-Pierre")]
    [InlineData("  Ana   María  ")]
    public void Acepta_nombres_con_tildes_apostrofos_guiones_y_espacios_sobrantes(string nombre) =>
        Assert.True(_validador.Validate(Valida(nombre: nombre)).IsValid);

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("A")]
    [InlineData("Ana3")]
    [InlineData("Ana@Maria")]
    public void Rechaza_nombres_invalidos(string nombre)
    {
        var resultado = _validador.Validate(Valida(nombre: nombre));

        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, error => error.PropertyName == "nombre");
    }

    [Fact]
    public void Rechaza_un_nombre_demasiado_largo() =>
        Assert.False(_validador.Validate(Valida(nombre: new string('a', 101))).IsValid);

    [Theory]
    [InlineData("+573001234567")]
    [InlineData("+5716012345")]
    [InlineData("+12125550123")]
    [InlineData("+57 (300) 123-4567")]
    public void Acepta_telefonos_en_formato_E164(string telefono) =>
        Assert.True(_validador.Validate(Valida(telefono: telefono)).IsValid);

    [Theory]
    [InlineData("3001234567")]
    [InlineData("+0300123456")]
    [InlineData("+57300")]
    [InlineData("+57-300-ABC")]
    [InlineData("")]
    public void Rechaza_telefonos_invalidos(string telefono)
    {
        var resultado = _validador.Validate(Valida(telefono: telefono));

        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, error => error.PropertyName == "telefono");
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("### ##")]
    [InlineData("<script>alert(1)</script>")]
    [InlineData("")]
    public void Rechaza_direcciones_invalidas(string direccion)
    {
        var resultado = _validador.Validate(Valida(direccion: direccion));

        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, error => error.PropertyName == "direccion");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Rechaza_identificadores_no_positivos(int identificador)
    {
        var resultado = _validador.Validate(Valida(paisId: identificador, departamentoId: identificador, municipioId: identificador));

        Assert.False(resultado.IsValid);
        Assert.Equal(3, resultado.Errors.Count);
    }

    [Fact]
    public void Reporta_todos_los_campos_invalidos_en_una_sola_respuesta()
    {
        var resultado = _validador.Validate(Valida(nombre: "A", telefono: "123", direccion: "abc"));

        Assert.False(resultado.IsValid);
        Assert.Equal(3, resultado.Errors.Count);
    }

    [Fact]
    public void Reporta_un_solo_mensaje_por_campo()
    {
        var resultado = _validador.Validate(Valida(nombre: ""));

        Assert.Single(resultado.Errors, error => error.PropertyName == "nombre");
    }
}
