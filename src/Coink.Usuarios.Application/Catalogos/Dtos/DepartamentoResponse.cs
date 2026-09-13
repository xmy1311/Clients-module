namespace Coink.Usuarios.Application.Catalogos.Dtos;

/// <summary>Departamento del catálogo.</summary>
/// <param name="DepartamentoId">Identificador. <example>1</example></param>
/// <param name="PaisId">País al que pertenece. <example>1</example></param>
/// <param name="Codigo">Código oficial dentro del país. <example>05</example></param>
/// <param name="Nombre">Nombre del departamento. <example>Antioquia</example></param>
public sealed record DepartamentoResponse(int DepartamentoId, int PaisId, string Codigo, string Nombre);
