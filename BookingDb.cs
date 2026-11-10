using MySqlConnector;

public class BookingDb
{
    private readonly Database database;

    public BookingDb(Database database)
    {
        this.database = database;
    }

    public bool BookableRoom(int roomId, DateTime checkIn, DateTime checkOut)
    {
        string sql = """
        SELECT COUNT(*)
        FROM Bookings
        WHERE  room_id = @roomId
        AND booking_start < @checkOut
        AND booking_end > @checkIn;
        """;

        using MySqlConnection connection = database.OpenConnection();
        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@roomId", roomId);
        command.Parameters.AddWithValue("@checkIn", checkIn);
        command.Parameters.AddWithValue("@checkOut", checkOut);

        int overlaps = Convert.ToInt32(command.ExecuteScalar());
        return overlaps == 0;

    }


    public int Create(int customerId, int roomId, DateTime checkIn, DateTime checkOut, int guests)

    {
        string sql = """
        INSERT INTO Bookings (customer_id, room_id, booking_start, booking_end, guests_count)
        VALUES (@customerId, @roomId, @checkIn, @checkOut, @guests);
        """;


        using MySqlConnection connection = database.OpenConnection();
        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@customerId", customerId);
        command.Parameters.AddWithValue("@roomId", roomId);
        command.Parameters.AddWithValue("@checkIn", checkIn);
        command.Parameters.AddWithValue("@checkOut", checkOut);
        command.Parameters.AddWithValue("@guests", guests);
        command.ExecuteNonQuery();

        return (int)command.LastInsertedId;
    }

    

    public List<Booking> GetByCustomer(int customerId)
    {
        string sql = """
        SELECT b.id, a.name AS hotel, r.rooms_number, b.booking_start, b.booking_end, b.guests_count,
               DATEDIFF(b.booking_end, b.booking_start) * r.price_per_night AS total_price
        FROM Bookings b
        JOIN Rooms r ON r.id = b.room_id
        JOIN Accomodations a ON a.id = r.accomodation_id
        WHERE b.customer_id = @customerId
        ORDER BY b.booking_Start;
        """;

        List<Booking> bookings = new List<Booking>();

        using MySqlConnection connection = database.OpenConnection();
        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@customerId", customerId);

        using MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            Booking booking = new Booking();
            booking.Id = reader.GetInt32("id");
            booking.Hotel = reader.GetString("hotel");
            booking.RoomNumber = reader.GetInt32("rooms_number");
            booking.CheckIn = reader.GetDateTime("booking_start");
            booking.CheckOut = reader.GetDateTime("booking_end");
            booking.Guests = reader.GetInt32("guests_count");
            booking.TotalPrice = reader.GetInt32("total_price");
            bookings.Add(booking);

        }
        return bookings;
    }

}