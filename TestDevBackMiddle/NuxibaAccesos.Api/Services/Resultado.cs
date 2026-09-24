namespace NuxibaAccesos.Api.Services;

public enum TipoError
{
    Ninguno,
    Validacion,
    NoEncontrado,
    Conflicto
}

// Resultado de una operación del servicio: el valor o el motivo por el que falló
public class Resultado<T>
{
    public T? Valor { get; private init; }

    public TipoError Error { get; private init; }

    public string? Mensaje { get; private init; }

    public bool EsExitoso => Error == TipoError.Ninguno;

    public static Resultado<T> Ok(T valor) => new() { Valor = valor };

    public static Resultado<T> Falla(TipoError error, string mensaje) => new() { Error = error, Mensaje = mensaje };
}
