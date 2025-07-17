using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
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
    }

    void updateType()
    {
        SelectedElementType = SelectedElement.Type ?? "";
    }
}