using Avalonia.Media;

namespace FEntwumS.NetlistViewer.Types.HierarchyView;

public class Port
{
    private string? _name;
    private StreamGeometry? _geometry;

    public string Name
    {
        get => _name ?? "";
        set => _name = value;
    }

    public StreamGeometry? Geometry
    {
        get => _geometry;
        set => _geometry = value;
    }
}