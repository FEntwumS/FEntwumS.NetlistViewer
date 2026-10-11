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
			OnViewPortChanged();
		}
	}
	
	#endregion
	
	#region Variables

	private List<GraphEntityNodeControl> _childEntities = new List<GraphEntityNodeControl>();
	private bool _hasEntityChildren = true;

	private List<GenericGraphElementControl> _childElements = new List<GenericGraphElementControl>();
	private bool _hasChildElements = true;
	#endregion
	
	#region Event handling

	protected override void OnLoaded(RoutedEventArgs e)
	{
		_childEntities.Clear();
		_childEntities.AddRange(this.VisualChildren.OfType<GraphEntityNodeControl>());
		
		_hasEntityChildren = _childEntities.Count > 0;
		
		_childElements.Clear();
		_childElements.AddRange(this.VisualChildren.OfType<GenericGraphElementControl>());
		
		_hasChildElements = _childElements.Count > 0;
		
		base.OnLoaded(e);
	}

	protected virtual void OnViewPortChanged()
	{
		if (!this.IsVisible)
		{
			return;
		}
		
		CheckChildVisibility();
		
		if (_hasEntityChildren)
		{
			foreach (GraphEntityNodeControl childEntity in _childEntities)
			{
				childEntity.CurrentViewPort = new Rect(CurrentViewPort.X - childEntity.X, CurrentViewPort.Y - childEntity.Y, CurrentViewPort.Width, CurrentViewPort.Height);
			}
		}
	}

	protected virtual void CheckChildVisibility()
	{
#if DEBUG
		if (ServiceManager.GetService<ISettingsService>().GetSettingValue<bool>("NetlistViewer_FreezeGUIState"))
		{
			return;
		}
#endif

		if (!_hasChildElements)
		{
			return;
		}

		foreach (GenericGraphElementControl childElement in _childElements)
		{
			Rect childArea = new Rect(childElement.X, childElement.Y,
				childElement.ElementWidth,  childElement.ElementHeight);
			
			bool isVisible = CurrentViewPort.Contains(childArea)
			                 || childArea.Intersects(CurrentViewPort)
			                 || childArea.Contains(CurrentViewPort);
			if (!childElement.IsVisible && isVisible)
			{
				childElement.IsVisible = true;
				
				childElement.Scale = this.Scale;
				
				childElement.InvalidateMeasure();
			}
			else
			{
				childElement.IsVisible = isVisible;
			}
		}
	}

	#endregion
}