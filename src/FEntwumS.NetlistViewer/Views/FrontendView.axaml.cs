using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using FEntwumS.NetlistViewer.Controls;
using FEntwumS.NetlistViewer.Services;
using FEntwumS.NetlistViewer.Types;
using FEntwumS.NetlistViewer.ViewModels;
using OneWare.Essentials.Services;

namespace FEntwumS.NetlistViewer.Views;

// Avalonia expects the View corresponding to a given ViewModel to have the same name, but ending in just "View" instead
// of "ViewModel" [1]
//
// [1]: https://docs.avaloniaui.net/docs/concepts/view-locator
public partial class FrontendView : UserControl
{
	private FrontendViewModel? _vm;

	public FrontendView()
	{
		InitializeComponent();

		if (DataContext is FrontendViewModel vm)
		{
			Initialize(vm);
		}

		DataContextChanged += OnDataContextChanged;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == DataContextProperty)
		{
			if (_vm == null)
			{
				_vm = e.NewValue as FrontendViewModel;
			}
		}

		base.OnPropertyChanged(e);
	}

	private void OnDataContextChanged(object? sender, EventArgs e)
	{
		if (IsInitialized)
		{
			_vm = DataContext as FrontendViewModel;
		}
		else
		{
			Initialized += delegate { OnDataContextChanged(sender, e); };
		}
	}

	private void NetlistControl_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (sender is not NetlistControl netlistControl) return;
		if (_vm != null && netlistControl is { IsInitialized: true, FileLoaded: true } &&
		    ((AvaloniaList<NetlistElement>)netlistControl.Items).Count == 0)
		{
			ServiceManager.GetService<IDockService>().CloseDockable(_vm);
		}
	}

	private void Initialize(FrontendViewModel vm)
	{
		_vm = vm;
	}

	private void Button_OnClick(object? sender, RoutedEventArgs e)
	{
		NetlistControl? netlistControl = this.Find<NetlistControl>("NetlistView");

		netlistControl?.ZoomToFit();
	}
}