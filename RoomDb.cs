using MySqlConnector;

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
}