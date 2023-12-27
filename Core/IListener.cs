using System.Net;

namespace RandomApp;

public interface IListener
{
    public void Start(IPAddress ip, int port, int register, int backlog);
    public void Stop(string reasons);
}