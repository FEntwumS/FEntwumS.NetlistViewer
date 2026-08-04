using FEntwumS.Common.Controls;

namespace FEntwumS.Common.Builders;

public class GraphLabelBuilder : GenericGraphElementBuilder<GraphLabelControl>
{
	private string _content;

	public new static GraphLabelBuilder Create()
	{
		return new GraphLabelBuilder();
	}

	public GraphLabelBuilder WithContent(string content)
	{
		this._content = content;
		return this;
	}

	public GraphLabelControl Build()
	{
		var c = base.Build();
		c.Content = this._content;
		return c;
	}
}