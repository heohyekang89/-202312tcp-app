using System.Net;
using System.Text.Json;

namespace RandomApp;

public class Settings
{
    #region Singleton
    private Settings(AppSettings config)
    {
        Config = config;
        Load();
    }
    private static readonly Lazy<Settings> SettingsInstance = new(() => new Settings(new AppSettings()));
    public static Settings Instance => SettingsInstance.Value;
    #endregion

    private const string Path = "./Settings/appsettings.json";
    public AppSettings Config { get; private set; }
    private void Load()
    {
        var readAllText = File.ReadAllText(Path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        Config = JsonSerializer.Deserialize<AppSettings>(readAllText, options) ?? throw new InvalidOperationException();
    }

    /// <summary>
    /// temp local ipaddress
    /// </summary>
    /// <returns></returns>
    public static IPAddress GetHost()
    {
        var hostName = Dns.GetHostName();
        var host = Dns.GetHostEntry(hostName);
        return host.AddressList[0];
    }
}