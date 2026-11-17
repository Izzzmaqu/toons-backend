using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Toons.Application.Autenticacion;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Seguridad;

public sealed class GeneradorTokenJwt(IOptions<JwtOptions> opciones, TimeProvider reloj) : IGeneradorToken
{
    public const string TipoClaimRol = "rol";
    public const string TipoClaimNombre = "nombre";

    public string Generar(Usuario usuario)
    {
        var jwt = opciones.Value;
        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave));
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(TipoClaimNombre, usuario.Nombre),
                new Claim(TipoClaimRol, usuario.Rol.Nombre),
            ]),
            Issuer = jwt.Emisor,
            Audience = jwt.Audiencia,
            Expires = reloj.GetUtcNow().UtcDateTime.AddMinutes(jwt.MinutosVigencia),
            SigningCredentials = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256),
        };
        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}
