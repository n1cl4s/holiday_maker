using MySqlConnector;

public class CustomerDb
{
    private readonly Database database;

    public CustomerDb(Database database)
    {
        this.database = database;
    }

    public List <Customer> GetAll()
    {
        List<Customer> customers = new List<Customer>();

        using MySqlConnection connection = database.OpenConnection();
        using MySqlCommand command = new MySqlCommand("SELECT id, name, email, tele FROM Customers", connection);
        using MySqlDataReader reader = command.ExecuteReader();
    
        while (reader.Read())
        {
            Customer customer = new Customer();
            customer.Id = reader.GetInt32("id");
            customer.Name = reader.GetString("name");
            customer.Email = reader.GetString("email");
            customer.Tele = reader.GetString("tele");
            customers.Add(customer);
        }
        return customers;
    }
}
