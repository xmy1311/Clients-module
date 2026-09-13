using Coink.Usuarios.Application.Catalogos.Dtos;

namespace Coink.Usuarios.Application.Catalogos.Interfaces;

/// <summary>
/// Lecturas de catálogo. 
/// </summary>
public interface ICatalogoRepositorio
{
    Task<IReadOnlyList<PaisResponse>> ObtenerPaisesAsync(CancellationToken cancelacion);
 
    Task<IReadOnlyList<DepartamentoResponse>> ObtenerDepartamentosAsync(int paisId, CancellationToken cancelacion);
  
    Task<IReadOnlyList<MunicipioResponse>> ObtenerMunicipiosAsync(int departamentoId, CancellationToken cancelacion);
}
