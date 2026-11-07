using MySqlConnector;

public class Database
{
    private readonly string connectionString;

    public Database(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public MySqlConnection OpenConnection()
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}