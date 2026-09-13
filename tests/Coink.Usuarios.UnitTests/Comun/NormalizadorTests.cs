using Coink.Usuarios.Application.Comun;
using Xunit;

namespace Coink.Usuarios.UnitTests.Comun;

public sealed class NormalizadorTests
{
    [Theory]
    [InlineData("  Ana   María  ", "Ana María")]
    [InlineData("Xiomara Zapata", "Xiomara Zapata")]
    [InlineData("   ", "")]
    [InlineData(null, "")]
    public void Texto_recorta_y_colapsa_espacios(string? entrada, string esperado) =>
        Assert.Equal(esperado, Normalizador.Texto(entrada));

    [Theory]
    [InlineData("+57 (300) 123-4567", "+573001234567")]
    [InlineData("  +573001234567 ", "+573001234567")]
    [InlineData("+57.300.123.4567", "+573001234567")]
    public void Telefono_elimina_separadores_visuales(string entrada, string esperado) =>
        Assert.Equal(esperado, Normalizador.Telefono(entrada));
}
