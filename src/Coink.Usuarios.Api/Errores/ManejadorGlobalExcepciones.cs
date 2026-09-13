using System.Data.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace Coink.Usuarios.Api.Errores;

/// <summary>
/// Único punto donde se capturan excepciones en toda la aplicación.
/// Registra el detalle completo en el log y devuelve al cliente una respuesta
/// </summary>
internal sealed class ManejadorGlobalExcepciones : IExceptionHandler
{
    private readonly ILogger<ManejadorGlobalExcepciones> _registro;

    public ManejadorGlobalExcepciones(ILogger<ManejadorGlobalExcepciones> registro) => _registro = registro;

    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext contexto,
        Exception excepcion,
        CancellationToken cancelacion)
    {
    
        if (excepcion is OperationCanceledException && contexto.RequestAborted.IsCancellationRequested)
        {
            _registro.LogInformation("El cliente canceló la petición {Ruta}.", contexto.Request.Path);
            return true;
        }

        var esTransitoria = excepcion is DbException { IsTransient: true } or TimeoutException;

        var estado = esTransitoria
            ? StatusCodes.Status503ServiceUnavailable
            : StatusCodes.Status500InternalServerError;

        _registro.LogError(
            excepcion,
            "Fallo no controlado en {Ruta}. Estado {Estado}. TraceId {TraceId}.",
            contexto.Request.Path, estado, Problemas.TraceId(contexto));

        var problema = esTransitoria
            ? Problemas.Crear(
                contexto, estado,
                "urn:coink:error:servicio-no-disponible",
                "Servicio no disponible.",
                "El servicio no está disponible temporalmente. Intente de nuevo en unos segundos.",
                "SERVICIO_NO_DISPONIBLE")
            : Problemas.Crear(
                contexto, estado,
                "urn:coink:error:interno",
                "Error interno del servidor.",
                "Ocurrió un error inesperado. Cite el traceId para el diagnóstico.",
                "ERROR_INTERNO");

        contexto.Response.StatusCode = estado;

        if (esTransitoria)
        {
            contexto.Response.Headers.RetryAfter = "5";
        }

  
        contexto.Response.ContentType = "application/problem+json";
        await contexto.Response.WriteAsJsonAsync(problema, cancellationToken: cancelacion);

        return true;
    }
}
