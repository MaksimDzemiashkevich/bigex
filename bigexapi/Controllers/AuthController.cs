using bigexapi.ModelDTO;
using bigexapi.Storage;
using bigexapi.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace bigexapi.Controllers;

[AllowAnonymous]
public class AuthController : BaseController
{
    public IStorage mySqlStorage;
    private IConfiguration _configuration;

    public AuthController(IStorage storage, IConfiguration configuration)
    {
        mySqlStorage = storage;
        _configuration = configuration;
    }

    [HttpPost("signin")]
    public bool CreateUser([FromBody] CreateUserData createUserData)
    {
        return mySqlStorage.CreateUser(createUserData);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserAuth userAuth)
    {
        long userId = mySqlStorage.AuthUser(userAuth);

        if (userId == -1)
        {
            return Unauthorized(new { message = "Неверный логин или пароль" });
        }

        List<Claim> claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Email, userAuth.UEmail)
    };
        string securityKey = _configuration["JwtSettings:Key"];

        JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(2),
                signingCredentials: new SigningCredentials(
                    AuthOptions.GetSymmetricSecurityKey(securityKey),
                    SecurityAlgorithms.HmacSha256));

        string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return Ok(new
        {
            access_token = encodedJwt,
            email = userAuth.UEmail
        });
    }
}
