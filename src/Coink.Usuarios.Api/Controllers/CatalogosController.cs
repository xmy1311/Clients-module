using Coink.Usuarios.Application.Catalogos.Dtos;
using Coink.Usuarios.Application.Catalogos.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coink.Usuarios.Api.Controllers;

/// <summary>
/// Catálogos paramétricos de país, departamento y municipio.
/// </summary>
[ApiController]
[Route("api/catalogos")]
[Tags("Catálogos")]
[Produces("application/json")]
public sealed class CatalogosController : ControllerBase
{
    private readonly ICatalogoRepositorio _repositorio;

    public CatalogosController(ICatalogoRepositorio repositorio) => _repositorio = repositorio;

    /// <summary>Lista los países disponibles.</summary>
    /// <param name="cancelacion">Token de cancelación de la petición.</param>
    /// <response code="200">Listado de países.</response>
    [HttpGet("paises")]
    [ProducesResponseType(typeof(IReadOnlyList<PaisResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ObtenerPaises(CancellationToken cancelacion) =>
        Ok(await _repositorio.ObtenerPaisesAsync(cancelacion));

    /// <summary>Lista los departamentos de un país.</summary>
    /// <remarks>Si el país no existe se devuelve una lista vacía:</remarks>
    /// <param name="paisId">Identificador del país.</param>
    /// <param name="cancelacion">Token de cancelación de la petición.</param>
    /// <response code="200">Listado de departamentos.</response>
    [HttpGet("paises/{paisId:int:min(1)}/departamentos")]
    [ProducesResponseType(typeof(IReadOnlyList<DepartamentoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ObtenerDepartamentos(int paisId, CancellationToken cancelacion) =>
        Ok(await _repositorio.ObtenerDepartamentosAsync(paisId, cancelacion));

    /// <summary>Lista los municipios de un departamento.</summary>
    /// <param name="departamentoId">Identificador del departamento.</param>
    /// <param name="cancelacion">Token de cancelación de la petición.</param>
    /// <response code="200">Listado de municipios.</response>
    [HttpGet("departamentos/{departamentoId:int:min(1)}/municipios")]
    [ProducesResponseType(typeof(IReadOnlyList<MunicipioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ObtenerMunicipios(int departamentoId, CancellationToken cancelacion) =>
        Ok(await _repositorio.ObtenerMunicipiosAsync(departamentoId, cancelacion));
}
