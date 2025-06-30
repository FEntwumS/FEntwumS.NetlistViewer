using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FEntwumS.NetlistViewer.Types.HierarchyView;

public class HierarchySideBarElement : INotifyPropertyChanged
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public ObservableCollection<Parameter> Attributes { get; set; } = new();
    public ObservableCollection<Port> Ports { get; set; } = new();
    public bool IsExpanded { get; set; }
    
    public ObservableCollection<HierarchySideBarElement> Children { get; } = new();
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}