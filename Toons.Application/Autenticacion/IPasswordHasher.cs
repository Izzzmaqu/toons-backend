namespace Toons.Application.Autenticacion;

public interface IPasswordHasher
{
    string Cifrar(string contrasena);

    bool Verificar(string contrasena, string hash);

    void SimularVerificacion(string contrasena);
}
