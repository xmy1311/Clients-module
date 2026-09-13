using Coink.Usuarios.Application.Usuarios.Dtos;
using Coink.Usuarios.Application.Usuarios.Interfaces;
using Coink.Usuarios.Application.Usuarios.Modelos;

namespace Coink.Usuarios.UnitTests.Dobles;


internal sealed class RepositorioUsuarioFalso : IUsuarioRepositorio
{
    private readonly RespuestaRegistro _respuesta;
    private readonly UsuarioDetalleResponse? _detalle;
    private readonly Exception? _excepcion;

    public RepositorioUsuarioFalso(
        RespuestaRegistro? respuesta = null,
        UsuarioDetalleResponse? detalle = null,
        Exception? excepcion = null)
    {
        _respuesta = respuesta ?? new RespuestaRegistro(0, 1, DateTimeOffset.UtcNow);
        _detalle = detalle;
        _excepcion = excepcion;
    }

    /// <summary>Último valor recibido, para comprobar que llegó normalizado.</summary>
    public NuevoUsuario? UltimoUsuarioRecibido { get; private set; }

    public Task<RespuestaRegistro> RegistrarAsync(NuevoUsuario usuario, CancellationToken cancelacion)
    {
        UltimoUsuarioRecibido = usuario;

        return _excepcion is not null
            ? Task.FromException<RespuestaRegistro>(_excepcion)
            : Task.FromResult(_respuesta);
    }

    public Task<UsuarioDetalleResponse?> ObtenerPorIdAsync(int usuarioId, CancellationToken cancelacion) =>
        _excepcion is not null
            ? Task.FromException<UsuarioDetalleResponse?>(_excepcion)
            : Task.FromResult(_detalle);
}
