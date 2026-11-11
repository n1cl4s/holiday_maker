using MySqlConnector;

// Handles all SQL queries for rooms.
public class RoomDb
{
    private readonly Database database;

    public RoomDb(Database database)
    {
        this.database = database;
    }

    public List <Room> GetAll()
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

        using MySqlConnection connection = database.OpenConnection();
        using MySqlCommand command = new MySqlCommand(sql, connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Room room = new Room();
            room.Id = reader.GetInt32("id");
            room.Hotel = reader.GetString("hotel");
            room.City = reader.GetString ("city");
            room.RoomNumber = reader.GetInt32("rooms_number");
            room.Capacity = reader.GetInt32("capacity");
            room.PricePerNight = reader.GetInt32("price_per_night");
            room.Rating = reader.GetInt32("rating");
            rooms.Add(room);
        }
        return rooms;

    }

    public List<Room> Search (string city, DateTime checkIn, DateTime checkOut, int guests)
    {
    List<Room> rooms = new List<Room>();

    string sql = """
        SELECT id, hotel, city, rooms_number, capacity, price_per_night, rating
        FROM RoomDetails
        WHERE city = @city
        AND capacity >= @guests
        AND id NOT IN
        (
        SELECT room_id
        FROM Bookings
        WHERE booking_start < @checkOut
            AND booking_end > @checkIn
        )
        ORDER BY price_per_night;   
        """;



    using MySqlConnection connection = database.OpenConnection();
    using MySqlCommand command = new MySqlCommand(sql, connection);
    command.Parameters.AddWithValue("@city", city);
    command.Parameters.AddWithValue("@guests", guests);
    command.Parameters.AddWithValue("@checkIn", checkIn);
    command.Parameters.AddWithValue("@checkOut", checkOut);
    using MySqlDataReader reader = command.ExecuteReader();

    while (reader.Read())
        {
            Room room = new Room();
            room.Id = reader.GetInt32("id");
            room.Hotel = reader.GetString("hotel");
            room.City = reader.GetString("city");
            room.RoomNumber = reader.GetInt32("rooms_number");
            room.Capacity = reader.GetInt32("capacity");
            room.PricePerNight = reader.GetInt32("price_per_night");
            room.Rating = reader.GetInt32("rating");
            rooms.Add(room);
        }
       
       return rooms; 

    }
    }