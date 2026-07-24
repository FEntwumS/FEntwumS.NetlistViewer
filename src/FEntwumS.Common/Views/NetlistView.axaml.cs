using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using FEntwumS.Common.ViewModels;

namespace FEntwumS.Common.Views;

public partial class NetlistView : UserControl
{
	private NetlistViewModel? _vm;
	
	public NetlistView()
	{
		InitializeComponent();

		if (DataContext is NetlistViewModel vm)
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
				_vm = e.NewValue as NetlistViewModel;
			}
		}

		base.OnPropertyChanged(e);
	}

	private void OnDataContextChanged(object? sender, EventArgs e)
	{
		if (IsInitialized)
		{
			_vm = DataContext as NetlistViewModel;
		}
		else
		{
			Initialized += delegate { OnDataContextChanged(sender, e); };
		}
	}
	
	private void Initialize(NetlistViewModel vm)
	{
		_vm = vm;
	}
	
	// For compiled bindings in code see: https://docs.avaloniaui.net/docs/fundamentals/coded-ui#compiled-bindings
	// For thread-safe accesses to UI objects via the dispatcher, see https://docs.avaloniaui.net/docs/app-development/threading#avaloniaobjectdispatcher
	private void ZoomToFitButton_OnClick(object? sender, RoutedEventArgs e)
	{
		PanningNetlistControl.ZoomToFit();
	}
}