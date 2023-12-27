namespace RandomApp;

public interface ISession
{
    public void Receive();
    public void Send(byte[] buffer);
    public void Disconnect();
    public IEnumerable<byte>? GeneratePacket(Packet packet);
}