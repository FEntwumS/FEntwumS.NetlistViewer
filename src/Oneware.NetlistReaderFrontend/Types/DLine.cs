using Avalonia;

namespace Oneware.NetlistReaderFrontend.Types;

public class DLine(Point start, Point end, ushort zIndex, NetlistElement element)
{
    public Point Start { get; set; } = start;
    public Point End { get; set; } = end;
    public ushort ZIndex { get; set; } = zIndex;
    public NetlistElement element { get; set; } = element;

    private const double lenience = 1.33d;

    public bool Hittest(Point pointer)
    {
        double dx = End.X - Start.X;
        double dy = End.Y - Start.Y;

        double uy, ly, lx, rx, diff;

        // Edge case: Vertical line
        if (dx == 0)
        {
            diff = pointer.X - Start.X;

            if (diff < 0)
            {
                diff *= -1;
            }
            
            if (diff <= lenience)
            {
                if (Start.Y > End.Y)
                {
                    uy = End.Y;
                    ly = Start.Y;
                }
                else
                {
                    uy = Start.Y;
                    ly = End.Y;
                }

                if (pointer.Y >= uy && pointer.Y <= ly)
                {
                    return true;
                }
            }

            return false;
        }

        // Edge case: Horizontal line
        if (dy == 0)
        {
            diff = pointer.Y - Start.Y;

            if (diff < 0)
            {
                diff *= -1;
            }
            
            if (diff <= lenience)
            {
                if (Start.X > End.X)
                {
                    lx = End.X;
                    rx = Start.X;
                }
                else
                {
                    lx = Start.X;
                    rx = End.X;
                }

                if (pointer.X >= lx && pointer.X <= rx)
                {
                    return true;
                }
            }

            return false;
        }
        
        return false;
    }
}