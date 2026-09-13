namespace Coink.Usuarios.Application.Usuarios.Modelos;

/// <summary>
/// Salida del procedimiento <c>sp_registrar_usuario</c>.
/// </summary>
/// <param name="Codigo">0 exitoso; 1 a 6 según el motivo del rechazo.</param>
/// <param name="UsuarioId">Identificador generado, solo cuando el código es 0.</param>
/// <param name="FechaRegistro">Fecha asignada por la base, solo cuando el código es 0.</param>
public sealed record RespuestaRegistro(short Codigo, int? UsuarioId, DateTimeOffset? FechaRegistro);
