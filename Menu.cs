public class Menu
{
    private readonly CustomerDb customerDb;
    private readonly RoomDb roomDb;
     private Customer? selectedCustomer;

    public Menu(Database database)
    {
        customerDb = new CustomerDb(database);
        roomDb = new RoomDb(database);
    }

    // Shows the main menu until the user chooses to exit.
    public void Run()
    {
        while (true)
        {
            Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("## Holiday Maker ## \n");
        Console.ResetColor();
        Console.WriteLine("* Operator mode *\n");

            if (selectedCustomer != null)
            {
                Console.WriteLine($"{selectedCustomer.Name} vald\n");
            }
            
            Console.WriteLine("1. Visa kunder");
            Console.WriteLine("2. Visa rum");
            Console.WriteLine("3. Välj kund");
            Console.WriteLine("0. Avsluta");
            Console.Write("Val: ");

            string choice = Console.ReadLine() ?? "";
            Console.WriteLine("");

            if (choice == "1")
                ShowCustomers();
            else if (choice == "2")
                ShowRooms();
            else if (choice == "3")
                SelectCustomer();
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

    private void SelectCustomer()
    {
        ShowCustomers();
        Console.Write("\nAnge kundens id: ");

        if(!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("\nOgiltigt id, skriv i en siffra");
            return;
        }
        Customer? customer = customerDb.GetById(id);

        if(customer == null)
        {
            Console.WriteLine($"\nDet finns ingen kund med angivet id: {id}");
            return;
        }

        selectedCustomer = customer;

    }
}