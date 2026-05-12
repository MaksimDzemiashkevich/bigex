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
        command.CommandText = @"select * from ichat 
            join ichatsandusers on ichatsandusers.ichatId = ichat.ichatId
            where userId = @id;";
        command.Parameters.AddWithValue("@id", id);
        using MySqlDataReader reader = command.ExecuteReader();

        List<AbstractChat> chatsWithUser = new List<AbstractChat>();
        while (reader.Read())
        {
            byte typ = reader.GetByte("chatType");
            ChatType type = (ChatType)typ;

            AbstractChat chat = type switch
            {
                ChatType.Group => new Group(),
                ChatType.PrivateChat => new Chat(),
                ChatType.Channel => new Channel()
            };

            byte[] guidBytes = (byte[])reader["ichatPublicId"];
            Guid userPublicId = new Guid(guidBytes);

            chat.IchatPublicId = userPublicId;
            chat.IchatName = reader.GetString("ichatName");
            chat.ChannelUniqueName = reader.GetString("channelUniqueName");


            chat.IchatMainImage = reader.GetString("ichatMainImage");
            chat.IchatDescription = reader.GetString("ichatDescription");
            chatsWithUser.Add(chat);
        }

        return chatsWithUser;
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
        command.Parameters.AddWithValue("@uPublicId", guid);
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

}
