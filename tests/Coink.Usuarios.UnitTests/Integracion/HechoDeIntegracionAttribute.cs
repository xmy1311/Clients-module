using Xunit;

namespace Coink.Usuarios.UnitTests.Integracion;

/// <summary>
/// Marca una prueba que necesita la API y la base de datos en marcha.
/// 
///   $env:COINK_PRUEBAS_INTEGRACION = "1"   # PowerShell
///   export COINK_PRUEBAS_INTEGRACION=1     # bash
/// </summary>
public sealed class HechoDeIntegracionAttribute : FactAttribute
{
    public HechoDeIntegracionAttribute()
    {
        if (Environment.GetEnvironmentVariable("COINK_PRUEBAS_INTEGRACION") != "1")
        {
            Skip = "Prueba de integración: levante la solución y defina COINK_PRUEBAS_INTEGRACION=1.";
        }
    }
}
