using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Microsoft.Extensions.Logging;
using OneWare.Essentials.Services;
using TextMateSharp.Themes;
using LoggerExtensions = OneWare.Essentials.Services.LoggerExtensions;

namespace FEntwumS.NetlistViewer.Services;

public class CustomLogger : ICustomLogger
{
	private const ConsoleColor LogMessageConsoleColor = ConsoleColor.Cyan;

	private static readonly IBrush LogMessageBrush =
		(Application.Current!.GetResourceObservable("ThemeAccentBrush") as IBrush)!;

	public void Log(string message, bool showOutput = false)
	{
		LoggerExtensions.Log(ContainerLocator.Current.Resolve<ILogger>(), message, showOutput, LogMessageBrush);
	}

	public void Error(string message, bool showOutput = true)
	{
		LoggerExtensions.Error(ContainerLocator.Current.Resolve<ILogger>(), message, null, showOutput);
	}
}