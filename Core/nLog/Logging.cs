using NLog;

namespace RandomApp;

public abstract class Logging
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static void Info(dynamic? input)
    {
        Logger.Info(input);
    }
    
    public static void Error(dynamic? input)
    {
        Logger.Error(input);
    }
}