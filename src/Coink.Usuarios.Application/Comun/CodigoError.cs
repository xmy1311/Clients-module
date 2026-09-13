namespace Coink.Usuarios.Application.Comun;

/// <summary>
/// Desenlaces esperados de un caso de uso. No son excepciones: son resultados
/// previstos que el borde HTTP traduce a un código de estado.
/// </summary>
public enum CodigoError
{
    /// <summary>Sin error.</summary>
    Ninguno = 0,

    PaisNoExiste = 1,

    DepartamentoNoExiste = 2,

    MunicipioNoExiste = 3,

    DepartamentoNoPerteneceAlPais = 4,

    MunicipioNoPerteneceAlDepartamento = 5,

    TelefonoDuplicado = 6,
   
    UsuarioNoEncontrado = 7
}
