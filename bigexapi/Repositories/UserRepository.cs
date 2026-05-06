using MySql.Data.MySqlClient;

namespace bigexapi.Repositories;

public static class UserRepository
{
    public static void SearchichatsWithUser(MySqlConnection mySqlConnection)
    {
        MySqlCommand command = mySqlConnection.CreateCommand();
        command.CommandText = @"select "
    }
}
