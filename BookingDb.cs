using MySqlConnector;

public class BookingDb
{
    private readonly Database database;

    public BookingDb(Database database)
    {
        this.database = database;
    }

    public async Task<bool> BookableRoom(int roomId, DateTime checkIn, DateTime checkOut)
    {
        string sql = """
        SELECT COUNT(*)
        FROM Bookings
        WHERE  room_id = @roomId
        AND booking_start < @checkOut
        AND booking_end > @checkIn;
        """;

        await using MySqlConnection connection = await database.OpenConnection();
        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@roomId", roomId);
        command.Parameters.AddWithValue("@checkIn", checkIn);
        command.Parameters.AddWithValue("@checkOut", checkOut);

        int overlaps = Convert.ToInt32(await command.ExecuteScalarAsync());
        return overlaps == 0;

    }


    public async Task<int> Create(int customerId, int roomId, DateTime checkIn, DateTime checkOut, int guests)

    {
        string sql = """
        INSERT INTO Bookings (customer_id, room_id, booking_start, booking_end, guests_count)
        VALUES (@customerId, @roomId, @checkIn, @checkOut, @guests);
        """;


        await using MySqlConnection connection = await database.OpenConnection();
        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@customerId", customerId);
        command.Parameters.AddWithValue("@roomId", roomId);
        command.Parameters.AddWithValue("@checkIn", checkIn);
        command.Parameters.AddWithValue("@checkOut", checkOut);
        command.Parameters.AddWithValue("@guests", guests);
        await command.ExecuteNonQueryAsync();

        return (int)command.LastInsertedId;
    }

    

    public async Task<List<Booking>> GetByCustomer(int customerId)
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

        await using MySqlConnection connection = await database.OpenConnection();
        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@customerId", customerId);

        await using MySqlDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
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

    public async Task<bool> Delete(int bookingId, int customerId)
    {
        string sql = """
        DELETE FROM Bookings
        WHERE id = @bookingId
        AND customer_id = @customerId;
        """;

        await using MySqlConnection connection = await database.OpenConnection();
        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@bookingId", bookingId);
        command.Parameters.AddWithValue("@customerId", customerId);

        int rowsDeleted = await command.ExecuteNonQueryAsync();
        return rowsDeleted == 1;
    }

    public async Task<bool> UpdateGuests(int bookingId, int customerId, int guests)
    {
        string sql = """
        UPDATE Bookings b
        JOIN Rooms r ON r.id = b.room_id
        SET b.guests_count = @guests
        WHERE b.id = @bookingId
        AND b.customer_id = @customerId
        AND r.capacity >= @guests;
        """;

        await using MySqlConnection connection = await database.OpenConnection();
        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@guests", guests);
        command.Parameters.AddWithValue("@bookingId", bookingId);
        command.Parameters.AddWithValue("@customerId", customerId);

        int rowsUpdated = await command.ExecuteNonQueryAsync();
        return rowsUpdated == 1;

    }

}