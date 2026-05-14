using MySql.Data.MySqlClient;

namespace bigexapi.Repositories;

public static class MessageRepository
{
    public static void CreateMessageTable(MySqlConnection mySqlConnection)
    {
        MySqlCommand command = mySqlConnection.CreateCommand();
        command.CommandText = @"CREATE TABLE IF NOT EXISTS message (
    messageId INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
    userId INT UNSIGNED NOT NULL,
    ichatId INT UNSIGNED NOT NULL,
    messagePublicId BINARY(16) NOT NULL,
    text TEXT,
    dateTimeSend DATETIME NOT NULL,
    isRead BOOLEAN DEFAULT FALSE,
    isDeleted BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (userId) REFERENCES user(userId),
    FOREIGN KEY (ichatId) REFERENCES ichat(ichatId)
);";
        command.ExecuteNonQuery();
    }
}
