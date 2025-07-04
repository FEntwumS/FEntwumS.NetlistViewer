namespace FEntwumS.NetlistViewer.Types;

public class HierarchyInformation
{
    private double _maxHeight { get; set; }

    public double MaxHeight
    {
        get => _maxHeight;
        set => _maxHeight = value;
    }
    
    private double _maxWidth { get; set; }

    public double MaxWidth
    {
        get => _maxWidth;
        set => _maxWidth = value;
    }
    
    private double _topX { get; set; }

    public double TopX
    {
        get => _topX;
        set => _topX = value;
    }
    
    private double _topY { get; set; }

    public double TopY
    {
        get => _topY;
        set => _topY = value;
    }
    
    private double _topWidth { get; set; }

    public double TopWidth
    {
        get => _topWidth;
        set => _topWidth = value;
    }
    
    private double _topHeight { get; set; }

    public double TopHeight
    {
        get => _topHeight;
        set => _topHeight = value;
    }
}