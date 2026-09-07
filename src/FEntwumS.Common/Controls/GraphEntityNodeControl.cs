using Avalonia;
using Avalonia.Data;
using Avalonia.Interactivity;
using FEntwumS.Common.Services;
using OneWare.Essentials.Services;

namespace FEntwumS.Common.Controls;

public class GraphEntityNodeControl : GraphNodeControl
{
	#region Properties

	private Rect _currentViewPort= new Rect(0, 0, 3840, 2160);

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
					childEntity.CurrentViewPort = new Rect(_currentViewPort.X - childEntity.X, _currentViewPort.Y - childEntity.Y, _currentViewPort.Width, _currentViewPort.Height);
				}
			}
		}
	}
	
	#endregion
	
	#region Variables

	private List<GraphEntityNodeControl> _childEntities = new List<GraphEntityNodeControl>();
	private List<GraphLabelControl> _childLabels = new List<GraphLabelControl>();
	private bool _hasEntityChildren = true;
	private bool _hasLabelChildren = true;
	#endregion
	
	#region Event handling

	protected override void OnLoaded(RoutedEventArgs e)
	{
		_childEntities.Clear();
		_childEntities.AddRange(this.VisualChildren.OfType<GraphEntityNodeControl>());
		
		_hasEntityChildren = _childEntities.Count > 0;
		
		_childLabels.Clear();
		_childLabels.AddRange(this.VisualChildren.OfType<GraphLabelControl>());
		
		_hasLabelChildren = _childLabels.Count > 0;
		
		base.OnLoaded(e);
	}

	protected void CheckChildVisibility()
	{
#if DEBUG
		if (ServiceManager.GetService<ISettingsService>().GetSettingValue<bool>("NetlistViewer_FreezeGUIState"))
		{
			return;
		}
#endif
		
		if (_hasLabelChildren)
		{
			foreach (GraphLabelControl childLabel in _childLabels)
			{
				Rect childArea = new Rect(childLabel.X, childLabel.Y,
					childLabel.ElementWidth,  childLabel.ElementHeight);
			
				bool isVisible = CurrentViewPort.Contains(childArea)
				                 || childArea.Intersects(CurrentViewPort)
				                 || childArea.Contains(CurrentViewPort);
			
				childLabel.IsVisible = isVisible;
			}
		}
		
		if (!_hasEntityChildren)
		{
			return;
		}
		
		foreach (GraphEntityNodeControl childEntity in _childEntities)
		{
			Rect childArea = new Rect(childEntity.X, childEntity.Y,
				childEntity.ElementWidth,  childEntity.ElementHeight);
			
			bool isVisible = CurrentViewPort.Contains(childArea)
				|| childArea.Intersects(CurrentViewPort)
				|| childArea.Contains(CurrentViewPort);
			
			childEntity.IsVisible = isVisible;
		}
	}

	#endregion
}