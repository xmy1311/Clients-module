namespace Coink.Usuarios.Application.Comun;

/// <summary>
/// Resultado de una operación.
/// Contiene un valor cuando la operación es exitosa, 
/// o un código de error y un mensaje cuando falla.
/// </summary>
/// <typeparam name="T">Tipo del valor devuelto cuando la operación es exitosa.</typeparam>
public sealed class Resultado<T>
{
    private Resultado(bool esExitoso, T? valor, CodigoError error, string? mensaje)
    {
        EsExitoso = esExitoso;
        Valor = valor;
        Error = error;
        Mensaje = mensaje;
    }

    public bool EsExitoso { get; }

    /// <summary>Valor producido. Solo tiene contenido cuando <see cref="EsExitoso"/> es verdadero.</summary>
    public T? Valor { get; }

    /// <summary>Motivo del fallo. Es <see cref="CodigoError.Ninguno"/> cuando la operación fue exitosa.</summary>
    public CodigoError Error { get; }

    /// <summary>Mensaje apto para mostrar al consumidor de la API.</summary>
    public string? Mensaje { get; }

    /// <summary>Crea un resultado exitoso.</summary>
    public static Resultado<T> Ok(T valor) => new(true, valor, CodigoError.Ninguno, null);

    /// <summary>Crea un resultado fallido.</summary>
    public static Resultado<T> Fallo(CodigoError error, string mensaje) => new(false, default, error, mensaje);
}
