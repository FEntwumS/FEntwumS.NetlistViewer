using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using FEntwumS.NetlistViewer.Types.HierarchyView;
using OneWare.Essentials.ViewModels;

namespace FEntwumS.NetlistViewer.ViewModels;

public class HierarchySidebarViewModel : ExtendedTool
{
	public AvaloniaList<HierarchySideBarElement> Elements
	{
		get => this._elements;
		set
		{
			this._elements.Clear();
			this._elements.AddRange(value);
			OnPropertyChanged();
		}
	}

	private AvaloniaList<HierarchySideBarElement> _elements;

	private HierarchySideBarElement? _selectedElement;

	public HierarchySideBarElement SelectedElement
	{
		get => this._selectedElement ?? new HierarchySideBarElement();
		set
		{
			this._selectedElement = value;
			OnPropertyChanged();
		}
	}

	public HierarchySidebarViewModel() : base("HierarchySidebar")
	{
		_elements = new AvaloniaList<HierarchySideBarElement>();
		Elements = new AvaloniaList<HierarchySideBarElement>();
	}
}