using System.Net;

namespace RandomApp;

internal class Program
{
    private static void Main(string[] args)
    {
        var listener = new Listener();
        listener.Start(Settings.GetHost(), Settings.Instance.Config.Port, Settings.Instance.Config.Register, Settings.Instance.Config.BackLog);
        while (true) { }
    }
}