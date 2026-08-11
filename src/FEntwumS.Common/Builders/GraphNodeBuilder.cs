using Avalonia.Controls;
using Avalonia.Layout;
using FEntwumS.Common.Controls;

namespace FEntwumS.Common.Builders;

public class GraphNodeBuilder : GenericGraphElementBuilder<GraphNodeControl>
{
	private string? _cellName;
	private string? _cellType;
	private StackPanel? _topLeftInteractionControls;
	private StackPanel? _topRightInteractionControls;
	private StackPanel? _topCenterInteractionControls;
	private StackPanel? _centerLeftInteractionControls;
	private StackPanel? _centerRightInteractionControls;
	private StackPanel? _centerCenterInteractionControls;
	private StackPanel? _bottomLeftInteractionControls;
	private StackPanel? _bottomRightInteractionControls;
	private StackPanel? _bottomCenterInteractionControls;
	private string? _locationPath;
	
	public new static GraphNodeBuilder Create()
	{
		return new GraphNodeBuilder();
	}

	public GraphNodeBuilder WithCellName(string cellName)
	{
		this._cellName = cellName;
		return this;
	}

	public GraphNodeBuilder WithCellType(string cellType)
	{
		this._cellType = cellType;
		return this;
	}
	
	#region Interaction control addition

	public GraphNodeBuilder WithTopLeftInteractionControl(Button button)
	{
		if (_topLeftInteractionControls is null)
		{
			_topLeftInteractionControls = new StackPanel();
			_topLeftInteractionControls.Spacing = 10.0d;
			_topLeftInteractionControls.HorizontalAlignment = HorizontalAlignment.Left;
			_topLeftInteractionControls.VerticalAlignment = VerticalAlignment.Top;
		}
		
		_topLeftInteractionControls.Children.Add(button);
		return this;
	}

	public GraphNodeBuilder WithTopRightInteractionControl(Button button)
	{
		if (_topRightInteractionControls is null)
		{
			_topRightInteractionControls = new StackPanel();
			_topRightInteractionControls.Spacing = 10.0d;
			_topRightInteractionControls.HorizontalAlignment = HorizontalAlignment.Right;
			_topRightInteractionControls.VerticalAlignment = VerticalAlignment.Top;
		}
		
		_topRightInteractionControls.Children.Add(button);
		return this;
	}

	public GraphNodeBuilder WithTopCenterInteractionControl(Button button)
	{
		if (_topCenterInteractionControls is null)
		{
			_topCenterInteractionControls = new StackPanel();
			_topCenterInteractionControls.Spacing = 10.0d;
			_topCenterInteractionControls.HorizontalAlignment = HorizontalAlignment.Center;
			_topCenterInteractionControls.VerticalAlignment = VerticalAlignment.Top;
			_topCenterInteractionControls.Orientation = Orientation.Horizontal;
		}
		
		_topCenterInteractionControls.Children.Add(button);
		return this;
	}
	
	public GraphNodeBuilder WithCenterLeftInteractionControl(Button button)
	{
		if (_centerLeftInteractionControls is null)
		{
			_centerLeftInteractionControls = new StackPanel();
			_centerLeftInteractionControls.Spacing = 10.0d;
			_centerLeftInteractionControls.HorizontalAlignment = HorizontalAlignment.Left;
			_centerLeftInteractionControls.VerticalAlignment = VerticalAlignment.Top;
		}
		
		_centerLeftInteractionControls.Children.Add(button);
		return this;
	}

	public GraphNodeBuilder WithCenterRightInteractionControl(Button button)
	{
		if (_centerRightInteractionControls is null)
		{
			_centerRightInteractionControls = new StackPanel();
			_centerRightInteractionControls.Spacing = 10.0d;
			_centerRightInteractionControls.HorizontalAlignment = HorizontalAlignment.Right;
			_centerRightInteractionControls.VerticalAlignment = VerticalAlignment.Top;
		}
		
		_centerRightInteractionControls.Children.Add(button);
		return this;
	}

	public GraphNodeBuilder WithCenterCenterInteractionControl(Button button)
	{
		if (_centerCenterInteractionControls is null)
		{
			_centerCenterInteractionControls = new StackPanel();
			_centerCenterInteractionControls.Spacing = 10.0d;
			_centerCenterInteractionControls.HorizontalAlignment = HorizontalAlignment.Center;
			_centerCenterInteractionControls.VerticalAlignment = VerticalAlignment.Top;
			_centerCenterInteractionControls.Orientation = Orientation.Horizontal;
		}

		_centerCenterInteractionControls.Children.Add(button);
		return this;
	}
	
