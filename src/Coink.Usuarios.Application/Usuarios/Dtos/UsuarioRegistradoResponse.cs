namespace Coink.Usuarios.Application.Usuarios.Dtos;

/// <summary>Confirmación del registro.</summary>
/// <param name="UsuarioId">Identificador asignado por la base de datos. <example>1</example></param>
/// <param name="FechaRegistro">Instante del registro, en UTC.</param>
public sealed record UsuarioRegistradoResponse(int UsuarioId, DateTimeOffset FechaRegistro);
