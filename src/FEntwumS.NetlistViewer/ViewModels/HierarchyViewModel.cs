using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FEntwumS.NetlistViewer.Helpers;
using FEntwumS.NetlistViewer.Types.HierarchyView;
using FEntwumS.NetlistViewer.Types.Messages;
using OneWare.Essentials.ViewModels;

namespace FEntwumS.NetlistViewer.ViewModels;

public class HierarchyViewModel : ExtendedTool
{
	public ICommand ZoomToFitCommand { get; }

	public ICommand ZoomToToplevelCommand { get; }

	private ulong _netlistId { get; set; }

	public ulong NetlistId
	{
		get => _netlistId;
		set
		{
			_netlistId = value;
			OnPropertyChanged(nameof(NetlistId));
		}
	}

	private AvaloniaList<HierarchyViewElement> items { get; set; }

	public AvaloniaList<HierarchyViewElement> Items
	{
		get => items;
		set
		{
			items.Clear();
			items.AddRange(value);
			OnPropertyChanged(nameof(Items));
		}
	}

	private double _offsetX { get; set; }

	public double OffsetX
	{
		get => _offsetX;
		set
		{
			_offsetX = value;
			OnPropertyChanged(nameof(OffsetX));
		}
	}

	private double _offsetY { get; set; }

	public double OffsetY
	{
		get => _offsetY;
		set
		{
			_offsetY = value;
			OnPropertyChanged(nameof(OffsetY));
		}
	}

	private double _scale { get; set; }

	public double Scale
	{
		get => _scale;
		set
		{
			_scale = value;
			OnPropertyChanged(nameof(Scale));
		}
	}

	public HierarchyViewModel() : base("Hierarchy")
	{
		items = new AvaloniaList<HierarchyViewElement>();

		Scale = 1;

		ZoomToFitCommand = new RelayCommand(() =>
		{
			WeakReferenceMessenger.Default.Send(new ZoomToFitmessage(_netlistId),
				FentwumSNetlistViewerSettingsHelper.HierarchyMessageChannel);
		});

		ZoomToToplevelCommand = new RelayCommand(() =>
		{
			WeakReferenceMessenger.Default.Send(new ZoomToToplevelMessage(_netlistId),
				FentwumSNetlistViewerSettingsHelper.HierarchyMessageChannel);
		});
	}
}