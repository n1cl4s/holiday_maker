using MySqlConnector;
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
            Console.WriteLine("4. Registrera kund");
            Console.WriteLine("5. Sök lediga rum");
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

            else if (choice == "4")
                RegisterCustomer();

            else if (choice == "5")
                SearchRooms();

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

    private void RegisterCustomer()
    {
        Console.Write("Namn: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("E-mail: ");
        string email = Console.ReadLine() ?? "";

        Console.Write("Telefonnummer: ");
        string tele = Console.ReadLine() ?? "";

        if (name == "" || email == "" || tele == "")
        {
            Console.WriteLine("Du missade att fylla i alla fält");
            return;
        }

        // The database does not allow two customers with the same email or phone.
        // The try/catch takes care of it if it happens

        int newCustomerId;

        try
        {
            newCustomerId = customerDb.Add(name, email, tele);
        }
        catch (MySqlException)
        {
            Console.WriteLine("E-post eller telefonnumer finns redan registrerad på en annan användare");
            return;
        }

        selectedCustomer = customerDb.GetById(newCustomerId);
        Console.WriteLine($"\n{name} är registrerad och vald");


    }

    private void SearchRooms()
    {
        Console.Write("Ort: ");
        string city = Console.ReadLine() ?? "";

        Console.Write("Incheckning (ÅÅÅÅ-MM-DD): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime checkIn))
        {
            Console.WriteLine("Ogiltigt datum");
            return;
        }

        Console.Write("Utcheckning (ÅÅÅÅ-MM-DD): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime checkOut))
        {
            Console.WriteLine("Ogiltigt datum");
            return;
        }

        if (checkOut <= checkIn)
        {
            Console.WriteLine("Datum för utcheckning måste ligga efter incheckning");
            return;
        }

        if (checkIn < DateTime.Today)
        {
            Console.WriteLine("Incheckningsdatum måste vara framåt i tiden");
            return;
        }

        Console.Write("Antal personer: ");
        if (!int.TryParse(Console.ReadLine(), out int guests) || guests < 1)
        {
            Console.WriteLine("Ogiltigt antal personer");
            return;
        }

        List<Room> rooms = roomDb.Search(city, checkIn, checkOut, guests);

        if (rooms.Count == 0)
        {
            Console.WriteLine("\nInga lediga rum hittades");
            return;
        }

        Console.WriteLine();
        for (int i = 0; i < rooms.Count; i++)
        {
            Room room = rooms[i];
            Console.WriteLine($"{i + 1}. {room.Hotel}, {room.City} - rum {room.RoomNumber}, {room.Capacity} pers, {room.PricePerNight} kr/natt");
        }

    }
}