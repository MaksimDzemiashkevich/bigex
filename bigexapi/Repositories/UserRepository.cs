using bigexapi.Model;
using bigexapi.Model.Enum;
using bigexapi.ModelDTO;
using MySql.Data.MySqlClient;

namespace bigexapi.Repositories;

public static class UserRepository
{
    public static List<AbstractChat> SearchichatsWithUser(MySqlConnection mySqlConnection, int id)
    {
        using MySqlCommand command = mySqlConnection.CreateCommand();
        command.CommandText = @"
    SELECT c.*,
           companion.userName AS companionName
    FROM ichat c
    JOIN ichatsandusers iu ON iu.ichatId = c.ichatId
    LEFT JOIN ichatsandusers iu2 ON iu2.ichatId = c.ichatId AND iu2.userId != @id
    LEFT JOIN user companion ON companion.userId = iu2.userId
    WHERE iu.userId = @id;";

        command.Parameters.AddWithValue("@id", id);
        using MySqlDataReader reader = command.ExecuteReader();

        List<AbstractChat> chatsWithUser = new List<AbstractChat>();
        while (reader.Read())
        {
            byte typ = reader.IsDBNull(reader.GetOrdinal("chatType"))
                       ? (byte)1
                       : reader.GetByte("chatType");

            ChatType type = (ChatType)typ;

            AbstractChat chat = type switch
            {
                ChatType.Group => new Group(),
                ChatType.PrivateChat => new bigexapi.Model.Chat(),
                ChatType.Channel => new Channel(),
                _ => new bigexapi.Model.Chat()
            };

            if (!reader.IsDBNull(reader.GetOrdinal("ichatPublicId")))
            {
                byte[] guidBytes = (byte[])reader["ichatPublicId"];
                chat.IchatPublicId = new Guid(guidBytes);
            }

            chat.IchatName = reader["ichatName"] as string ?? "Без названия";
            chat.IchatMainImage = reader["ichatMainImage"] as string ?? "";

            chat.ChannelUniqueName = reader["channelUniqueName"] as string;
            chat.IchatDescription = reader["ichatDescription"] as string;
            chat.CompanionName = reader["companionName"] as string;

            chatsWithUser.Add(chat);
        }

        return chatsWithUser;
    }

    public static bool CheckExistingUser(MySqlConnection mySqlConnection, CreateUserData createUserData)
    {
        using MySqlCommand command = mySqlConnection.CreateCommand();
        command.CommandText = @"select userId from user where userEmail=@uEmail";
        command.Parameters.AddWithValue("@uEmail", createUserData.UEmail);
        using MySqlDataReader reader = command.ExecuteReader();
        while(reader.Read())
        {
            return true;
        }
        return false;
    }

    public static void CreateTableUser(MySqlConnection mySqlConnection)
    {
        using MySqlCommand command = mySqlConnection.CreateCommand();
        command.CommandText = @"create table if not exists user(
            userId int unsigned primary key auto_increment,
            userPublicId binary(16) not null,
            userName varchar(100) not null,
            userUniqueName varchar(100) not null,
            userDescription varchar(500),
            userMainImageUrl varchar(2083),
            userPhone varchar(50),
            userEmail varchar(200) not null,
            userPassword char(64) not null,
            unique index index_user_email(userEmail),
            unique index index_user_unique_name(userUniqueName));";
        command.ExecuteNonQuery();
    }

    public static bool InsertUser(MySqlConnection mySqlConnection, CreateUserData createUserData)
    {
        Guid guid = Guid.NewGuid();

        using MySqlCommand command = mySqlConnection.CreateCommand();
        command.CommandText = @"insert into user(userPublicId, userName, userUniqueName, userEmail, userPassword) values(
            @uPublicId, @uName, @uName, @uEmail, @uPassword);";
        command.Parameters.AddWithValue("@uName", createUserData.UName);
        command.Parameters.AddWithValue("@uEmail", createUserData.UEmail);
        command.Parameters.AddWithValue("@uPassword", createUserData.UPassword);
        command.Parameters.AddWithValue("@uPublicId", guid.ToByteArray());
        int result = command.ExecuteNonQuery();
        return result > 0;
    }

    public static int CheckUser(MySqlConnection mySqlConnection, UserAuth userAuth) 
    {
        using MySqlCommand command = mySqlConnection.CreateCommand();
        command.CommandText = @"select userId, userEmail, userPassword from user
            where @uEmail = userEmail;";
        command.Parameters.AddWithValue("@uEmail", userAuth.UEmail);
        using MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            if (reader.GetString("userPassword") == userAuth.UPassword)
            {
                return reader.GetInt32("userId");
            }
        }
        return -1;
    }

    public static List<UserSearchDTO> SearchUsers(MySqlConnection mySqlConnection, string query)
    {
        using MySqlCommand command = mySqlConnection.CreateCommand();
        command.CommandText = @"SELECT userName, userEmail, userPublicId FROM user 
                            WHERE userName LIKE @query OR userEmail LIKE @query 
                            LIMIT 20;";
        command.Parameters.AddWithValue("@query", $"%{query}%");

        using MySqlDataReader reader = command.ExecuteReader();
        List<UserSearchDTO> foundUsers = new List<UserSearchDTO>();

        while (reader.Read())
        {
            foundUsers.Add(new UserSearchDTO
            {
                UName = reader.GetString("userName"),
                UEmail = reader.GetString("userEmail")
            });
        }
        return foundUsers;
    }

}
