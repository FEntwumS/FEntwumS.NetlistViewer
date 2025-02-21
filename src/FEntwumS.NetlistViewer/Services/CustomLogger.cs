using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using FEntwumS.Common;
using OneWare.Essentials.Services;
using TextMateSharp.Themes;

namespace FEntwumS.NetlistViewer.Services;

public class CustomLogger : ICustomLogger
{
    private readonly ILogger _logger;

    private const ConsoleColor LogMessageConsoleColor = ConsoleColor.Cyan;
    private static readonly IBrush LogMessageBrush = (Application.Current!.GetResourceObservable("ThemeAccentBrush") as IBrush)!;

    private readonly string _assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;

    public CustomLogger()
    {
        _logger = ServiceManager.GetLogger();
    }

    public void Log(string message, bool showOutput = false)
    {
        _logger.Log("["+ _assemblyName + "]: " + message, LogMessageConsoleColor, showOutput, LogMessageBrush);
    }

    public void Error(string message, bool showOutput = true)
    {
        _logger.Error("["+ _assemblyName + "]: " + message, null, showOutput);
    }
}