namespace Toons.Application.Comun;

public sealed class Resultado<T>
{
    private readonly T? valor;
    private readonly ErrorAplicacion? error;

    private Resultado(T? valor, ErrorAplicacion? error)
    {
        this.valor = valor;
        this.error = error;
    }

    public bool EsExitoso => error is null;

    public T Valor => valor ?? throw new InvalidOperationException("El resultado es un error.");

    public ErrorAplicacion Error => error ?? throw new InvalidOperationException("El resultado es exitoso.");

    public static Resultado<T> Exito(T valor) => new(valor, null);

    public static Resultado<T> Fallo(string codigo, string? campo = null) => new(default, new ErrorAplicacion(codigo, campo));

    public static Resultado<T> Fallo(ErrorAplicacion error) => new(default, error);
}
