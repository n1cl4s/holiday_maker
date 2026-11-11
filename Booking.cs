public record Booking
{
    public int Id { get; init; }
    public string Hotel { get; init; } = "";
    public int RoomNumber { get; init; }
    public DateTime CheckIn { get; init; }
    public DateTime CheckOut { get; init; }
    public int Guests { get; init; }
    public int TotalPrice { get; init; }
}