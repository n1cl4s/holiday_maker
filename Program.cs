
using MySqlConnector;

// Makes Swedish characters (å, ä, ö) display correctly in the console.
Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Ange användarnamn och lösenord för att få tillgång");
Console.Write("Andvändarnamn: ");
string user = Console.ReadLine() ?? "";
Console.Write("Lösenord: ");
string password = Console.ReadLine() ?? "";

string connectionString = $"Server=localhost;Database=holiday_maker;User ID={user};Password={password};";


try
{
    await using MySqlConnection test = new MySqlConnection(connectionString);
    await test.OpenAsync();
    Console.Clear();

}
catch (MySqlException)
{
    Console.WriteLine("Fel användarnamn eller lösenord");
    return;
}

Database database = new Database(connectionString);
Menu menu = new Menu(database);
await menu.Run();
