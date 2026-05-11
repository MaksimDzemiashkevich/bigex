using bigexapi.Model;

namespace bigexapi.Storage;

public interface IStorage
{
    List<AbstractChat> GetIChats(int id);
}
