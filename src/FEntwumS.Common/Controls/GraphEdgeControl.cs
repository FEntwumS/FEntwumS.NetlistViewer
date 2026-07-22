using Avalonia;
using Avalonia.Collections;
using Avalonia.Data;
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

	private bool _isThick
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
		Pen linePen = _isThick ? NetlistTheme.BundledEdgePen : NetlistTheme.EdgePen;
		context.DrawGeometry(null, linePen, _contentGeometry);
		
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