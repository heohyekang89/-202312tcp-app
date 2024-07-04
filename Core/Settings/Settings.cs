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

    public AppSettings Config { get; private set; }
    private void Load()
    {
        string strAppConfigFilename = "appsettings.json";
        string strAppConfigFilePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        FileInfo settingsFile = new(Path.Combine(strAppConfigFilePath, "Settings", strAppConfigFilename));
        Console.WriteLine(settingsFile);

        var readAllText = File.ReadAllText(settingsFile.FullName);
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