using MySqlConnector;

Console.OutputEncoding = System.Text.Encoding.UTF8;

string connectionString = "Server=localhost;Database=holiday_maker;User ID=root;Password=kurslosenord;";

using MySqlConnection connection = new MySqlConnection(connectionString);
connection.Open();
Console.WriteLine("Ansluten till databasen!");

// Kunder
Console.WriteLine();
Console.WriteLine("Kunder:");

using MySqlCommand command = new MySqlCommand("SELECT id, name FROM Customers ORDER BY name;", connection);
using MySqlDataReader reader = command.ExecuteReader();

while (reader.Read())
{
    Console.WriteLine($"{reader.GetInt32("id")}. {reader.GetString("name")}");
}
reader.Close();

// Rum
Console.WriteLine();
Console.WriteLine("Rum:");

string roomSql = """
    SELECT a.name AS hotel, r.rooms_number, r.capacity, r.price_per_night
    FROM Rooms AS r
    INNER JOIN Accomodations AS a
        ON r.accomodation_id = a.id
    ORDER BY a.name, r.rooms_number;
    """;

using MySqlCommand roomCommand = new MySqlCommand(roomSql, connection);
using MySqlDataReader roomReader = roomCommand.ExecuteReader();

while (roomReader.Read())
{
    Console.WriteLine($"{roomReader.GetString("hotel")} – rum {roomReader.GetInt32("rooms_number")}, {roomReader.GetInt32("capacity")} pers, {roomReader.GetInt32("price_per_night")} kr");
}