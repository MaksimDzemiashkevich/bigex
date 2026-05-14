using bigexapi.Model;
using bigexapi.ModelDTO;
using bigexapi.Repositories;
using MySql.Data.MySqlClient;

namespace bigexapi.Storage;

public class MySqlStorage : IStorage
{
    private string _connectionString;

    public MySqlStorage(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<AbstractChat> GetIChats(int id)
    {
        using MySqlConnection mySqlConnection = new MySqlConnection(_connectionString);
        mySqlConnection.Open();

        ChatRepository.CreateTableichat(mySqlConnection);
        ChatRepository.CreateTableichatsandusers(mySqlConnection);
        return UserRepository.SearchichatsWithUser(mySqlConnection, id);

    }

    public bool CreateUser(CreateUserData createUserData)
    {
        using MySqlConnection mySqlConnection = new MySqlConnection(_connectionString);
        mySqlConnection.Open();
        UserRepository.CreateTableUser(mySqlConnection);
        if (UserRepository.CheckExistingUser(mySqlConnection, createUserData))
        {
            return false;
        }
        return UserRepository.InsertUser(mySqlConnection, createUserData);
    }

    public int AuthUser(UserAuth userAuth)
    {
        using MySqlConnection mySqlConnection = new MySqlConnection(_connectionString);
        mySqlConnection.Open();
        UserRepository.CreateTableUser(mySqlConnection);
        return UserRepository.CheckUser(mySqlConnection, userAuth);
    }

    public List<UserSearchDTO> SearchUsers(string query)
    {
        using MySqlConnection connection = new MySqlConnection(_connectionString);
        connection.Open();
        List<UserSearchDTO> users = UserRepository.SearchUsers(connection, query);
        return users;
    }

    public int GetUserIdByEmail(string email)
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT userId FROM user WHERE userEmail = @email LIMIT 1;";
        command.Parameters.AddWithValue("@email", email);

        var result = command.ExecuteScalar();
        return result != null ? Convert.ToInt32(result) : -1;
    }

    public Guid CreatePrivateChatWithFirstMessage(int senderId, int recipientId, string messageText)
    {
        using MySqlConnection connection = new MySqlConnection(_connectionString);
        connection.Open();
        using MySqlTransaction transaction = connection.BeginTransaction();
        MessageRepository.CreateMessageTable(connection);
        try
        {
            Guid chatGuid = Guid.NewGuid();
            int chatId;
            using (var cmd = new MySqlCommand(@"
            INSERT INTO ichat (ichatPublicId, ichatName, chatType, ichatMainImage, ichatDescription) 
            VALUES (@pId, @name, 1, '', ''); 
            SELECT LAST_INSERT_ID();", connection, transaction))
            {
                cmd.Parameters.AddWithValue("@pId", chatGuid.ToByteArray());
                cmd.Parameters.AddWithValue("@name", "Личный чат");
                chatId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            string linkSql = "INSERT INTO ichatsandusers (userId, ichatId, role) VALUES (@uId, @cId, @role);";
            using (var cmd = new MySqlCommand(linkSql, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@uId", senderId);
                cmd.Parameters.AddWithValue("@cId", chatId);
                cmd.Parameters.AddWithValue("@role", 1);
                cmd.ExecuteNonQuery();
            }
            if (senderId != recipientId)
            {
                using (var cmd = new MySqlCommand(linkSql, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@uId", recipientId);
                    cmd.Parameters.AddWithValue("@cId", chatId);
                    cmd.Parameters.AddWithValue("@role", 0);
                    cmd.ExecuteNonQuery();
                }
            }
            using (var cmd = new MySqlCommand(@"
            INSERT INTO message (userId, ichatId, messagePublicId, text, dateTimeSend, isRead, isDeleted) 
            VALUES (@uId, @cId, @mPId, @text, @date, 0, 0);", connection, transaction))
            {
                cmd.Parameters.AddWithValue("@uId", senderId);
                cmd.Parameters.AddWithValue("@cId", chatId);
                cmd.Parameters.AddWithValue("@mPId", Guid.NewGuid().ToByteArray());
                cmd.Parameters.AddWithValue("@text", messageText);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
            return chatGuid;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public static List<MessageDTO> GetMessagesByChatId(MySqlConnection mySqlConnection, string chatPublicId)
    {
        Guid guid = Guid.Parse(chatPublicId);
        byte[] guidBytes = guid.ToByteArray();

        using MySqlCommand command = mySqlConnection.CreateCommand();
        command.CommandText = @"
        SELECT m.text, m.userId AS senderId, m.dateTimeSend AS messageTimestamp
        FROM message m
        JOIN ichat c ON m.ichatId = c.ichatId
        WHERE c.ichatPublicId = @chatPublicId
        ORDER BY m.dateTimeSend ASC;";

        command.Parameters.AddWithValue("@chatPublicId", guidBytes);

        using MySqlDataReader reader = command.ExecuteReader();
        List<MessageDTO> messages = new List<MessageDTO>();

        while (reader.Read())
        {
            messages.Add(new MessageDTO
            {
                Text = reader.GetString("text"),
                SenderId = reader.GetInt32("senderId"),
                Timestamp = reader.GetDateTime("messageTimestamp")
            });
        }

        return messages;
    }

    public void SendMessage(string chatPublicId, int senderId, string text)
    {
        using MySqlConnection connection = new MySqlConnection(_connectionString);
        connection.Open();
        int chatId;
        using (var cmd = new MySqlCommand(
            "SELECT ichatId FROM ichat WHERE ichatPublicId = @pId LIMIT 1;", connection))
        {
            cmd.Parameters.AddWithValue("@pId", Guid.Parse(chatPublicId).ToByteArray());
            var result = cmd.ExecuteScalar();
            if (result == null) throw new Exception("Чат не найден");
            chatId = Convert.ToInt32(result);
        }
        using (var cmd = new MySqlCommand(@"
        INSERT INTO message (userId, ichatId, messagePublicId, text, dateTimeSend, isRead, isDeleted)
        VALUES (@userId, @chatId, @mPId, @text, @date, 0, 0);", connection))
        {
            cmd.Parameters.AddWithValue("@userId", senderId);
            cmd.Parameters.AddWithValue("@chatId", chatId);
            cmd.Parameters.AddWithValue("@mPId", Guid.NewGuid().ToByteArray());
            cmd.Parameters.AddWithValue("@text", text);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.ExecuteNonQuery();
        }
    }

    public UserSearchDTO? GetChatCompanion(string chatPublicId, int currentUserId)
    {
        using MySqlConnection connection = new MySqlConnection(_connectionString);
        connection.Open();

        using var cmd = new MySqlCommand(@"
        SELECT u.userId, u.userName AS uName, u.userEmail AS uEmail
        FROM ichatsandusers icu
        JOIN ichat c ON icu.ichatId = c.ichatId
        JOIN user u ON icu.userId = u.userId
        WHERE c.ichatPublicId = @chatPublicId
          AND icu.userId != @currentUserId
        LIMIT 1;", connection);

        cmd.Parameters.AddWithValue("@chatPublicId", Guid.Parse(chatPublicId).ToByteArray());
        cmd.Parameters.AddWithValue("@currentUserId", currentUserId);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return new UserSearchDTO
        {
            UName = reader.GetString("uName"),
            UEmail = reader.GetString("uEmail")
        };
    }
}
