namespace RandomApp;

public class Packet
{
    public string EndPoint { get; init; }
    public int Random { get; init; }
    public int Round { get; init; }
    public int Min { get; init; }
    public int Max { get; init; }

    public Packet(string endPoint, int random, int round, int min, int max)
    {
        EndPoint = endPoint;
        Random = random;
        Round = round;
        Min = min;
        Max = max;
    }
}