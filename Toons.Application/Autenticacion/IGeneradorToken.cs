using Toons.Domain.Entidades;

namespace Toons.Application.Autenticacion;

public interface IGeneradorToken
{
    string Generar(Usuario usuario);
}
