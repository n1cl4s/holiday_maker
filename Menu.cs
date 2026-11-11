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
    public async Task Run()
    {
        while (true)
        {
            // Clears the screen and the scroll history before the menu is shown.
            Console.Clear();
            Console.Write("\x1b[3J");

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

            // Clears the menu so only the result of the choice is shown.
            Console.Clear();
            Console.Write("\x1b[3J");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("## Holiday Maker ## \n");
            Console.ResetColor();

            // Catches database errors so the program does not crash, for example if the database is down.
            try
            {
                if (choice == "1")
                    await ShowCustomers();

                else if (choice == "2")
                    await ShowRooms();

                else if (choice == "3")
                {
                    // The user gets right back to the start menu when the customer is picked.
                    // The menu shows which one is chosen.
                    if (await SelectCustomer())
                        continue;
                }

                else if (choice == "4")
                    await RegisterCustomer();

                else if (choice == "5")
                    await SearchRooms();

                else if (choice == "6")
                    await ShowBookings();

                else if (choice == "7")
                    await CancelBooking();

                else if (choice == "8")
                    await ChangeGuests();

                else if (choice == "0")
                    break;

                else
                    Console.WriteLine("Ogiltigt val.");
            }
            catch (MySqlException)
            {
                Console.WriteLine("\nNågot gick fel i kontakten med databasen. Kontrollera att databasen är igång och försök igen.");
            }

            // Stops the screen so the user can see what happened after their choice.
            Console.Write("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey(true);
        }
    }

    private async Task ShowCustomers()
    {
        List<Customer> customers = await customerDb.GetAll();

        foreach (Customer customer in customers)
        {
            Console.WriteLine($"{customer.Id}. {customer.Name} ({customer.Email}) ");
        }
    }

    private async Task ShowRooms()
    {
        List<Room> rooms = await roomDb.GetAll();

        foreach (Room room in rooms)
        {
            Console.WriteLine($"{room.Hotel}, {room.City} - rum {room.RoomNumber}, Max antal personer: {room.Capacity}, Pris: {room.PricePerNight} kr");
        }
    }

    // Shows all bookings for the selected customer.
    private async Task ShowBookings()
    {
        if (selectedCustomer == null)
        {
            Console.WriteLine("\nVälj en kund först.");
            return;
        }

        List<Booking> bookings = await bookingDb.GetByCustomer(selectedCustomer.Id);

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
    private async Task CancelBooking()
    {
        if (selectedCustomer == null)
        {
            Console.WriteLine("\nVälj en kund först.");
            return;
        }

        await ShowBookings();

        Console.Write("\nAnge bokningsnummer på bokningen du vill avboka: ");
        if (!int.TryParse(Console.ReadLine(), out int bookingId))
        {
            Console.WriteLine("Ogiltigt bokningsnummer");
            return;
        }

        if (await bookingDb.Delete(bookingId, selectedCustomer.Id))
        {
            Console.WriteLine("Bokningen är avbokad");
        }
        else
        {
            Console.WriteLine("Kunden innehar inte denna bokning");
        }
    }

    private async Task ChangeGuests()
    {
        if (selectedCustomer == null)
        {
            Console.WriteLine("\nVälj en kund först");
            return;
        }
        
        await ShowBookings();

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

        if (await bookingDb.UpdateGuests(bookingId, selectedCustomer.Id, guests))
        {
            Console.WriteLine("Antal personer är ändrat");
        }
        else
        {
            Console.WriteLine("Ändringen nekades. Rummet rymmer inte så många, eller så tillhör bokningen inte kunden");
        }
    }

    private async Task<bool> SelectCustomer()
    {
        await ShowCustomers();
        Console.Write("\nAnge kundens id: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("\nOgiltigt id, skriv i en siffra");
            return false;
        }

        Customer? customer = await customerDb.GetById(id);

        if (customer == null)
        {
            Console.WriteLine($"\nDet finns ingen kund med angivet id: {id}");
            return false;
        }

        selectedCustomer = customer;
        return true;
    }

    private async Task RegisterCustomer()
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
            newCustomerId = await customerDb.Add(name, email, tele);
        }
        catch (MySqlException)
        {
            Console.WriteLine("E-post eller telefonnumer finns redan registrerad på en annan användare");
            return;
        }

        selectedCustomer = await customerDb.GetById(newCustomerId);
        Console.WriteLine($"\n{name} är registrerad och vald");
    }

    private async Task SearchRooms()
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

        Console.Write("Måste ha pool? (j/n): ");
        bool needPool = Console.ReadLine() == "j";

        Console.Write("Måste ha restaurang? (j/n): ");
        bool needRestaurant = Console.ReadLine() == "j";

        // Enter or invalid input gives 0, which means no limit.
        Console.Write("Max avstånd till strand i meter (Enter = inget krav): ");
        int.TryParse(Console.ReadLine(), out int maxBeach);

        Console.Write("Max avstånd till centrum i meter (Enter = inget krav): ");
        int.TryParse(Console.ReadLine(), out int maxCentrum);

        // Anything other than "o" sorts on price.
        Console.Write("Sortera på pris eller omdöme? (p/o): ");
        string sortBy = Console.ReadLine() == "o" ? "rating" : "price";

        List<Room> rooms = await roomDb.Search(city, checkIn, checkOut, guests, needPool, needRestaurant, maxBeach, maxCentrum, sortBy);

        if (rooms.Count == 0)
        {
            Console.WriteLine("\nInga lediga rum hittades");
            return;
        }

        Console.WriteLine();
        for (int i = 0; i < rooms.Count; i++)
        {
            Room room = rooms[i];
            string pool = room.Pool ? "Ja" : "Nej";
            string restaurant = room.Restaurant ? "Ja" : "Nej";

            Console.WriteLine($"{i + 1}. {room.Hotel}, {room.City} - rum {room.RoomNumber}, {room.Capacity} pers, {room.PricePerNight} kr/natt");
            Console.WriteLine($"   Omdöme: {room.Rating}/5 | Pool: {pool} | Restaurang: {restaurant} | Strand: {room.DistanceToBeach} m | Centrum: {room.DistanceToCentrum} m");
        }

        if (selectedCustomer == null)
        {
            Console.WriteLine("\nVälj en kund först för att kunna boka");
            return;
        }

        Console.Write("\nVälj rum att boka (0 för att avbryta): ");
        if (!int.TryParse(Console.ReadLine(), out int roomChoice) || roomChoice < 0 || roomChoice > rooms.Count)
        {
            Console.WriteLine("Ogiltigt val");
            return;
        }

        if (roomChoice == 0)
        {
            return;
        }

        Room selectedRoom = rooms[roomChoice - 1];

        // Check availability again right before saving.
        if (!await bookingDb.BookableRoom(selectedRoom.Id, checkIn, checkOut))
        {
            Console.WriteLine("Rummet är inte längre ledigt");
            return;
        }

        int bookingId = await bookingDb.Create(selectedCustomer.Id, selectedRoom.Id, checkIn, checkOut, guests);

        int nights = (checkOut - checkIn).Days;
        int totalPrice = nights * selectedRoom.PricePerNight;

        Console.WriteLine($"\nBokning bekräftad! Bokningsnummer: {bookingId}");
        Console.WriteLine($"{selectedRoom.Hotel}, rum {selectedRoom.RoomNumber}, {nights} nätter");
        Console.WriteLine($"Totalpris: {totalPrice} kr");
    }
}