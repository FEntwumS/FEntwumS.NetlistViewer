using Avalonia;

namespace FEntwumS.NetlistViewer.Types.HierarchyView;

public class HierarchyViewEdge : HierarchyViewElement
{
    private List<Point>? _points = new List<Point>();

    public List<Point>? Points
    {
        get => _points ?? new List<Point>();
        set => _points = value;
    }
}