public class Menu
{
    private readonly CustomerDb customerDb;
        private readonly RoomDb roomDb;

    public Menu(Database database)
    {
        customerDb = new CustomerDb(database);
        roomDb = new RoomDb(database);
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
        List <Customer> customers = customerDb.GetAll();

        foreach (Customer customer in customers)
        {
            Console.WriteLine($"{customer.Id}. {customer.Name} ({customer.Email}) ");
        }
    }

    private void ShowRooms()
    {
        List<Room> rooms = roomDb.GetAll();

        foreach (Room room in rooms)
        {
            Console.WriteLine($"{room.Hotel}, {room.City} - rum {room.RoomNumber}, Max antal personer: {room.Capacity}, Pris: {room.PricePerNight} kr");
        }
    }
}