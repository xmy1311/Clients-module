namespace Coink.Usuarios.Application.Catalogos.Dtos;

/// <summary>Municipio del catálogo.</summary>
/// <param name="MunicipioId">Identificador. <example>1</example></param>
/// <param name="DepartamentoId">Departamento al que pertenece. <example>1</example></param>
/// <param name="Codigo">Código oficial dentro del departamento. <example>05001</example></param>
/// <param name="Nombre">Nombre del municipio. <example>Medellín</example></param>
public sealed record MunicipioResponse(int MunicipioId, int DepartamentoId, string Codigo, string Nombre);
