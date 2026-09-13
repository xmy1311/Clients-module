using Coink.Usuarios.Application.Comun;

namespace Coink.Usuarios.Application.Usuarios.Dtos;

/// <summary>Datos necesarios para registrar un usuario.</summary>
/// <param name="Nombre">Nombre completo. Solo letras, espacios, apóstrofos y guiones. <example>Xiomara Zapata</example></param>
/// <param name="Telefono">Teléfono en formato internacional. <example>+573001234567</example></param>
/// <param name="PaisId">Identificador del país. <example>1</example></param>
/// <param name="DepartamentoId">Identificador del departamento, que debe pertenecer al país. <example>1</example></param>
/// <param name="MunicipioId">Identificador del municipio, que debe pertenecer al departamento. <example>1</example></param>
/// <param name="Direccion">Dirección de residencia. <example>Calle 10 # 43-12 Apto 301</example></param>
public sealed record RegistrarUsuarioRequest(
    string Nombre,
    string Telefono,
    int PaisId,
    int DepartamentoId,
    int MunicipioId,
    string Direccion)
{
    // Propiedades calculadas con la forma normalizada de cada campo.
    
    /// <summary>Nombre sin espacios sobrantes.</summary>
    internal string NombreNormalizado => Normalizador.Texto(Nombre);

    /// <summary>Teléfono, sin separadores visuales.</summary>
    internal string TelefonoNormalizado => Normalizador.Telefono(Telefono);

    /// <summary>Dirección sin espacios sobrantes.</summary>
    internal string DireccionNormalizada => Normalizador.Texto(Direccion);
}
