using Avalonia.Media;

namespace FEntwumS.Common.Controls;

public class GraphRootNodeControl : GraphEntityNodeControl
{
	#region Rendering

	public override void Render(DrawingContext context)
	{
		// Don't render anything, since the root node only serves as a container for the actual graph contents
	}

	#endregion
}