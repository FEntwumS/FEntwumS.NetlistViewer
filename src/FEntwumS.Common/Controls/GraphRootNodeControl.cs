using Avalonia.Media;

namespace FEntwumS.Common.Controls;

public class GraphRootNodeControl : GraphEntityNodeControl
{
	#region Rendering

	public override void Render(DrawingContext context)
	{
		// Don't render anything, since the root node only serves as a container for the actual graph contents
	}

	public override void ToSVG(FileStream svgFileStream, bool isTop, double offsetX, double offsetY, double width,
		double height)
	{
		if (_hasEntityChildren)
		{
			_childEntities[0].ToSVG(svgFileStream, true, 0, 0, ElementWidth, ElementHeight);
		}
	}

	#endregion

	protected override void UpdateScale(double newScale)
	{
		NetlistTheme.RegenerateBrushesAndPens(newScale);
		
		base.UpdateScale(newScale);
	}
}