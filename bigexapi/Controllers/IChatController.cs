using bigexapi.Model;
using bigexapi.ModelDTO;
using bigexapi.Storage;
using Microsoft.AspNetCore.Mvc;

namespace bigexapi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IChatController : ControllerBase
{
    private readonly IStorage _storage;

    public IChatController(IStorage storage)
    {
        _storage = storage;
    }

    [HttpGet("ichats")]
    public List<AbstractChat> GetIChats([FromQuery] int userId)
    {
        return _storage.GetIChats(userId);
    }

    [HttpGet("search-users")]
    public IActionResult SearchUsers([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return Ok(new List<object>());

        var users = _storage.SearchUsers(query);
        return Ok(users);
    }

    [HttpPost("create-and-send")]
    public IActionResult CreateAndSend([FromBody] CreateChatRequest request)
    {
        // 1. Ищем получателя в базе по Email через UserRepository
        // (Метод поиска по email у тебя уже есть частично в CheckExistingUser)
        int recipientId = _storage.GetUserIdByEmail(request.RecipientEmail);

        if (recipientId == -1) return BadRequest("Пользователь не найден");

        // 2. Создаем новый чат и сохраняем сообщение
        // Возвращаем PublicId нового чата, чтобы фронт мог сделать редирект
        Guid newChatGuid = _storage.CreatePrivateChatWithFirstMessage(
            request.SenderId,
            recipientId,
            request.MessageText
        );

        return Ok(new { ichatPublicId = newChatGuid });
    }
}