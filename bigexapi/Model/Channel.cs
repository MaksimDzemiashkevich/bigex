using bigexapi.Model.Enum;

namespace bigexapi.Model;

public class Channel : AbstractChat
{
    public Channel()
    {
        base.ChatType = ChatType.Channel;
    }
}
