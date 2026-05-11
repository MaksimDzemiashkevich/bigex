using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace bigexapi.Token;


public class AuthOptions
{
    public const string ISSUER = "AuthServer";
    public const string AUDIENCE = "AuthClient";
    public static SymmetricSecurityKey GetSymmetricSecurityKey(string str) => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(str));
}
