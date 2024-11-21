﻿using Avalonia.Controls.Primitives;
using Avalonia.Media;
using OneWare.Essentials.Services;

namespace Oneware.NetlistReaderFrontend.Services;

public class CustomLogger : ICustomLogger
{
    private readonly ILogger _logger;

    private const ConsoleColor LogMessageConsoleColor = ConsoleColor.Cyan;
    private static readonly IBrush LogMessageBrush = new SolidColorBrush(Colors.Cyan);

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