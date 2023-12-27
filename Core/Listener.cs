using System.Net;
using System.Net.Sockets;

namespace RandomApp;

public class Listener: IListener
{
    
    private TcpListener _tcpListener = null!;
    private readonly CancellationTokenSource _cts = new();
    public async void Start(IPAddress ip, int port, int register, int backlog)
    {
        try
        {
            _tcpListener = new TcpListener(ip, port);
            Logging.Info($"[Tcp Listener {ip}, {port} Running...]");
            _tcpListener.Start(backlog);
            do
            {
                var client = await _tcpListener.AcceptTcpClientAsync();
                var session = SessionManager.Instance.Create();
                session.Client = client;
                await Task.Run(session.Receive, _cts.Token);
                Logging.Info($"Client Connected: {session.SessionId} | {session.Client.Client.RemoteEndPoint}");
            } while (!_cts.IsCancellationRequested);
        }
        catch (Exception e)
        {
            Stop(e.Message);
        }
        
    }
    public void Stop(string reasons)
    {
        Logging.Error($"Listener: {reasons}");
        _tcpListener.Stop();
        _cts.Cancel();
    }
}