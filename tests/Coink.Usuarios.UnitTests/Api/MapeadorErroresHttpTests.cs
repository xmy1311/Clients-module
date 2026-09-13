using Coink.Usuarios.Api.Errores;
using Coink.Usuarios.Application.Comun;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Coink.Usuarios.UnitTests.Api;

public sealed class MapeadorErroresHttpTests
{
    [Theory]
    [InlineData(CodigoError.PaisNoExiste, StatusCodes.Status422UnprocessableEntity)]
    [InlineData(CodigoError.DepartamentoNoExiste, StatusCodes.Status422UnprocessableEntity)]
    [InlineData(CodigoError.MunicipioNoExiste, StatusCodes.Status422UnprocessableEntity)]
    [InlineData(CodigoError.DepartamentoNoPerteneceAlPais, StatusCodes.Status422UnprocessableEntity)]
    [InlineData(CodigoError.MunicipioNoPerteneceAlDepartamento, StatusCodes.Status422UnprocessableEntity)]
    [InlineData(CodigoError.TelefonoDuplicado, StatusCodes.Status409Conflict)]
    [InlineData(CodigoError.UsuarioNoEncontrado, StatusCodes.Status404NotFound)]
    public void Cada_error_de_negocio_tiene_su_codigo_http(CodigoError error, int estadoEsperado)
    {
        var (estado, tipo, titulo, codigo) = MapeadorErroresHttp.Mapear(error);

        Assert.Equal(estadoEsperado, estado);
        Assert.StartsWith("urn:coink:error:", tipo);
        Assert.False(string.IsNullOrWhiteSpace(titulo));
        Assert.False(string.IsNullOrWhiteSpace(codigo));
    }

    [Fact]
    public void Los_codigos_estables_no_se_repiten_entre_errores()
    {
        var codigos = Enum.GetValues<CodigoError>()
            .Where(error => error != CodigoError.Ninguno)
            .Select(error => MapeadorErroresHttp.Mapear(error).Codigo)
            .ToArray();

        Assert.Equal(codigos.Length, codigos.Distinct().Count());
    }
}
