namespace Coink.Usuarios.Application.Catalogos.Dtos;

/// <summary>País del catálogo.</summary>
/// <param name="PaisId">Identificador. <example>1</example></param>
/// <param name="Codigo">Código ISO 3166-1 alfa-3. <example>COL</example></param>
/// <param name="Nombre">Nombre del país. <example>Colombia</example></param>
public sealed record PaisResponse(int PaisId, string Codigo, string Nombre);
