using Avalonia.Threading;
using FEntwumS.Common.Controls;
using FEntwumS.Common.Types;

namespace FEntwumS.Common.Builders;

public class GraphJunctionBuilder : GenericGraphElementBuilder<GraphJunctionControl>
{
	private JunctionShape _junctionShape;

	public new static GraphJunctionBuilder Create()
	{
		return new GraphJunctionBuilder();
	}

	public GraphJunctionBuilder WithJunctionShape(JunctionShape junctionShape)
	{
		this._junctionShape = junctionShape;
		return this;
	}

	public GraphJunctionControl Build()
	{
		GraphJunctionControl c = null;
		
		Dispatcher.UIThread.Invoke(() =>
		{
			c = base.Build();
			c._junctionShape = this._junctionShape;
		});
		
		return c;
	}
}