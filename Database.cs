using MySqlConnector;

public class Database
{
    private readonly string connectionString;

    public Database(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task<MySqlConnection> OpenConnection()
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}