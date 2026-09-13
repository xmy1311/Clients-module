using Coink.Usuarios.Api.Errores;
using Coink.Usuarios.Application.Usuarios.Dtos;
using Coink.Usuarios.Application.Usuarios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coink.Usuarios.Api.Controllers;

/// <summary>Registro y consulta de usuarios.</summary>
[ApiController]
[Route("api/usuarios")]
[Tags("Usuarios")]
[Produces("application/json")]
public sealed class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _servicio;

    /// <summary>Recibe el caso de uso por inyección de dependencias.</summary>
    public UsuariosController(IUsuarioService servicio) => _servicio = servicio;

    /// <summary>Registra un usuario con su ubicación.</summary>
    /// <remarks>
    ///
    /// Datos de ejemplo: país 1 (Colombia),
    /// departamento 1 (Antioquia), municipio 1 (Medellín).
    /// </remarks>
    /// <param name="peticion">Datos del usuario.</param>
    /// <param name="cancelacion">Token de cancelación de la petición.</param>
    /// <response code="201">Usuario registrado.</response>
    /// <response code="400">Algún campo no cumple el formato esperado.</response>
    /// <response code="409">El teléfono ya está registrado.</response>
    /// <response code="422">El país, el departamento o el municipio no existen, o no forman una cadena coherente.</response>
    /// <response code="500">Error inesperado.</response>
    /// <response code="503">La base de datos no está disponible. La cabecera Retry-After indica cuándo reintentar.</response>
    [HttpPost]
    [ProducesResponseType(typeof(UsuarioRegistradoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistrarUsuarioRequest peticion,
        CancellationToken cancelacion)
    {
        var resultado = await _servicio.RegistrarAsync(peticion, cancelacion);

        if (!resultado.EsExitoso)
        {
            return resultado.AProblemDetails(HttpContext);
        }

        return CreatedAtAction(nameof(Obtener), new { id = resultado.Valor!.UsuarioId }, resultado.Valor);
    }

    /// <summary>Obtiene un usuario registrado.</summary>
    /// <remarks>Devuelve los nombres de país, departamento y municipio.</remarks>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="cancelacion">Token de cancelación de la petición.</param>
    /// <response code="200">Usuario encontrado.</response>
    /// <response code="404">No existe un usuario con ese identificador.</response>
    /// <response code="500">Error inesperado.</response>
    /// <response code="503">La base de datos no está disponible.</response>
    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType(typeof(UsuarioDetalleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Obtener(int id, CancellationToken cancelacion)
    {
        var resultado = await _servicio.ObtenerAsync(id, cancelacion);

        return resultado.EsExitoso
            ? Ok(resultado.Valor)
            : resultado.AProblemDetails(HttpContext);
    }
}
