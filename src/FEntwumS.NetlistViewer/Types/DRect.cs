using Avalonia;

namespace FEntwumS.NetlistViewer.Types;

public class DRect(double x, double y, double width, double height, ushort zIndex, NetlistElement? element)
{
	public double X { get; set; } = x;
	public double Y { get; set; } = y;
	public double Width { get; set; } = width;
	public double Height { get; set; } = height;
	public ushort ZIndex { get; set; } = zIndex;
	public NetlistElement? element { get; set; } = element;

	public bool Hittest(Point pointer)
	{
		if (pointer.X >= X && pointer.X <= X + Width)
		{
			if (pointer.Y >= Y && pointer.Y <= Y + Height)
			{
				return true;
			}
		}

		return false;
	}
}