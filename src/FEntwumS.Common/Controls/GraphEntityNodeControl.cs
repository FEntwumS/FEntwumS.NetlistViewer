using Avalonia;
using Avalonia.Data;

namespace FEntwumS.Common.Controls;

public class GraphEntityNodeControl : GraphNodeControl
{
	#region Properties

	private Rect _currentViewPort= new Rect(0, 0, 3980, 2160);

	public Rect CurrentViewPort
	{

		get => _currentViewPort;
		set
		{
			_currentViewPort = value;
			CheckChildVisibility();

			if (_hasEntityChildren)
			{
				foreach (GraphEntityNodeControl childEntity in _childEntities)
				{
					childEntity.CurrentViewPort = _currentViewPort;
				}
			}
		}
	}
	
	#endregion
	
	#region Variables

	private List<GraphEntityNodeControl> _childEntities = new List<GraphEntityNodeControl>();
	private bool _hasEntityChildren = true;
	#endregion
	
	#region Event handling

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		_childEntities.Clear();
		_childEntities.AddRange(this.VisualChildren.OfType<GraphEntityNodeControl>());
		
		_hasEntityChildren = _childEntities.Count > 0;
		
		base.OnAttachedToVisualTree(e);
	}

	protected void CheckChildVisibility()
	{
		if (!_hasEntityChildren)
		{
			return;
		}
		
		foreach (GraphEntityNodeControl childEntity in _childEntities)
		{
			Rect childArea = new Rect(childEntity.X, childEntity.Y,
				childEntity.Width,  childEntity.Height);
			
			bool isVisible = CurrentViewPort.Contains(childArea)
				|| childArea.Intersects(CurrentViewPort)
				|| childArea.Contains(CurrentViewPort);
			
			childEntity.IsVisible = isVisible;
		}
	}

	#endregion
}