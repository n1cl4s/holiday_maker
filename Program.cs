using MySqlConnector;

string connectionString = "Server=localhost;Database=holiday_maker;User ID=root;Password=kurslosenord;";

using MySqlConnection connection = new MySqlConnection(connectionString);

connection.Open();

// Is the database connected?
// Yes it was
Console.WriteLine("Ansluten till databasen!");

// How many customers is it in the database? Should be 5
// It was 5. Great!
using MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM Customers;", connection);
object? result = command.ExecuteScalar();
Console.WriteLine($"Antal kunder: {result}");