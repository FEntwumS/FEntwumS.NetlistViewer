using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using OneWare.Essentials.Services;
using FEntwumS.Common.Services;

namespace FEntwumS.NetlistViewer.Services;

public class CustomLogger : ICustomLogger
{
    private readonly ILogger _logger;

    private const ConsoleColor LogMessageConsoleColor = ConsoleColor.Cyan;
    private static readonly IBrush LogMessageBrush = (Application.Current!.GetResourceObservable("ThemeAccentBrush") as IBrush)!;

    public CustomLogger()
    {
        _logger = ServiceManager.GetLogger();
    }

    public void Log(string message, bool showOutput = false)
    {
        string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
        
        _logger.Log("["+ assemblyName + "]: " + message, LogMessageConsoleColor, showOutput, LogMessageBrush);
    }

    public void Error(string message, bool showOutput = true)
    {
        string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
        
        _logger.Error("["+ assemblyName + "]: " + message, null, showOutput);
    }
}