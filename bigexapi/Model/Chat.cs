using bigexapi.Model.Enum;

namespace bigexapi.Model;

public class Chat : AbstractChat
{
    public Chat()
    {
        base.ChatType = ChatType.PrivateChat;
    }
}
