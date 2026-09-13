using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Coink.Usuarios.Api.Errores;

/// <summary>
/// Construcción de respuestas de error. 
/// </summary>
internal static class Problemas
{
    /// <summary>Identificador de la petición.</summary>
    internal static string TraceId(HttpContext contexto) =>
        Activity.Current?.Id ?? contexto.TraceIdentifier;

    /// <summary>Crea un <see cref="ProblemDetails"/> con las extensiones propias.</summary>
    internal static ProblemDetails Crear(
        HttpContext contexto,
        int estado,
        string tipo,
        string titulo,
        string? detalle,
        string codigo)
    {
        var problema = new ProblemDetails
        {
            Status = estado,
            Type = tipo,
            Title = titulo,
            Detail = detalle,
            Instance = contexto.Request.Path
        };

        problema.Extensions["codigo"] = codigo;
        problema.Extensions["traceId"] = TraceId(contexto);

        return problema;
    }

    /// <summary>Completa un error de validación para que tenga la misma forma que los demás.</summary>
    internal static ValidationProblemDetails DecorarValidacion(HttpContext contexto, ValidationProblemDetails problema)
    {
        problema.Status = StatusCodes.Status400BadRequest;
        problema.Type = "urn:coink:error:validacion";
        problema.Title = "Uno o más campos son inválidos.";
        problema.Instance = contexto.Request.Path;
        problema.Extensions["codigo"] = "VALIDACION";
        problema.Extensions["traceId"] = TraceId(contexto);

        return problema;
    }
}
