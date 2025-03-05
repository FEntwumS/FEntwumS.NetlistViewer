using Avalonia;

namespace FEntwumS.NetlistViewer.Types;

public class DCircle (double x, double y, double radius, ushort zIndex, NetlistElement element)
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public double Radius { get; set; } = radius;
    public ushort ZIndex { get; set; } = zIndex;
    public NetlistElement Element { get; set; } = element;

    public bool Hittest(Point pointer)
    {
        double distSquared = (X - pointer.X) * (X - pointer.X) + (Y - pointer.Y) * (Y - pointer.Y);

        if (distSquared <= Radius * Radius)
        {
            return true;
        }

        return false;
    }
}