Console.OutputEncoding = System.Text.Encoding.UTF8;

string connectionString = "Server=localhost;Database=holiday_maker;User ID=root;Password=kurslosenord;";


Database database = new Database(connectionString);
Menu menu = new Menu(database);
menu.Run();