using Coink.Usuarios.Application.Comun;
using Microsoft.AspNetCore.Mvc;

namespace Coink.Usuarios.Api.Errores;

/// <summary>Convierte un resultado fallido en la respuesta HTTP correspondiente.</summary>
public static class ResultadoHttpExtensions
{
    /// <summary>Construye el <see cref="ProblemDetails"/> del fallo con su código de estado.</summary>
    public static ObjectResult AProblemDetails<T>(this Resultado<T> resultado, HttpContext contexto)
    {
        var (estado, tipo, titulo, codigo) = MapeadorErroresHttp.Mapear(resultado.Error);
        var problema = Problemas.Crear(contexto, estado, tipo, titulo, resultado.Mensaje, codigo);

        return new ObjectResult(problema)
        {
            StatusCode = estado,
            ContentTypes = { "application/problem+json" }
        };
    }
}
