namespace Coink.Usuarios.Application.Usuarios.Dtos;

/// <summary>Detalle de un usuario, con la ubicación resuelta a nombres.</summary>
/// <param name="UsuarioId">Identificador del usuario. <example>1</example></param>
/// <param name="Nombre">Nombre completo. <example>Xiomara Zapata</example></param>
/// <param name="Telefono">Teléfono en formato E.164. <example>+573001234567</example></param>
/// <param name="Direccion">Dirección de residencia. <example>Calle 10 # 43-12 Apto 301</example></param>
/// <param name="Pais">Nombre del país. <example>Colombia</example></param>
/// <param name="Departamento">Nombre del departamento. <example>Antioquia</example></param>
/// <param name="Municipio">Nombre del municipio. <example>Medellín</example></param>
/// <param name="FechaRegistro">Instante del registro, en UTC.</param>
public sealed record UsuarioDetalleResponse(
    int UsuarioId,
    string Nombre,
    string Telefono,
    string Direccion,
    string Pais,
    string Departamento,
    string Municipio,
    DateTimeOffset FechaRegistro);
