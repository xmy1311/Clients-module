using System.Text.RegularExpressions;

namespace Coink.Usuarios.Application.Comun;

/// <summary>
/// Normaliza los valores de entrada antes de validarlos y antes de persistirlos.
/// </summary>
public static partial class Normalizador
{
    /// <summary>Recorta los extremos y colapsa los espacios interiores.</summary>
    public static string Texto(string? valor) =>
        valor is null ? string.Empty : EspaciosMultiples().Replace(valor.Trim(), " ");

    /// <summary>
    /// Recorta y elimina los separadores visuales del teléfono (espacios, guiones,
    /// puntos y paréntesis)
    /// </summary>
    public static string Telefono(string? valor) =>
        valor is null ? string.Empty : SeparadoresTelefono().Replace(valor.Trim(), string.Empty);

    [GeneratedRegex(@"\s+", RegexOptions.None, 100)]
    private static partial Regex EspaciosMultiples();

    [GeneratedRegex(@"[\s\-\.\(\)]", RegexOptions.None, 100)]
    private static partial Regex SeparadoresTelefono();
}
