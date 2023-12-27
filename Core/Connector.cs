using System.Net;
using System.Net.Sockets;

namespace RandomApp;

public class Connector: IConnector
{
    private static readonly TimeSpan ConnectTimeOut = TimeSpan.FromSeconds(Settings.Instance.Config.ConnectTimeOutSeconds);
    private readonly CancellationTokenSource _cts = new(ConnectTimeOut);
    public async Task Start(IPAddress ip, int port, int count)
    {
        try
        {
            for (var i = 0; i < count; i++)
            {
                TcpClient client = new();
                await client.ConnectAsync(ip, port, _cts.Token);
                var session = SessionManager.Instance.Create();
                session.Client = client;
                
                if (count == SessionManager.Instance.Count())
                    SessionManager.Instance.GetClientAll();
            }
        }
        catch (Exception e)
        {
            Stop(e.Message);
        }
    }
    public void Stop(string reasons)
    {
        _cts.Cancel();
        Logging.Error($"Connector Connect: {reasons}");
        Environment.Exit(-1);
    }
}