using Avalonia;
using Avalonia.Data;
using Avalonia.Media;
using FEntwumS.Common.Types;

namespace FEntwumS.Common.Controls;

public class GraphPortControl : GenericGraphElementControl
{
	#region Properties

	private PortShape _portShape = PortShape.Square;
	
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
		double lx = X,
			rx = X + Width,
			mx = rx - 5.0d,
			ty = Y,
			by = Y + Height,
			my = by - (Height / 2.0d);
		
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
				new Point(rx, ty),
				new Point(mx, ty),
				new Point(rx, my),
				new Point(mx, by),
				new Point(lx, by)
			], true),
			_ => throw new ArgumentOutOfRangeException()
		};
		
		base.OnInitialized();
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == NetlistThemeProperty)
		{
			RegenerateDrawnElements();
		}
		
		base.OnPropertyChanged(e);
	}

	#endregion

	#region Rendering

	public override void Render(DrawingContext context)
	{
		context.DrawGeometry(NetlistTheme.FillBrush, NetlistTheme.BorderPen, _contentGeometry);
		
		base.Render(context);
	}

	private void RegenerateDrawnElements()
	{
		_contentGeometry.Transform = new ScaleTransform(Scale, Scale);
	}

	#endregion
}