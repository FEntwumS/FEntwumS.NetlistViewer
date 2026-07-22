using Avalonia;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Rendering;
using FEntwumS.Common.Types;

namespace FEntwumS.Common.Controls;

public class GraphJunctionControl : GenericGraphElementControl, ICustomHitTest
{
	#region Properties

	private JunctionShape _junctionShape = JunctionShape.Circle;
	
	/// <summary>
	/// The shape requested for this junction
	/// </summary>
	public static readonly StyledProperty<JunctionShape> JunctionShapeProperty =
		AvaloniaProperty.Register<GraphJunctionControl, JunctionShape>(nameof(_junctionShape),
			defaultBindingMode: BindingMode.TwoWay,
			defaultValue: JunctionShape.Circle);

	#endregion

	#region Variables

	private Geometry _contentGeometry = new PolylineGeometry([new Point(0, 0), new Point(0, 10)], true);

	#endregion

	#region Event handling

	protected override void OnInitialized()
	{
		double rh = Width / 2.0d,
			rv =  Height / 2.0d;

		double ow = Width * 1.4d,
			oh = Height * 1.4d;

		double orh = ow / 2.0d,
			orv = oh / 2.0d;

		double ilx = X - rh,
			irx = X + rh,
			ity = Y - rv,
			iby = Y + rv;

		double olx = X - orh,
			orx = X + orh,
			oty = Y - orv,
			oby = Y + orv;
		_contentGeometry = _junctionShape switch
		{
			JunctionShape.Circle => new EllipseGeometry(new Rect(ilx, ity, Width, Height)),
			JunctionShape.Square => new RectangleGeometry(new Rect(ilx, ity, Width, Height)),
			JunctionShape.Diamond => new PolylineGeometry([
				new Point(X, oty),
				new Point(orx, Y),
				new Point(X, oby),
				new Point(olx, Y)], true),
			JunctionShape.TriangleLeft => new PolylineGeometry([
				new Point(X, oty),
				new Point(orx, Y),
				new Point(X, oby)], true),
			JunctionShape.TriangleRight => new PolylineGeometry([
				new Point(X, oty),
				new Point(X, oby),
				new Point(olx, Y)], true),
			_ => throw new ArgumentOutOfRangeException()
		};
		
		base.OnInitialized();
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == NetlistThemeProperty)
		{
			
		}
		
		base.OnPropertyChanged(e);
	}

	#endregion

	#region Rendering

	public override void Render(DrawingContext context)
	{
		context.DrawGeometry(NetlistTheme.EdgeBrush, null, _contentGeometry);
		
		base.Render(context);
	}

	private void RegenerateDrawnElements()
	{
		_contentGeometry.Transform = new ScaleTransform(Scale, Scale);
	}

	#endregion

	#region Hittesting

	public bool HitTest(Point point)
	{
		throw new NotImplementedException();
	}

	#endregion
}