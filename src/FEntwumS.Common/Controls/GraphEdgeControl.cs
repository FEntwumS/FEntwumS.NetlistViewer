using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;
using Avalonia.Rendering;

namespace FEntwumS.Common.Controls;

public class GraphEdgeControl : GenericGraphElementControl, ICustomHitTest
{
	#region Properties

	private AvaloniaList<Point> _points = new AvaloniaList<Point>();
	
	/// <summary>
	/// The points defining the path of the edge, from source to sink
	/// </summary>
	public static readonly DirectProperty<GraphEdgeControl, AvaloniaList<Point>> PointsProperty =
		AvaloniaProperty.RegisterDirect<GraphEdgeControl, AvaloniaList<Point>>(nameof(_points),
			o => o._points,
			(o, v) => o._points = v);

	#endregion

	#region Variables

	

	#endregion

	#region Event handling
	

	

	#endregion

	#region Rendering

	public override void Render(DrawingContext context)
	{
		base.Render(context);
	}

	#endregion

	#region Hittesting

	public bool HitTest(Point point)
	{
		throw new NotImplementedException();
	}

	#endregion
}