	public GraphNodeBuilder WithBottomLeftInteractionControl(Button button)
	{
		if (_bottomLeftInteractionControls is null)
		{
			_bottomLeftInteractionControls = new StackPanel();
			_bottomLeftInteractionControls.Spacing = 10.0d;
			_bottomLeftInteractionControls.HorizontalAlignment = HorizontalAlignment.Left;
			_bottomLeftInteractionControls.VerticalAlignment = VerticalAlignment.Top;
		}
		
		_bottomLeftInteractionControls.Children.Add(button);
		return this;
	}

	public GraphNodeBuilder WithBottomRightInteractionControl(Button button)
	{
		if (_bottomRightInteractionControls is null)
		{
			_bottomRightInteractionControls = new StackPanel();
			_bottomRightInteractionControls.Spacing = 10.0d;
			_bottomRightInteractionControls.HorizontalAlignment = HorizontalAlignment.Right;
			_bottomRightInteractionControls.VerticalAlignment = VerticalAlignment.Top;
		}
		
		_bottomRightInteractionControls.Children.Add(button);
		return this;
	}

	public GraphNodeBuilder WithBottomCenterInteractionControl(Button button)
	{
		if (_bottomCenterInteractionControls is null)
		{
			_bottomCenterInteractionControls = new StackPanel();
			_bottomCenterInteractionControls.Spacing = 10.0d;
			_bottomCenterInteractionControls.HorizontalAlignment = HorizontalAlignment.Center;
			_bottomCenterInteractionControls.VerticalAlignment = VerticalAlignment.Top;
			_bottomCenterInteractionControls.Orientation = Orientation.Horizontal;
		}

		_bottomCenterInteractionControls.Children.Add(button);
		return this;
	}

	public GraphNodeBuilder WithInteractionControl(Button button,
		HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
		VerticalAlignment verticalAlignment = VerticalAlignment.Top)
	{
		switch (horizontalAlignment)
		{
			case HorizontalAlignment.Left:
				switch (verticalAlignment)
				{
					case VerticalAlignment.Top:
							return this.WithTopLeftInteractionControl(button);
						break;
					
					case VerticalAlignment.Bottom:
							return this.WithBottomLeftInteractionControl(button);
						break;
					
					default:
							return this.WithCenterLeftInteractionControl(button);
						break;
				}
				break;
			
			case HorizontalAlignment.Right:
				switch (verticalAlignment)
				{
					case VerticalAlignment.Top:
							return this.WithTopRightInteractionControl(button);
						break;
					
					case VerticalAlignment.Bottom:
							return this.WithBottomRightInteractionControl(button);
						break;
					
					default:
							return this.WithCenterRightInteractionControl(button);
						break;
				}
				break;
			
			default:
				switch (verticalAlignment)
				{
					case VerticalAlignment.Top:
							return this.WithTopCenterInteractionControl(button);
						break;
					
					case VerticalAlignment.Bottom:
							return this.WithBottomCenterInteractionControl(button);
						break;
					
					default:
							return this.WithCenterCenterInteractionControl(button);
						break;
				}
				break;
		}
	}
	
	#endregion

	public GraphNodeBuilder WithJumpToSourceInteractionControl(
		HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
		VerticalAlignment verticalAlignment = VerticalAlignment.Top)
	{
		// Make button
		// Add the bindings and behavior
		// add to relevant interaction control panel
		//What to do on error? Nothing???

		return this;
	}

	public GraphNodeBuilder WithLocationPath(string locationPath)
	{
		this._locationPath = locationPath;
		return this;
	}

	public GraphNodeControl Build()
	{
		var c = base.Build();
		c.CellName = this._cellName ?? "";
		c.CellType = this._cellType ?? "";
		c.LocationPath =  this._locationPath;

		if (_topLeftInteractionControls is not null)
		{
			c._interactionControls.Add(_topLeftInteractionControls);
		}

		if (_topRightInteractionControls is not null)
		{
			c._interactionControls.Add(_topRightInteractionControls);
		}

		if (_topCenterInteractionControls is not null)
		{
			c._interactionControls.Add(_topCenterInteractionControls);
		}

		return c;
	}
}