using Avalonia;
using Avalonia.Collections;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Rendering;
using FEntwumS.Common.Services;
using OneWare.Essentials.Services;

namespace FEntwumS.Common.Controls;

public class GraphEdgeControl : GenericGraphElementControl, ICustomHitTest
{
	#region Properties

	public AvaloniaList<Point> _points = new AvaloniaList<Point>();
	
	/// <summary>
	/// The points defining the path of the edge, from source to sink
	/// </summary>
	public static readonly DirectProperty<GraphEdgeControl, AvaloniaList<Point>> PointsProperty =
		AvaloniaProperty.RegisterDirect<GraphEdgeControl, AvaloniaList<Point>>(nameof(_points),
			o => o._points,
			(o, v) => o._points = v);

	public bool _isThick
	{
		get => GetValue(IsThickProperty);
		set => SetValue(IsThickProperty, value);
	}
	
	public static readonly StyledProperty<bool> IsThickProperty =
		AvaloniaProperty.Register<GraphEdgeControl, bool>(nameof(_isThick),
			defaultBindingMode: BindingMode.TwoWay,
			defaultValue: false);

	#endregion

	#region Variables

	private Geometry _contentGeometry = new PolylineGeometry([new Point(0, 0), new Point(0, 10)], false);

	#endregion

	#region Event handling

	protected override void OnInitialized()
	{
		_contentGeometry = new PolylineGeometry(_points, false);
		

		var b = _contentGeometry.Bounds;

		X += b.X;
		Y += b.Y;
		ElementWidth = b.Width;
		ElementHeight = b.Height;

		AvaloniaList<Point> offsetPoints = new(_points.Count);

		foreach (Point p in _points)
		{
			offsetPoints.Add(new Point(p.X - b.X,  p.Y - b.Y));
		}
		
		_points = offsetPoints;
		
		_contentGeometry = new PolylineGeometry(_points, false);
		
		base.OnInitialized();
	}

	#endregion

	#region Rendering

	public override void Render(DrawingContext context)
	{
		#if DEBUG
		if (ServiceManager.GetService<ISettingsService>().GetSettingValue<bool>("NetlistViewer_ShowEdgeAABB"))
		{
			context.DrawRectangle(null, NetlistTheme.HighLightPen, new Rect(0, 0, ElementWidth, ElementHeight));
		}
#endif
		Pen linePen = _isThick ? NetlistTheme.BundledEdgePen : NetlistTheme.EdgePen;
		context.DrawGeometry(null, linePen, _contentGeometry);
	}

	protected override void RegenerateDrawnElements(double newScale)
	{
		if (!this.IsVisible)
		{
			return;
		}
		
		_contentGeometry.Transform = new ScaleTransform(newScale, newScale);
		Rect newBounds = _contentGeometry.Bounds;
	}

	#endregion

	#region Hittesting

	public bool HitTest(Point point)
	{
		if (NetlistTheme is null)
		{
			return false;
		}
		
		Pen linePen = _isThick ? NetlistTheme.BundledEdgePen : NetlistTheme.EdgePen;
		return _contentGeometry.StrokeContains(linePen, point);
	}

	#endregion
}