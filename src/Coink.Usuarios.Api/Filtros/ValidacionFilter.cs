using Coink.Usuarios.Api.Errores;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Coink.Usuarios.Api.Filtros;

/// <summary>
/// Ejecuta el validador del argumento antes de que la acción se ejecute.
/// </summary>
public sealed class ValidacionFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _servicios;

    public ValidacionFilter(IServiceProvider servicios) => _servicios = servicios;

    public async Task OnActionExecutionAsync(ActionExecutingContext contexto, ActionExecutionDelegate siguiente)
    {
        foreach (var argumento in contexto.ActionArguments.Values)
        {
            if (argumento is null)
            {
                continue;
            }

            var tipoValidador = typeof(IValidator<>).MakeGenericType(argumento.GetType());

            if (_servicios.GetService(tipoValidador) is not IValidator validador)
            {
                continue;
            }

            var resultado = await validador.ValidateAsync(
                new ValidationContext<object>(argumento),
                contexto.HttpContext.RequestAborted);

            if (resultado.IsValid)
            {
                continue;
            }

            var errores = resultado.Errors
                .GroupBy(fallo => fallo.PropertyName)
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo.Select(fallo => fallo.ErrorMessage).ToArray());

            var problema = Problemas.DecorarValidacion(
                contexto.HttpContext,
                new ValidationProblemDetails(errores));

            contexto.Result = new ObjectResult(problema)
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ContentTypes = { "application/problem+json" }
            };

            return;
        }

        await siguiente();
    }
}
