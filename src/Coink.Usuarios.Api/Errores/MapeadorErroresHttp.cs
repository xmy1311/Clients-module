using Coink.Usuarios.Application.Comun;
using Microsoft.AspNetCore.Http;

namespace Coink.Usuarios.Api.Errores;

/// <summary>
/// Traducción de los errores de negocio a HTTP.
/// </summary>
public static class MapeadorErroresHttp
{
    /// <summary>Devuelve el estado, el tipo, el título y el código estable de un error de negocio.</summary>
    public static (int Estado, string Tipo, string Titulo, string Codigo) Mapear(CodigoError error) => error switch
    {
        CodigoError.PaisNoExiste => (
            StatusCodes.Status422UnprocessableEntity,
            "urn:coink:error:pais-no-existe",
            "Los datos enviados no son válidos.",
            "PAIS_NO_EXISTE"),

        CodigoError.DepartamentoNoExiste => (
            StatusCodes.Status422UnprocessableEntity,
            "urn:coink:error:departamento-no-existe",
            "Los datos enviados no son válidos.",
            "DEPARTAMENTO_NO_EXISTE"),

        CodigoError.MunicipioNoExiste => (
            StatusCodes.Status422UnprocessableEntity,
            "urn:coink:error:municipio-no-existe",
            "Los datos enviados no son válidos.",
            "MUNICIPIO_NO_EXISTE"),

        CodigoError.DepartamentoNoPerteneceAlPais => (
            StatusCodes.Status422UnprocessableEntity,
            "urn:coink:error:departamento-no-pertenece-al-pais",
            "Los datos enviados no son válidos.",
            "DEPARTAMENTO_NO_PERTENECE_AL_PAIS"),

        CodigoError.MunicipioNoPerteneceAlDepartamento => (
            StatusCodes.Status422UnprocessableEntity,
            "urn:coink:error:municipio-no-pertenece-al-departamento",
            "Los datos enviados no son válidos.",
            "MUNICIPIO_NO_PERTENECE_AL_DEPARTAMENTO"),

        CodigoError.TelefonoDuplicado => (
            StatusCodes.Status409Conflict,
            "urn:coink:error:telefono-duplicado",
            "Conflicto con el estado actual.",
            "TELEFONO_DUPLICADO"),

        CodigoError.UsuarioNoEncontrado => (
            StatusCodes.Status404NotFound,
            "urn:coink:error:usuario-no-encontrado",
            "Recurso no encontrado.",
            "USUARIO_NO_ENCONTRADO"),

        _ => (
            StatusCodes.Status500InternalServerError,
            "urn:coink:error:interno",
            "Error interno del servidor.",
            "ERROR_INTERNO")
    };
}
