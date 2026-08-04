using Avalonia.Controls;
using Avalonia.Layout;
using FEntwumS.Common.Controls;

namespace FEntwumS.Common.Builders;

public class GraphNodeBuilder : GenericGraphElementBuilder<GraphNodeControl>
{
	private string _cellName;
	private string _cellType;
	private StackPanel? _topLeftInteractionControls;
	private StackPanel? _topRightInteractionControls;
	
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

	public GraphNodeControl Build()
	{
		var c = base.Build();
		c.CellName = this._cellName;
		c.CellType = this._cellType;

		if (_topLeftInteractionControls is not null)
		{
			c._interactionControls.Add(_topLeftInteractionControls);
		}

		if (_topRightInteractionControls is not null)
		{
			c._interactionControls.Add(_topRightInteractionControls);
		}

		return c;
	}
}