using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using FEntwumS.Common.Controls;
using FEntwumS.Common.Services;
using FEntwumS.Common.ViewModels;
using Microsoft.Extensions.Logging;
using OneWare.Essentials.Helpers;
using OneWare.Essentials.Services;

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
		PanningNetlistControl.Child = _vm.RootNode;
	}
	
	// For compiled bindings in code see: https://docs.avaloniaui.net/docs/fundamentals/coded-ui#compiled-bindings
	// For thread-safe accesses to UI objects via the dispatcher, see https://docs.avaloniaui.net/docs/app-development/threading#avaloniaobjectdispatcher
	private void ZoomToFitButton_OnClick(object? sender, RoutedEventArgs e)
	{
		PanningNetlistControl.ZoomToFit();
	}

	private void ExportToSVGButton_OnClick(object? sender, RoutedEventArgs e)
	{
		_ = ExportToSVGAsync();
	}

	private async Task ExportToSVGAsync()
	{
		// Get the toplevel visual
		if (TopLevel.GetTopLevel(this) is not { } toplevelVisual)
		{
			return;
		}
		
		// Get the save file path through the file picker
		var path = await StorageProviderHelper.SelectSaveFileAsync(toplevelVisual,
			"Export netlist to SVG",
			null,
			".svg",
			"netlist.svg",
			true,
			new FilePickerFileType("SVG Image") {Patterns = ["*.svg"]});

		if (path == null)
		{
			return;
		}

		try
		{
			var file = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
			
			// Actually write SVG

			if (PanningNetlistControl.Child is GraphRootNodeControl rootNode)
			{
				rootNode.ToSVG(file, true, 0, 0, 100, 100);
			}
			
			file.Close();
		}
		catch (Exception exception)
		{
			// Todo improve error handling with better messages
			ServiceManager.GetService<ILogger>().Error(exception.Message,  exception);
		}
	}
}