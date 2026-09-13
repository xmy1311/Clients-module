using Coink.Usuarios.Application.Usuarios.Dtos;
using Coink.Usuarios.Application.Usuarios.Modelos;

namespace Coink.Usuarios.Application.Usuarios.Interfaces;

/// <summary>Acceso a los datos de usuario.</summary>
public interface IUsuarioRepositorio
{
    /// <summary>Invoca <c>sp_registrar_usuario</c> y devuelve su código de resultado.</summary>
    Task<RespuestaRegistro> RegistrarAsync(NuevoUsuario usuario, CancellationToken cancelacion);

    /// <summary>Invoca <c>fn_obtener_usuario</c>. Devuelve <c>null</c> si no existe.</summary>
    Task<UsuarioDetalleResponse?> ObtenerPorIdAsync(int usuarioId, CancellationToken cancelacion);
}
