using bigexapi.Model;
using bigexapi.ModelDTO;

namespace bigexapi.Storage;

public interface IStorage
{
    List<AbstractChat> GetIChats(int id);
    public bool CreateUser(CreateUserData createUserData);
    public int AuthUser(UserAuth userAuth);
    public List<UserSearchDTO> SearchUsers(string query);
    public int GetUserIdByEmail(string email);
    public Guid CreatePrivateChatWithFirstMessage(int senderId, int recipientId, string messageText);
    public void SendMessage(string chatPublicId, int senderId, string text);
    UserSearchDTO? GetChatCompanion(string chatPublicId, int currentUserId);
}
