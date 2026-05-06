using bigexapi.Model.Enum;

namespace bigexapi.Model;

public class Group : AbstractChat
{
    public Group()
    {
        base.ChatType = ChatType.Group;
    }
}
