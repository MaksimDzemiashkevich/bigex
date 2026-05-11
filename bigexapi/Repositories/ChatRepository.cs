using MySql.Data.MySqlClient;

namespace bigexapi.Repositories;

public static class ChatRepository
{
    public static void CreateTableichat(MySqlConnection mySqlConnection)
    {
        MySqlCommand command = mySqlConnection.CreateCommand();
        command.CommandText = @"create table if not exists ichat(
            ichatId int unsigned primary key auto_increment,
            ichatPublicId binary(16) not null,
            ichatName varchar(100),
            channelUniqueName varchar(100),
            chatType tinyint unsigned not null,
            ichatMainImage varchar(2083) not null,
            ichatDescription varchar(500))";
    }

    public static void CreateTableichatsandusers(MySqlConnection mySqlConnection)
    {
        MySqlCommand command = mySqlConnection.CreateCommand();
        command.CommandText = @"create table if not exists ichatsandusers(
            id int unsigned primary key auto_increment,
            userId int unsigned not null,
            ichatId int unsigned not null,
            role tinyint unsigned not null,
            foreign key (userId) references user(userId),
            foreign key (ichatId) references ichat(ichatId),
            unique key (ichatId, userId));";
    }

    public static int GetCountichat(MySqlConnection mySqlConnection, int id)
    {
        MySqlCommand command = mySqlConnection.CreateCommand();
        command.CommandText = @"select count(*) from ichatsandusers where userId = @userId;";
        command.Parameters.AddWithValue("@userId", id);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public static void Getichats(MySqlConnection mySqlConnection)
    {
        MySqlCommand command = mySqlConnection.CreateCommand();




        command.CommandText = @"select ichatPublicId, ichatName, channelUniqueName, chatType, ichatMainImage, ichatDescription
from ichat";
        return;
    }
}
