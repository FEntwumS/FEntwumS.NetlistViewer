using System.Collections.ObjectModel;

namespace FEntwumS.NetlistViewer.Types.HierarchyView;

public class HierarchySideBarElement
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public ObservableCollection<string?> Attributes { get; set; } = new();
    public ObservableCollection<string?> Ports { get; set; } = new();
    
    public ObservableCollection<HierarchySideBarElement> Children { get; } = new();
}