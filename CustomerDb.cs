using MySqlConnector;

// Handles all SQL queries for the Customers table.
public class CustomerDb
{
    private readonly Database database;

    public CustomerDb(Database database)
    {
        this.database = database;
    }

    public async Task<List<Customer>> GetAll()
    {
        List<Customer> customers = new List<Customer>();

        await using MySqlConnection connection = await database.OpenConnection();
        using MySqlCommand command = new MySqlCommand("SELECT id, name, email, tele FROM Customers ORDER BY name", connection);
        await using MySqlDataReader reader = await command.ExecuteReaderAsync();
    
        while (await reader.ReadAsync())
        {
            Customer customer = new Customer
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Email = reader.GetString("email"),
                Tele = reader.GetString("tele")
            };
            customers.Add(customer);
        }
        return customers;
    }

   
    
     // Returns null if no customer with the given id exists.
    public async Task<Customer?> GetById(int id)
    {
        await using MySqlConnection connection = await database.OpenConnection();
        using MySqlCommand command = new MySqlCommand("SELECT id, name, email, tele FROM Customers WHERE id = @id;", connection);
        command.Parameters.AddWithValue("@id", id);
        await using MySqlDataReader reader = await command.ExecuteReaderAsync();

       
        if (!await reader.ReadAsync())
        {
            return null;
        }

            Customer customer = new Customer
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Email = reader.GetString("email"),
                Tele = reader.GetString("tele")
            };
            return customer;
    }

    public async Task<int> Add(string name, string  email, string tele)
    {
        await using MySqlConnection connection = await database.OpenConnection();
        using MySqlCommand command = new MySqlCommand
            ("INSERT INTO Customers (name, email, tele) VALUES (@name, @email, @tele);", connection);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@tele", tele);
            await command.ExecuteNonQueryAsync();

            return (int)command.LastInsertedId;
    }
}
