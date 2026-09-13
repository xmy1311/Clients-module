namespace Coink.Usuarios.Application.Usuarios.Modelos;

/// <summary>
/// Datos ya validados y normalizados que viajan del servicio al repositorio.
/// </summary>
public sealed record NuevoUsuario(
    string Nombre,
    string Telefono,
    int PaisId,
    int DepartamentoId,
    int MunicipioId,
    string Direccion);
