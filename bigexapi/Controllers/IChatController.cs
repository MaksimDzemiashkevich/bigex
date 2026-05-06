using bigexapi.Model;
using bigexapi.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        return _storage.GetIChats();
    }
}
