using bigexapi.Model.Enum;

namespace bigexapi.Model;

public class IChatsAndUsers
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int IChatId { get; set; }
    public ChatType ChatType { get; set; }
}
