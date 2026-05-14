using bigexapi.ModelDTO;
using bigexapi.Storage;
using Microsoft.AspNetCore.Mvc;

namespace bigexapi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IStorage _storage;

    public AuthController(IStorage storage)
    {
        _storage = storage;
    }

    [HttpPost("signin")]
    public bool CreateUser([FromBody] CreateUserData createUserData)
    {
        return _storage.CreateUser(createUserData);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserAuth userAuth)
    {
        int userId = _storage.AuthUser(userAuth);

        if (userId == -1)
        {
            return Unauthorized(new { message = "Неверный логин или пароль" });
        }

        return Ok(new
        {
            userId = userId,
            email = userAuth.UEmail
        });
    }
}