using Avalonia.Threading;
using FEntwumS.Common.Controls;

namespace FEntwumS.Common.Builders;

public class GraphLabelBuilder : GenericGraphElementBuilder<GraphLabelControl>
{
	private string _content;
	private double _fontSize = 10.0d;

	public new static GraphLabelBuilder Create()
	{
		return new GraphLabelBuilder();
	}

	public GraphLabelBuilder WithContent(string content)
	{
		this._content = content;
		return this;
	}

	public GraphLabelBuilder WithFontSize(double fontSize)
	{
		this._fontSize = fontSize;
		return this;
	}

	public GraphLabelControl Build()
	{
		GraphLabelControl c = null;
		
		Dispatcher.UIThread.Invoke(() =>
		{
			c = base.Build();
			c.Content = this._content;
			c.Fontsize = this._fontSize;
		});
		
		return c;
	}
}