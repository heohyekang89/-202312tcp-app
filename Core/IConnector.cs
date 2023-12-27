using System.Net;

namespace RandomApp;

public interface IConnector
{
    public Task Start(IPAddress ip, int port, int count);
    public void Stop(string reasons);
}