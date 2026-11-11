public record Room
{
    public int Id { get; init; }
    public string Hotel { get; init; } = "";
    public string City { get; init; } = "";
    public int RoomNumber { get; init; }
    public int Capacity { get; init; }
    public int Rating { get; init; }
    public int PricePerNight { get; init; }
    public bool Pool { get; init; }
    public bool Restaurant { get; init; }
    public int DistanceToBeach { get; init; }
    public int DistanceToCentrum { get; init; }
}