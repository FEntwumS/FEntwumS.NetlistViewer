using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FEntwumS.Common.Views;

public partial class NetlistView : UserControl
{
	// For compiled bindings in code see: https://docs.avaloniaui.net/docs/fundamentals/coded-ui#compiled-bindings
	// For thread-safe accesses to UI objects via the dispatcher, see https://docs.avaloniaui.net/docs/app-development/threading#avaloniaobjectdispatcher
	private void ZoomToFitButton_OnClick(object? sender, RoutedEventArgs e)
	{
		PanningControl.ZoomToFit();
	}
}