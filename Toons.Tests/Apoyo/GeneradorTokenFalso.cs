using Toons.Application.Autenticacion;
using Toons.Domain.Entidades;

namespace Toons.Tests.Apoyo;

public sealed class GeneradorTokenFalso : IGeneradorToken
{
    public const string TokenFijo = "token-de-prueba";

    public string Generar(Usuario usuario) => TokenFijo;
}
