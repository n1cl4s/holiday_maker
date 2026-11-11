using MySqlConnector;

public class Menu
{
    private readonly CustomerDb customerDb;
    private readonly RoomDb roomDb;
    private readonly BookingDb bookingDb;
    private Customer? selectedCustomer;

    public Menu(Database database)
    {
        customerDb = new CustomerDb(database);
        roomDb = new RoomDb(database);
        bookingDb = new BookingDb(database);
    }

    // Shows the main menu until the user chooses to exit.
    public void Run()
    {

        Console.Clear();
        while (true)
        {

            
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
            Console.WriteLine("5. Sök och boka rum");
            Console.WriteLine("6. Visa kundens bokningar");
            Console.WriteLine("7. Avboka");
            Console.WriteLine("8. Ändra antal personer");
            Console.WriteLine("0. Avsluta");
            Console.Write("Val: ");

            string choice = Console.ReadLine() ?? "";
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("## Holiday Maker ## \n");
            Console.ResetColor();

            if (choice == "1")
                ShowCustomers();

            else if (choice == "2")
                ShowRooms();

            else if (choice == "3")

            // The user gets right back to the start menu when the customer is picked
            // The menu shows which one i chosen
            {        
                if (SelectCustomer())
                continue;
            }

            else if (choice == "4")
                RegisterCustomer();

            else if (choice == "5")
                SearchRooms();

            else if (choice == "6")
                ShowBookings();

            else if (choice == "7")
                CancelBooking();

            else if (choice == "8")
                ChangeGuests();

            else if (choice == "0")
                break;
                

            else
                Console.WriteLine("Ogiltigt val.");

                // Stops the screen so the user can see what happened after their choice
                Console.ReadKey();
        }
    }

    private void ShowCustomers()
    {
        List<Customer> customers = customerDb.GetAll();

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

    // Shows all bookings for the selected customer.
    private void ShowBookings()
    {
        if (selectedCustomer == null)
        {
            Console.WriteLine("\nVälj en kund först.");
            return;
        }

        List<Booking> bookings = bookingDb.GetByCustomer(selectedCustomer.Id);

        if (bookings.Count == 0)
        {
            Console.WriteLine("\nKunden har inga bokningar");
            return;
        }

        Console.WriteLine($"\nBokningar för {selectedCustomer.Name}:");
        foreach (Booking booking in bookings)
        {
            Console.WriteLine($"#{booking.Id} {booking.Hotel}, rum {booking.RoomNumber}, {booking.CheckIn:yyyy-MM-dd} till {booking.CheckOut:yyyy-MM-dd}, {booking.Guests} pers, {booking.TotalPrice} kr");
        }
    }

    // Cancels one of the selected customer's bookings.
    private void CancelBooking()
    {
        if (selectedCustomer == null)
        {
            Console.WriteLine("\nVälj en kund först.");
            return;
        }

        ShowBookings();

        Console.Write("\nAnge bokningsnummer på bokningen du vill avboka: ");
        int bookingId = int.Parse(Console.ReadLine()!);

        if (bookingDb.Delete(bookingId, selectedCustomer.Id))
        {
            Console.WriteLine("Bokningen är avbokad");
        }
        else
        {
            Console.WriteLine("Kunden innehar inte denna bokning");
        }
    }

    private void ChangeGuests()
    {
        if (selectedCustomer == null)
        {
            Console.WriteLine("\nVälj en kund först");
            return;
        }
        
        ShowBookings();

        Console.Write("\nAnge bokningsnummer: ");
        if (!int.TryParse(Console.ReadLine(), out int bookingId))
        {
            Console.WriteLine("Ogitigt bokningsnummer");
            return;
        }

        Console.Write("Nytt antal personer: ");
        if (!int.TryParse(Console.ReadLine(), out int guests) || guests < 1)
        {
            Console.WriteLine("Ogiltigt antal personer");
            return;
        }

        if (bookingDb.UpdateGuests(bookingId, selectedCustomer.Id, guests))
        {
            Console.WriteLine("Antal personer är ändrat");
        }
        else
        {
            Console.WriteLine("Ändringen nekades. Rummet rymmer inte så många, eller så tillhör bokningen inte kunden");
        }
    }

    private bool SelectCustomer()
    {
        ShowCustomers();
        Console.Write("\nAnge kundens id: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("\nOgiltigt id, skriv i en siffra");
            return false;
        }

        Customer? customer = customerDb.GetById(id);

        if (customer == null)
        {
            Console.WriteLine($"\nDet finns ingen kund med angivet id: {id}");
            return false;
        }

        selectedCustomer = customer;
        return true;
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

        if (!email.Contains("@"))
        {
            Console.WriteLine("E-mail måste innehålla @");
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

        if (selectedCustomer == null)
        {
            Console.WriteLine("\nVälj en kund först för att kunna boka");
            return;
        }

        Console.Write("\nVälj rum att boka: ");
        int roomChoice = int.Parse(Console.ReadLine()!);

        Room selectedRoom = rooms[roomChoice - 1];

        // Check availability again right before saving.
        if (!bookingDb.BookableRoom(selectedRoom.Id, checkIn, checkOut))
        {
            Console.WriteLine("Rummet är inte längre ledigt");
            return;
        }

        int bookingId = bookingDb.Create(selectedCustomer.Id, selectedRoom.Id, checkIn, checkOut, guests);

        int nights = (checkOut - checkIn).Days;
        int totalPrice = nights * selectedRoom.PricePerNight;

        Console.WriteLine($"\nBokning bekräftad! Bokningsnummer: {bookingId}");
        Console.WriteLine($"{selectedRoom.Hotel}, rum {selectedRoom.RoomNumber}, {nights} nätter");
        Console.WriteLine($"Totalpris: {totalPrice} kr");
    }
}