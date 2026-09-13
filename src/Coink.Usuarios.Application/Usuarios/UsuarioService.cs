using Coink.Usuarios.Application.Comun;
using Coink.Usuarios.Application.Usuarios.Dtos;
using Coink.Usuarios.Application.Usuarios.Interfaces;
using Coink.Usuarios.Application.Usuarios.Modelos;
using Microsoft.Extensions.Logging;

namespace Coink.Usuarios.Application.Usuarios;

/// <summary>
/// registra un usuario, invoca el repositorio 
/// que llama al procedimiento almacenado y traduce su código a un resultado de negocio.
/// </summary>
public sealed class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepositorio _repositorio;
    private readonly ILogger<UsuarioService> _registro;

    public UsuarioService(IUsuarioRepositorio repositorio, ILogger<UsuarioService> registro)
    {
        _repositorio = repositorio;
        _registro = registro;
    }

    public async Task<Resultado<UsuarioRegistradoResponse>> RegistrarAsync(
        RegistrarUsuarioRequest peticion,
        CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(peticion);

        var nuevo = new NuevoUsuario(
            peticion.NombreNormalizado,
            peticion.TelefonoNormalizado,
            peticion.PaisId,
            peticion.DepartamentoId,
            peticion.MunicipioId,
            peticion.DireccionNormalizada);

        var respuesta = await _repositorio.RegistrarAsync(nuevo, cancelacion);

        if (respuesta.Codigo == 0)
        {
            if (respuesta.UsuarioId is not { } usuarioId || respuesta.FechaRegistro is not { } fecha)
            {
                throw new InvalidOperationException(
                    "sp_registrar_usuario informó éxito pero no devolvió el identificador o la fecha.");
            }

            // Se registran identificadores
            _registro.LogInformation(
                "Usuario {UsuarioId} registrado en el municipio {MunicipioId}.",
                usuarioId, nuevo.MunicipioId);

            return Resultado<UsuarioRegistradoResponse>.Ok(new UsuarioRegistradoResponse(usuarioId, fecha));
        }

        var (error, mensaje) = Traducir(respuesta.Codigo, nuevo);

        _registro.LogWarning(
            "Registro rechazado con código {CodigoProcedimiento} ({Error}). País {PaisId}, departamento {DepartamentoId}, municipio {MunicipioId}.",
            respuesta.Codigo, error, nuevo.PaisId, nuevo.DepartamentoId, nuevo.MunicipioId);

        return Resultado<UsuarioRegistradoResponse>.Fallo(error, mensaje);
    }

    public async Task<Resultado<UsuarioDetalleResponse>> ObtenerAsync(int usuarioId, CancellationToken cancelacion)
    {
        var detalle = await _repositorio.ObtenerPorIdAsync(usuarioId, cancelacion);

        return detalle is null
            ? Resultado<UsuarioDetalleResponse>.Fallo(
                CodigoError.UsuarioNoEncontrado,
                $"No existe un usuario con el identificador {usuarioId}.")
            : Resultado<UsuarioDetalleResponse>.Ok(detalle);
    }

    /// <summary>
    /// Traduce el código del procedimiento.
    /// </summary>
    private static (CodigoError Error, string Mensaje) Traducir(short codigo, NuevoUsuario usuario) => codigo switch
    {
        1 => (CodigoError.PaisNoExiste,
              $"El país {usuario.PaisId} no existe."),
        2 => (CodigoError.DepartamentoNoExiste,
              $"El departamento {usuario.DepartamentoId} no existe."),
        3 => (CodigoError.MunicipioNoExiste,
              $"El municipio {usuario.MunicipioId} no existe."),
        4 => (CodigoError.DepartamentoNoPerteneceAlPais,
              $"El departamento {usuario.DepartamentoId} no pertenece al país {usuario.PaisId}."),
        5 => (CodigoError.MunicipioNoPerteneceAlDepartamento,
              $"El municipio {usuario.MunicipioId} no pertenece al departamento {usuario.DepartamentoId}."),
        6 => (CodigoError.TelefonoDuplicado,
              "El teléfono indicado ya está registrado."),
        _ => throw new InvalidOperationException(
              $"sp_registrar_usuario devolvió un código no contemplado: {codigo}.")
    };
}
