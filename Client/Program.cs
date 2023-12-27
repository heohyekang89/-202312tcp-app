using System.Net;
using System.Text.RegularExpressions;
using static System.Int32;

namespace RandomApp;

internal abstract class Program
{
    private static int _round;
    private const char Separator = '-';
    private static readonly CancellationTokenSource Cts = new();
    private static readonly Regex BoundaryRegex = new(@"^\d+-\d+$");

    public static async Task Main(string[] args)
    {
        do
        {
            // Enter the number of clients
            Logging.Info("[ 1. Enter the number of clients ]");
            var clients = Console.ReadLine();
            var tryParse = TryParse(clients, out var count);
            if (clients == null || !tryParse) continue;

            // Connect client
            Logging.Info($"[Request {clients} client connections]");
            var connector = new Connector();
            await connector.Start(Settings.GetHost(), Settings.Instance.Config.Port, count);
            
            // Input Random Range
            do
            {
                Logging.Info($"[ 2. Input Random Range | ex) 1-100 ]");
                var randomBoundary = Console.ReadLine();
                if (!ValidatorRandomBoundary(randomBoundary, out var min, out var max)) continue;
                Interlocked.Increment(ref _round);
                Logging.Info($"[Process Round: {_round}, Input Boundary: {randomBoundary}]");
                SessionManager.Instance.RandomSendAll(_round, min, max);
            } while (!Cts.IsCancellationRequested);
        } while (!Cts.IsCancellationRequested);
    }

    /// <summary>
    /// Check Boundary Regex
    /// </summary>
    /// <param name="randomBoundary"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private static bool ValidatorRandomBoundary(string? randomBoundary, out int min, out int max)
    {
        if(randomBoundary == null || !BoundaryRegex.Match(randomBoundary).Success)
        {
            min = 0;
            max = 0;
            Logging.Error("Please fill in the form | ex) 1-100");
            return false;
        }
        var boundary = randomBoundary.Split(Separator);
        min = Parse(boundary[0]);
        max = Parse(boundary[1]);
        if (min < max) return true;
        Logging.Error($"Max({max}) must be greater than Min({min})");
        return false;
    }
    
}