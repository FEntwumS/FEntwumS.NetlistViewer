using Avalonia;
using Avalonia.Data;
using Avalonia.Interactivity;
using FEntwumS.Common.Services;
using OneWare.Essentials.Services;

namespace FEntwumS.Common.Controls;

public class GraphEntityNodeControl : GraphNodeControl
{
	#region Properties
	
	#endregion
	
	#region Variables

	private List<GraphEntityNodeControl> _childEntities = new List<GraphEntityNodeControl>();
	private bool _hasEntityChildren = true;
	#endregion
	
	#region Event handling

	protected override void OnLoaded(RoutedEventArgs e)
	{
		_childEntities.Clear();
		_childEntities.AddRange(this.VisualChildren.OfType<GraphEntityNodeControl>());
		
		_hasEntityChildren = _childEntities.Count > 0;
		
		base.OnLoaded(e);
	}

	protected override void OnViewPortChanged()
	{
		if (_hasEntityChildren)
		{
			foreach (GraphEntityNodeControl childEntity in _childEntities)
			{
				childEntity.CurrentViewPort = new Rect(CurrentViewPort.X - childEntity.X, CurrentViewPort.Y - childEntity.Y, CurrentViewPort.Width, CurrentViewPort.Height);;
			}
		}
		
		base.OnViewPortChanged();
	}

	protected override void CheckChildVisibility()
	{
#if DEBUG
		if (ServiceManager.GetService<ISettingsService>().GetSettingValue<bool>("NetlistViewer_FreezeGUIState"))
		{
			return;
		}
#endif
		
		base.CheckChildVisibility();
		
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