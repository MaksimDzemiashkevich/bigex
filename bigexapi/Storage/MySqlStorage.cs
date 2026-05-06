using bigexapi.Model;
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
        
        if (ChatRepository.GetCountichat(mySqlConnection) == 0)
        {
            return new List<AbstractChat>();
        }
        else
        {
            List<AbstractChat> chats = new List<AbstractChat>();


        }

    }
}
