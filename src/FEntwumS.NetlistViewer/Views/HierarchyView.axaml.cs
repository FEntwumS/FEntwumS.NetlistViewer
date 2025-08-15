using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using FEntwumS.NetlistViewer.Types;
using FEntwumS.NetlistViewer.ViewModels;

namespace FEntwumS.NetlistViewer.Views;

public partial class HierarchyView : UserControl
{
	private HierarchyViewModel? _hierarchyViewModel;

	public HierarchyView()
	{
		InitializeComponent();

		if (DataContext is HierarchyViewModel vm)
		{
			Initialize(vm);
		}

		DataContextChanged += OnDataContextChanged;
	}

	private void Initialize(HierarchyViewModel vm)
	{
		_hierarchyViewModel = vm;
	}

	private void OnDataContextChanged(object? sender, EventArgs e)
	{
		if (IsInitialized)
		{
			_hierarchyViewModel = DataContext as HierarchyViewModel;
		}
		else
		{
			Initialized += delegate { OnDataContextChanged(sender, e); };
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == DataContextProperty)
		{
			if (_hierarchyViewModel == null)
			{
				_hierarchyViewModel = e.NewValue as HierarchyViewModel;
			}
		}

		base.OnPropertyChanged(e);
	}
}