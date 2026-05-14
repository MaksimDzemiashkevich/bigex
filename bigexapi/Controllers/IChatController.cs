using bigexapi.Model;
using bigexapi.ModelDTO;
using bigexapi.Storage;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

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
        int recipientId = _storage.GetUserIdByEmail(request.RecipientEmail);

        if (recipientId == -1) return BadRequest("Пользователь не найден");

        Guid newChatGuid = _storage.CreatePrivateChatWithFirstMessage(
            request.SenderId,
            recipientId,
            request.MessageText
        );

        return Ok(new { ichatPublicId = newChatGuid });
    }

    [HttpGet("messages")]
    public IActionResult GetMessages([FromQuery] string chatId)
    {
        try
        {
            using (var connection = new MySqlConnection("server=localhost;port=3306;database=bigex;user=root;password=mongoFisher!;"))
            {
                connection.Open();
                var messages = MySqlStorage.GetMessagesByChatId(connection, chatId);
                return Ok(messages);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("message")]
    public IActionResult SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            _storage.SendMessage(request.ChatId, request.SenderId, request.Text);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("chat-companion")]
    public IActionResult GetChatCompanion([FromQuery] string chatId, [FromQuery] int userId)
    {
        try
        {
            var companion = _storage.GetChatCompanion(chatId, userId);
            if (companion == null) return NotFound();
            return Ok(companion);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}