using Avalonia;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Rendering;
using FEntwumS.Common.Types;

namespace FEntwumS.Common.Controls;

public class GraphJunctionControl : GenericGraphElementControl, ICustomHitTest
{
	#region Properties

	public JunctionShape _junctionShape
	{
		get => GetValue(JunctionShapeProperty);
		set => SetValue(JunctionShapeProperty, value);
	}
	
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
		double rh = ElementWidth / 2.0d,
			rv =  ElementHeight / 2.0d;

		double ow = ElementWidth * 1.4d,
			oh = ElementHeight * 1.4d;

		double orh = ow / 2.0d,
			orv = oh / 2.0d;

		double ilx = -rh,
			irx = rh,
			ity = -rv,
			iby = rv;

		double olx = -orh,
			orx = orh,
			oty = -orv,
			oby = orv;
		_contentGeometry = _junctionShape switch
		{
			JunctionShape.Circle => new EllipseGeometry(new Rect(ilx, ity, ElementWidth, ElementHeight)),
			JunctionShape.Square => new RectangleGeometry(new Rect(ilx, ity, ElementWidth, ElementHeight)),
			JunctionShape.Diamond => new PolylineGeometry([
				new Point(0.0d, oty),
				new Point(orx, 0.0d),
				new Point(0.0d, oby),
				new Point(olx, 0.0d)], true),
			JunctionShape.TriangleLeft => new PolylineGeometry([
				new Point(0.0d, oty),
				new Point(orx, 0.0d),
				new Point(0.0d, oby)], true),
			JunctionShape.TriangleRight => new PolylineGeometry([
				new Point(0.0d, oty),
				new Point(0.0d, oby),
				new Point(olx, 0.0d)], true),
			_ => throw new ArgumentOutOfRangeException()
		};
		
		base.OnInitialized();
	}

	#endregion

	#region Rendering

	public override void Render(DrawingContext context)
	{
		if (ElementHeight >= 2.0d)
		{
			context.DrawGeometry(NetlistTheme.EdgeBrush, null, _contentGeometry);
		}
	}

	protected override void RegenerateDrawnElements(double newScale)
	{
		_contentGeometry.Transform = new ScaleTransform(newScale, newScale);
	}

	#endregion

	#region Hittesting

	public bool HitTest(Point point)
	{
		return false;
		throw new NotImplementedException();
	}

	#endregion
}