using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System;

namespace RandomApp;

public class Session: ISession
{
    public TcpClient Client { get; set; } = null!;
    public long SessionId;
    private readonly CancellationTokenSource _cts = new();

    public async void Receive()
    {
        try
        {
            while (!_cts.IsCancellationRequested)
            {
                var readBuffer = new byte[Settings.Instance.Config.ReadBufferSize];
                var networkStream = Client.GetStream();
                var nBytes = await networkStream.ReadAsync(readBuffer.AsMemory(0, readBuffer.Length));
                var read = Encoding.UTF8.GetString(readBuffer, 0, nBytes);
                var packet = JsonSerializer.Deserialize<Packet>(read);
                if (packet != null) SessionManager.Instance.AddPacket(packet);
            }
        }
        catch (Exception)
        {
            SessionManager.Instance.Remove(SessionId, this);
            SessionManager.Instance.ResetPacket();
            _cts.Cancel();
        }
    }

    public async void Send(byte[] writeBuffer)
    {
        try
        {
            var networkStream = Client.GetStream();
            await networkStream.WriteAsync(writeBuffer);
        }
        catch (Exception)
        {
            Logging.Error("Session Send: Server Disconnected");
            Environment.Exit(-1);
        }
    }

    public byte[] GeneratePacket(Packet packet)
    {
        try
        {
            var packetToSerialize = JsonSerializer.Serialize(packet);
            return Encoding.UTF8.GetBytes(packetToSerialize);
        }
        catch (Exception e)
        {
            Logging.Error($"Session GeneratePacket: {e.Message}");
            Disconnect();
        }

        throw new Exception("Generate Packet Error");
    }

    public void Disconnect()
    {
        Client.GetStream().Close();
        Client.Client.Close();
    }

    IEnumerable<byte>? ISession.GeneratePacket(Packet packet)
    {
        return GeneratePacket(packet);
    }
}