using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using FEntwumS.NetlistViewer.Types.HierarchyView;
using OneWare.Essentials.ViewModels;

namespace FEntwumS.NetlistViewer.ViewModels;

public class HierarchySidebarViewModel : ExtendedTool
{
    public ObservableCollection<HierarchySideBarElement> Elements
    {
        get => this._elements;
        set
        {
            this._elements.Clear();
            this._elements.AddRange(value);
            OnPropertyChanged();
        }
    }
    private ObservableCollection<HierarchySideBarElement> _elements { get; }

    public HierarchicalTreeDataGridSource<HierarchySideBarElement> Source { get; }
    
    private HierarchySideBarElement _selectedElement { get; set; }

    public HierarchySideBarElement SelectedElement
    {
        get => this._selectedElement;
        set
        {
            this._selectedElement = value;
            updateType();
            OnPropertyChanged();
        }
    }
    
    private string _selectedElementType { get; set; }

    public string SelectedElementType
    {
        get => this._selectedElementType;
        set
        {
            this._selectedElementType = value;
            OnPropertyChanged();
        }
    }

    public HierarchySidebarViewModel() : base("HierarchySidebar")
    {
        _elements = new ObservableCollection<HierarchySideBarElement>();
        Elements = new ObservableCollection<HierarchySideBarElement>();
        SelectedElementType = "";
        
        Source = new HierarchicalTreeDataGridSource<HierarchySideBarElement>(_elements)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<HierarchySideBarElement>(
                    new TextColumn<HierarchySideBarElement, string>("Instance name", x => x.Name ?? "TEST"),
                    x => x.Children)
            }
        };
    }

    void updateType()
    {
        SelectedElementType = SelectedElement.Type ?? "";
    }
}