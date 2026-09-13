using Coink.Usuarios.Application.Comun;
using Coink.Usuarios.Application.Usuarios.Dtos;

namespace Coink.Usuarios.Application.Usuarios.Interfaces;

/// <summary>Casos de uso de usuario.</summary>
public interface IUsuarioService
{
    /// <summary>Normaliza, registra y traduce el código del procedimiento a un resultado de negocio.</summary>
    Task<Resultado<UsuarioRegistradoResponse>> RegistrarAsync(RegistrarUsuarioRequest peticion, CancellationToken cancelacion);

    /// <summary>Obtiene el detalle de un usuario.</summary>
    Task<Resultado<UsuarioDetalleResponse>> ObtenerAsync(int usuarioId, CancellationToken cancelacion);
}
