using System.Collections.Concurrent;

namespace RandomApp;

public class SessionManager
{
    #region Singleton
    private SessionManager() { }
    private static readonly Lazy<SessionManager> SessionInstance = new(() => new SessionManager());
    public static SessionManager Instance => SessionInstance.Value;
    #endregion

    private long _sessionId;
    private readonly object _lock = new();
    private readonly ConcurrentDictionary<long, Session> _sessions = new();
    private readonly ConcurrentDictionary<int, List<Packet>> _store = new();
    private readonly ConcurrentDictionary<int, string> _record = new();
    private readonly List<Packet> _packets = new();
    
    public int Count() => _sessions.Count;
    public Session Create()
    {
        Interlocked.Increment(ref _sessionId);
        var session = new Session() { SessionId = _sessionId };
        _sessions.TryAdd(_sessionId, session);
        return session;
    }
    public void Remove(long sessionId, Session session)
    {
        if(_sessions.TryRemove(sessionId, out var removed))
            Logging.Info($"disconnected session: {removed.Client.Client.RemoteEndPoint} | {removed.SessionId}");
        if (_sessions.IsEmpty)
        {
            _sessionId = 0;
        }
    }

    #region Client

    public void GetClientAll()
    {
        foreach (var session in _sessions)
        {
            if (session.Value.Client.Client.LocalEndPoint != null)
                Logging.Info($"id: {session.Key}, ip: {session.Value.Client.Client.LocalEndPoint}");
        }
    }
    
    public void RandomSendAll(int round, int min, int max)
    {
        foreach (var session in _sessions)
        {
            var endpoint = $"{session.Value.Client.Client.LocalEndPoint}";
            var random = new Random().Next(min, max + 1);
            var packet = new Packet(endpoint, random, round, min, max);
            var generatePacket = session.Value.GeneratePacket(packet);
            session.Value.Send(generatePacket);
        }
        Logging.Info($"========= Round[{round}] Sent All =========");
    }

    #endregion
    
    #region Server

    public void AddPacket(Packet packet)
    {
        lock (_lock)
        {
            _packets.Add(packet);
            if (_packets.Count != _sessions.Count) return;
            _store.TryAdd(packet.Round, _packets);
            GetStatisticsByRound(packet.Round);
            _packets.Clear();
        }
    }
    
    public void ResetPacket()
    {
        lock (_lock)
        {
            _store.Clear();
            _record.Clear();
            _packets.Clear();
        }
    }

    private void GetStatisticsByRound(int round)
    {
        var roundData = _store[round];
        foreach (var data in roundData)
            Logging.Info($"round: {data.Round} | boundary: {data.Min} - {data.Max} | value: {data.Random} | client: {data.EndPoint}");
        
        ViewStatistics(round, roundData);
        ViewRecords();
    }

    private void ViewStatistics(int round, IReadOnlyCollection<Packet> roundData)
    {
        var total = (from data in roundData select data).Sum(d => d.Random);
        var average = (from data in roundData select data).Average(d => d.Random);
        var dataSerialize = $"round: {round} | clients: {_sessions.Count} | total: {total} | avg: {average}";
        Logging.Info("=============================================================================================");
        Logging.Info($"Statistics) {dataSerialize}");
        _record.TryAdd(round, $"Record) {dataSerialize}");
    }

    private void ViewRecords()
    {
        Logging.Info("=============================================================================================");
        foreach (var each in _record)
            Logging.Info($"{each.Value}");
    }

    #endregion
}