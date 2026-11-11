using MySqlConnector;

// Handles all SQL queries for rooms.
public class RoomDb
{
    private readonly Database database;

    public RoomDb(Database database)
    {
        this.database = database;
    }

    public async Task<List<Room>> GetAll()
    {
        List<Room> rooms = new List<Room>();

        // Joins Accomodations and Cities to get hotel name and city for each room.
        // Aliases inspired by the book "Prata med data"

        string sql = """
        SELECT r.id, a.name AS hotel, c.name AS city, r.rooms_number, r.capacity, r.price_per_night, a.rating
        FROM Rooms AS r
        INNER JOIN Accomodations AS a
        ON r.accomodation_id = a.id
        INNER JOIN Cities AS c
        ON a.city_id = c.id
        ORDER BY a.name, r.rooms_number;
        """;

        await using MySqlConnection connection = await database.OpenConnection();
        using MySqlCommand command = new MySqlCommand(sql, connection);
        await using MySqlDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Room room = new Room
            {
                Id = reader.GetInt32("id"),
                Hotel = reader.GetString("hotel"),
                City = reader.GetString("city"),
                RoomNumber = reader.GetInt32("rooms_number"),
                Capacity = reader.GetInt32("capacity"),
                PricePerNight = reader.GetInt32("price_per_night"),
                Rating = reader.GetInt32("rating")
            };
            rooms.Add(room);
        }

        return rooms;
    }

    public async Task<List<Room>> Search(string city, DateTime checkIn, DateTime checkOut, int guests,
        bool hasPool, bool hasRestaurant, int maxBeach, int maxCentrum, string sortBy)
    {
        List<Room> rooms = new List<Room>();

        // Sorts on rating (highest first) when sortBy is 'rating', otherwise on price (lowest first).
        // Price is also used to order rooms with the same rating.
        string sql = """
        SELECT id, hotel, city, rooms_number, capacity, price_per_night, rating,
               pool, restaurant, distance_to_beach, distance_to_centrum
        FROM RoomDetails
        WHERE city = @city
        AND capacity >= @guests
        AND (@hasPool = 0 OR pool = 1)
        AND (@hasRestaurant = 0 OR restaurant = 1)
        AND (@maxBeach = 0 OR distance_to_beach <= @maxBeach)
        AND (@maxCentrum = 0 OR distance_to_centrum <= @maxCentrum)
        AND id NOT IN
        (
        SELECT room_id
        FROM Bookings
        WHERE booking_start < @checkOut
            AND booking_end > @checkIn
        )
        ORDER BY CASE WHEN @sortBy = 'rating' THEN rating END DESC, price_per_night;
        """;

        await using MySqlConnection connection = await database.OpenConnection();
        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@city", city);
        command.Parameters.AddWithValue("@guests", guests);
        command.Parameters.AddWithValue("@checkIn", checkIn);
        command.Parameters.AddWithValue("@checkOut", checkOut);
        command.Parameters.AddWithValue("@hasPool", hasPool);
        command.Parameters.AddWithValue("@hasRestaurant", hasRestaurant);
        command.Parameters.AddWithValue("@maxBeach", maxBeach);
        command.Parameters.AddWithValue("@maxCentrum", maxCentrum);
        command.Parameters.AddWithValue("@sortBy", sortBy);
        await using MySqlDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Room room = new Room
            {
                Id = reader.GetInt32("id"),
                Hotel = reader.GetString("hotel"),
                City = reader.GetString("city"),
                RoomNumber = reader.GetInt32("rooms_number"),
                Capacity = reader.GetInt32("capacity"),
                PricePerNight = reader.GetInt32("price_per_night"),
                Rating = reader.GetInt32("rating"),
                Pool = reader.GetBoolean("pool"),
                Restaurant = reader.GetBoolean("restaurant"),
                DistanceToBeach = reader.GetInt32("distance_to_beach"),
                DistanceToCentrum = reader.GetInt32("distance_to_centrum")
            };
            rooms.Add(room);
        }

        return rooms;
    }
}