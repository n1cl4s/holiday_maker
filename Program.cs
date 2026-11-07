using MySqlConnector;

string connectionString = "Server=localhost;Database=holiday_maker;User ID=root;Password=kurslosenord;";

using MySqlConnection connection = new MySqlConnection(connectionString);

connection.Open();

using MySqlCommand command = new MySqlCommand("SELECT id, name FROM Customers ORDER BY name;", connection);

using MySqlDataReader reader = command.ExecuteReader();


while (reader.Read())
{
    Console.WriteLine($"{reader.GetInt32("id")}. {reader.GetString("name")}");
}


