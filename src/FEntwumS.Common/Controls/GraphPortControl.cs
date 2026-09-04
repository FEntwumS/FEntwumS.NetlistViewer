using Avalonia;
using Avalonia.Data;
using Avalonia.Media;
using FEntwumS.Common.Types;

namespace FEntwumS.Common.Controls;

public class GraphPortControl : GenericGraphElementControl
{
	#region Properties

	public PortShape _portShape
	{
		get => GetValue(PortShapeProperty);
		set => SetValue(PortShapeProperty, value);
	}
	
	public static readonly StyledProperty<PortShape> PortShapeProperty =
		AvaloniaProperty.Register<GraphPortControl, PortShape>(nameof(_portShape),
			defaultBindingMode: BindingMode.TwoWay,
			defaultValue: PortShape.Square);

	#endregion

	#region Variables

	private Geometry _contentGeometry = new PolylineGeometry([new Point(0, 0), new Point(0, 10)], true);

	#endregion

	#region Event handling

	protected override void OnInitialized()
	{
		double lx = 0.0d,
			rx = 0.0d + ElementWidth,
			rox = rx - 5.0d,
			mx = rx - (ElementWidth / 2.0d),
			ty = 0.0d,
			by = 0.0d + ElementHeight,
			my = by - (ElementHeight / 2.0d);
		
		_contentGeometry = _portShape switch
		{
			PortShape.Square => new PolylineGeometry([
				new Point(lx, ty),
				new Point(rx, ty),
				new Point(rx, by),
				new Point(lx, by)
			], true),
			PortShape.Tag =>
			new PolylineGeometry([
				new Point(lx, ty),
				new Point(rox, ty),
				new Point(rx, my),
				new Point(rox, by),
				new Point(lx, by)
			], true),
			PortShape.SquareCircle => new GeometryGroup()
			{
				Children = [
					new RectangleGeometry(new Rect(lx, ty, ElementWidth / 2.0d, ElementHeight)),
					new EllipseGeometry(new Rect(mx + 1.0d, ty + 1.0d, (ElementWidth / 2.0d) - 2.0d, ElementHeight - 2.0d))
				]
			},
			PortShape.CircleSquare => new GeometryGroup()
			{
				Children = [
					new EllipseGeometry(new Rect(lx + 1.0d, ty + 1.0d, (ElementWidth / 2.0d) - 2.0d, ElementHeight - 2.0d)),
					new RectangleGeometry(new Rect(mx, ty, ElementWidth / 2.0d, ElementHeight))
				]
			},
			_ => throw new ArgumentOutOfRangeException()
		};
		
		base.OnInitialized();
	}

	#endregion

	#region Rendering

	public override void Render(DrawingContext context)
	{
		if (_contentGeometry.Bounds.Height >= 2.0d)
		{
			context.DrawGeometry(NetlistTheme.FillBrush, NetlistTheme.BorderPen, _contentGeometry);
		}
	}

	protected override void RegenerateDrawnElements(double newScale)
	{
		_contentGeometry.Transform = new ScaleTransform(newScale, newScale);
	}

	#endregion
}