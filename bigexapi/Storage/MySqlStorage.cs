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
            // 1. Создаем запись в ichat (согласно image_bd5e9e.png)
            Guid chatGuid = Guid.NewGuid();
            int chatId;
            using (var cmd = new MySqlCommand(@"
            INSERT INTO ichat (ichatPublicId, ichatName, chatType, ichatMainImage, ichatDescription) 
            VALUES (@pId, @name, 1, '', ''); 
            SELECT LAST_INSERT_ID();", connection, transaction))
            {
                cmd.Parameters.AddWithValue("@pId", chatGuid.ToByteArray());
                cmd.Parameters.AddWithValue("@name", "Личный чат"); // Можно передать имя собеседника
                chatId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // 2. Связываем пользователей с чатом в ichatsandusers
            string linkSql = "INSERT INTO ichatsandusers (userId, ichatId, role) VALUES (@uId, @cId, @role);";

            // Отправитель
            using (var cmd = new MySqlCommand(linkSql, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@uId", senderId);
                cmd.Parameters.AddWithValue("@cId", chatId);
                cmd.Parameters.AddWithValue("@role", 1);
                cmd.ExecuteNonQuery();
            }

            // Получатель (Добавляем только если это другой пользователь)
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

            // 3. Создаем первое сообщение в таблице message
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
}
