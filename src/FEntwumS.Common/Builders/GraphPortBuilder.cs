using Avalonia.Threading;
using FEntwumS.Common.Controls;
using FEntwumS.Common.Types;

namespace FEntwumS.Common.Builders;

public class GraphPortBuilder : GenericGraphElementBuilder<GraphPortControl>
{
	private PortShape _portShape;

	public new static GraphPortBuilder Create()
	{
		return new GraphPortBuilder();
	}

	public GraphPortBuilder WithPortShape(PortShape portShape)
	{
		this._portShape = portShape;
		return this;
	}

	public GraphPortControl Build()
	{
		GraphPortControl c = null;
		
		Dispatcher.UIThread.Invoke(() =>
		{
			c = base.Build();
			c._portShape = this._portShape;
		});
		
		return c;
	}
}