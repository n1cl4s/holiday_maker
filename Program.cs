
// Makes Swedish characters (å, ä, ö) display correctly in the console.
Console.OutputEncoding = System.Text.Encoding.UTF8;

// Connection to the database
string connectionString = "Server=localhost;Database=holiday_maker;User ID=root;Password=hidden;";

Database database = new Database(connectionString);
Menu menu = new Menu(database);
menu.Run();