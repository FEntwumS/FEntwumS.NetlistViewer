using System.Text;
using Avalonia;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
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

	protected List<GraphEntityNodeControl> _childEntities = new List<GraphEntityNodeControl>();
	protected bool _hasEntityChildren = true;

	private List<GenericGraphElementControl> _childElements = new List<GenericGraphElementControl>();
	private bool _hasChildElements = true;

	public List<GenericGraphElementControl> associatedPortsAndLabels = new List<GenericGraphElementControl>();
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

	#region Rendering
	public override void Render(DrawingContext context)
	{
		//context.DrawRectangle(new SolidColorBrush(new Color(128, 0, 255, 0)), null, CurrentViewPort);
		
		base.Render(context);
	}

	public override void ToSVG(FileStream svgFileStream, bool isTop, double offsetX, double offsetY, double width,
		double height)
	{
		double invariantScale = 1.0d / Scale;
		
		if (isTop)
		{
			if (this is not GraphRootNodeControl)
			{
				var (dx, dy, w, h) = EstimateOffsets();

				offsetX = dx;
				offsetY = dy;
				width = w;
				height = h;
			}

			string svgString = $"<svg width=\"{width}\" height=\"{height}\" xmlns=\"http://www.w3.org/2000/svg\">\n";
			var svgBytes = new UTF8Encoding(true).GetBytes(svgString);
		
			svgFileStream.Write(svgBytes);
		}

		foreach (GenericGraphElementControl associatedElement in associatedPortsAndLabels)
		{
			associatedElement.ToSVG(svgFileStream, false, offsetX, offsetY, width, height);
		}
		
		base.ToSVG(svgFileStream, false, offsetX, offsetY, width, height);

		if (isTop)
		{
			string svgString = "</svg>";
			var svgBytes = new UTF8Encoding(true).GetBytes(svgString);
		
			svgFileStream.Write(svgBytes);
		}
	}

	private (double offsetX, double offsetY, double width, double height) EstimateOffsets()
	{
		double lx = 0.0d,
			rx = 0.0d,
			ty = 0.0d,
			by = 0.0d;

		double invariantScale = 1.0d / Scale;

		foreach (GenericGraphElementControl element in associatedPortsAndLabels)
		{
			double elementInvariantScale = 1.0d / element.Scale;
			
			double dlx = element.X * elementInvariantScale - X * invariantScale,
				drx = (element.X + element.ElementWidth) * elementInvariantScale - (X + ElementWidth) * invariantScale,
				dty =  element.Y * elementInvariantScale - Y * invariantScale,
				dby = (element.Y + element.ElementHeight) * elementInvariantScale - (Y + ElementHeight) * invariantScale;

			if (dlx < lx)
			{
				lx = dlx;
			}

			if (drx > rx)
			{
				rx = drx;
			}

			if (dty < ty)
			{
				ty = dty;
			}

			if (dby > by)
			{
				by = dby;
			}
		}
		
		return (-lx - X * invariantScale, -ty - Y * invariantScale, -lx + ElementWidth * invariantScale + rx, -ty+ ElementHeight * invariantScale + by);
	}
	
	#endregion
}