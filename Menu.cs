using MySqlConnector;

public class Menu
{
    private readonly Database database;

    public Menu(Database database)
    {
        this.database = database;
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("* Holiday Maker *");
            Console.WriteLine("Visa befintliga");
            Console.WriteLine("1. Kunder");
            Console.WriteLine("2. Rum");
            Console.WriteLine("0. Avsluta");
            Console.Write("Val: ");

            string choice = Console.ReadLine() ?? "";

            if (choice == "1")
                ShowCustomers();
            else if (choice == "2")
                ShowRooms();
            else if (choice == "0")
                break;
            else
                Console.WriteLine("Ogiltigt val.");
        }
    }

    private void ShowCustomers()
    {
        using MySqlConnection connection = database.OpenConnection();
        using MySqlCommand command = new MySqlCommand("SELECT id, name FROM Customers ORDER BY name;", connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine($"{reader.GetInt32("id")}. {reader.GetString("name")}");
        }
    }

    private void ShowRooms()
    {
        string sql = """
            SELECT a.name AS hotel, r.rooms_number, r.capacity, r.price_per_night
            FROM Rooms AS r
            INNER JOIN Accomodations AS a
                ON r.accomodation_id = a.id
            ORDER BY a.name, r.rooms_number;
            """;

        using MySqlConnection connection = database.OpenConnection();
        using MySqlCommand command = new MySqlCommand(sql, connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine($"{reader.GetString("hotel")} – rum {reader.GetInt32("rooms_number")}, {reader.GetInt32("capacity")} pers, {reader.GetInt32("price_per_night")} kr");
        }
    }
}