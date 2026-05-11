using bigexapi.Model;
using bigexapi.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bigexapi.Controllers;

public class IChatController : BaseController
{
    private IStorage _storage;

    public IChatController(IStorage storage)
    {
        _storage = storage;
    }

    [HttpGet("ichats")]
    public List<AbstractChat> GetIChats()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return _storage.GetIChats(userId);
    }
}
