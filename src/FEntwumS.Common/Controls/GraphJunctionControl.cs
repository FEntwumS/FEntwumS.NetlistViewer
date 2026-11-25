using System.Text;
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

	protected override void UpdateScale(double newScale)
	{
		base.UpdateScale(newScale);
	}

	protected override void RegenerateDrawnElements(double newScale)
	{
		if (!this.IsVisible)
		{
			return;
		}
		
		_contentGeometry.Transform = new ScaleTransform(newScale, newScale);
	}

	public override void ToSVG(FileStream svgFileStream, bool isTop, double offsetX, double offsetY, double width,
		double height)
	{
		double invariantScale = 1.0d / Scale;
		double baseX = offsetX + X * invariantScale;
		double baseY = offsetY + Y * invariantScale;
		StringBuilder svgStringBuilder = new();

		if (_contentGeometry is EllipseGeometry ellipseGeometry)
		{
			svgStringBuilder.Append($"<ellipse cx=\"{baseX + ellipseGeometry.Center.X * invariantScale}\" cy=\"{baseY + ellipseGeometry.Center.Y * invariantScale}\" rx=\"{ellipseGeometry.Rect.Width / 2.0d}\" ry=\"{ellipseGeometry.Rect.Height / 2.0d}\" fill=\"{NetlistTheme.EdgeColor.ToString().ToLower()}\" style=\"fill:#{Convert.ToString(NetlistTheme.EdgeColor.R, 16)}{Convert.ToString(NetlistTheme.EdgeColor.G, 16)}{Convert.ToString(NetlistTheme.EdgeColor.B, 16)};fill-opacity:{NetlistTheme.EdgeColor.A / 256.0d}\"/>\n");
		} else if (_contentGeometry is RectangleGeometry rectangleGeometry)
		{
			svgStringBuilder.Append(
				$"<rect x=\"{baseX + rectangleGeometry.Rect.X * invariantScale}\" y=\"{baseY + rectangleGeometry.Rect.Y * invariantScale}\" width=\"{rectangleGeometry.Rect.Width}\" height=\"{rectangleGeometry.Rect.Height}\" fill=\"{NetlistTheme.EdgeColor.ToString().ToLower()}\" style=\"fill:#{Convert.ToString(NetlistTheme.EdgeColor.R, 16)}{Convert.ToString(NetlistTheme.EdgeColor.G, 16)}{Convert.ToString(NetlistTheme.EdgeColor.B, 16)};fill-opacity:{NetlistTheme.EdgeColor.A / 256.0d}\"/>\n");
		} else if (_contentGeometry is PolylineGeometry polylineGeometry)
		{
			svgStringBuilder.Append("<path d=\"M");
			
			bool first = true;

			foreach (Point point in polylineGeometry.Points)
			{
				if (!first)
				{
					svgStringBuilder.Append(" L");
				}
				
				svgStringBuilder.Append($"{baseX + point.X} {baseY + point.Y}");
				
				first = false;
			}

			svgStringBuilder.Append($"Z\" fill=\"{NetlistTheme.EdgeColor.ToString().ToLower()}\" style=\"fill:#{Convert.ToString(NetlistTheme.EdgeColor.R, 16)}{Convert.ToString(NetlistTheme.EdgeColor.G, 16)}{Convert.ToString(NetlistTheme.EdgeColor.B, 16)};fill-opacity:{NetlistTheme.EdgeColor.A / 256.0d}\"/>\n");
		}
		else
		{
			return;
		}
		
		var svgBytes = new UTF8Encoding(true).GetBytes(svgStringBuilder.ToString());
		
		svgFileStream.Write(svgBytes);
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