using Toons.Application.Autenticacion;

namespace Toons.Tests.Apoyo;

public sealed class HasherEspia(IPasswordHasher real) : IPasswordHasher
{
    public int Simulaciones { get; private set; }

    public string Cifrar(string contrasena) => real.Cifrar(contrasena);

    public bool Verificar(string contrasena, string hash) => real.Verificar(contrasena, hash);

    public void SimularVerificacion(string contrasena)
    {
        Simulaciones++;
        real.SimularVerificacion(contrasena);
    }
}
