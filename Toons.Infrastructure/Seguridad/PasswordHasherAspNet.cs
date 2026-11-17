using Microsoft.AspNetCore.Identity;
using Toons.Application.Autenticacion;

namespace Toons.Infrastructure.Seguridad;

public sealed class PasswordHasherAspNet : IPasswordHasher
{
    private static readonly object UsuarioNulo = new();

    private readonly PasswordHasher<object> hasher = new();
    private readonly string hashFicticio;

    public PasswordHasherAspNet()
    {
        hashFicticio = Cifrar("contrasena-ficticia-para-igualar-tiempos");
    }

    public string Cifrar(string contrasena)
    {
        return hasher.HashPassword(UsuarioNulo, contrasena);
    }

    public bool Verificar(string contrasena, string hash)
    {
        var resultado = hasher.VerifyHashedPassword(UsuarioNulo, hash, contrasena);
        return resultado != PasswordVerificationResult.Failed;
    }

    public void SimularVerificacion(string contrasena)
    {
        Verificar(contrasena, hashFicticio);
    }
}
