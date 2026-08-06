using Avalonia;
using FEntwumS.Common.Controls;

namespace FEntwumS.Common.Builders;

public class GraphEdgeBuilder : GenericGraphElementBuilder<GraphEdgeControl>
{
	private List<Point>? _points;
	private bool _isThick;

	public new static GraphEdgeBuilder Create()
	{
		return new GraphEdgeBuilder();
	}

	public GraphEdgeBuilder WithPoint(Point point)
	{
		if (_points is null)
		{
			_points = new List<Point>();
		}
		
		_points.Add(point);
		return this;
	}

	public GraphEdgeBuilder WithIsThick(bool isThick)
	{
		this._isThick = isThick;
		return this;
	}

	public GraphEdgeBuilder WithPoints(List<Point> points)
	{
		if (_points is null)
		{
			_points = new List<Point>();
		}
		
		this._points.AddRange(points);
		return this;
	}

	public GraphEdgeControl Build()
	{
		var c = base.Build();
		if (_points is not null)
		{
			c._points.AddRange(_points);
		}
		c._isThick = this._isThick;
		return c;
	}
}