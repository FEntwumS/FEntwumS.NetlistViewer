using System.Collections.Specialized;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Rendering;

namespace FEntwumS.Common.Controls;

public class GraphNodeControl : GenericGraphElementControl, ICustomHitTest
{
	#region Properties

	private string CellName
	{
		get => GetValue(CellNameProperty);
		set => SetValue(CellNameProperty, value);
	}
	
	public static readonly StyledProperty<string> CellNameProperty =
		AvaloniaProperty.Register<GraphNodeControl, string>(nameof(CellName),
			defaultBindingMode: BindingMode.TwoWay,
			defaultValue: "");

	private string CellType
	{
		get => GetValue(CellTypeProperty);
		set => SetValue(CellTypeProperty, value);
	}

	public static readonly StyledProperty<string> CellTypeProperty =
		AvaloniaProperty.Register<GraphNodeControl, string>(nameof(CellType),
			defaultBindingMode: BindingMode.TwoWay,
			defaultValue: "");
	
	private AvaloniaList<Control> _interactionControls = new AvaloniaList<Control>();
	
	/// <summary>
	/// The buttons displayed on the node
	/// </summary>
	public static readonly DirectProperty<GraphNodeControl, AvaloniaList<Control>> InteractionControlsProperty =
		AvaloniaProperty.RegisterDirect<GraphNodeControl, AvaloniaList<Control>>(nameof(_interactionControls),
			control => control._interactionControls,
			delegate(GraphNodeControl control, AvaloniaList<Control> interactionControls)
			{
				control._interactionControls = interactionControls;
			});
	
	public AvaloniaList<PositionableSubControl> Items { get; } = new AvaloniaList<PositionableSubControl>();

	/// <summary>
	/// The items displayed within
	/// </summary>
	public static readonly DirectProperty<GraphNodeControl, AvaloniaList<PositionableSubControl>> ItemsProperty =
		AvaloniaProperty.RegisterDirect<GraphNodeControl, AvaloniaList<PositionableSubControl>>(nameof(Items),
			control => control.Items,
			(control, children) => {control.Items.Clear(); control.Items.AddRange(children); });

	#endregion

	#region Variables

	private Rect contentRect = new Rect(0, 0, 100, 100);
	private Point dsp1 = new Point(0, 0);
	private Point dsp2 = new Point(0, 0);
	private Point dsp3 = new Point(0, 0);

	#endregion

	#region Event handling

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == NetlistThemeProperty)
		{
			RegenerateDrawnElements();
		}

		if (e.Property == ScaleProperty)
		{
			RegenerateDrawnElements();
		}
		
		base.OnPropertyChanged(e);
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		VisualChildren.Clear();
		VisualChildren.AddRange(Items);
		
		Dictionary<(HorizontalAlignment, VerticalAlignment), Control> usedPositionDict = new();
		
		foreach (var interactionControl in _interactionControls.Where(interactionControl => !usedPositionDict.ContainsKey((interactionControl.HorizontalAlignment,
			         interactionControl.VerticalAlignment))))
		{
			usedPositionDict[(interactionControl.HorizontalAlignment, interactionControl.VerticalAlignment)] = interactionControl;
				
			VisualChildren.Add(interactionControl);
		}
		
		base.OnAttachedToVisualTree(e);
	}

	private void ItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		LogicalChildren.Clear();
		VisualChildren.Clear();
		
		LogicalChildren.AddRange(Items);
		VisualChildren.AddRange(Items);
		
		Dictionary<(HorizontalAlignment, VerticalAlignment), Control> usedPositionDict = new();

		foreach (var interactionControl in _interactionControls.Where(interactionControl => !usedPositionDict.ContainsKey((interactionControl.HorizontalAlignment,
			         interactionControl.VerticalAlignment))))
		{
			usedPositionDict[(interactionControl.HorizontalAlignment, interactionControl.VerticalAlignment)] = interactionControl;
				
			LogicalChildren.Add(interactionControl);
			VisualChildren.Add(interactionControl);
		}
	}

	#endregion

	#region Rendering

	protected override Size MeasureOverride(Size availableSize)
	{
		availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

		foreach (PositionableSubControl child in Items)
		{
			child.Measure(availableSize);
		}

		return new Size();
	}

	private void ArrangeSubControl(PositionableSubControl child, Size availableSize)
	{
		double x = 0.0d,
			y = 0.0d;

		if (!double.IsNaN(child.X))
		{
			x =  child.X;
		}

		if (!double.IsNaN(child.Y))
		{
			y =  child.Y;
		}
		
		child.Arrange(new Rect(x, y, child.DesiredSize.Width, child.DesiredSize.Height));
	}

	private void ArrangeInteractionControl(Control interactionControl, Size availableSize)
	{
		double x = interactionControl.HorizontalAlignment switch
		{
			HorizontalAlignment.Center => availableSize.Width / 2.0d - (interactionControl.DesiredSize.Width / 2.0d),
			HorizontalAlignment.Right => availableSize.Width - interactionControl.DesiredSize.Width,
			_ => 0.0d
		};

		double y = interactionControl.VerticalAlignment switch
		{
			VerticalAlignment.Center => availableSize.Height / 2.0d - (interactionControl.DesiredSize.Height / 2.0d),
			VerticalAlignment.Bottom => availableSize.Height - interactionControl.DesiredSize.Height,
			_ => 0.0d
		};
		
		interactionControl.Arrange(new Rect(x, y, interactionControl.DesiredSize.Width, interactionControl.DesiredSize.Height));
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		foreach (PositionableSubControl child in Items)
		{
			ArrangeSubControl(child, finalSize);
		}

		Dictionary<(HorizontalAlignment, VerticalAlignment), Control> usedPositionDict = new();

		foreach (Control interactionControl in _interactionControls)
		{
			if (!usedPositionDict.ContainsKey((interactionControl.HorizontalAlignment,
				    interactionControl.VerticalAlignment)))
			{
				usedPositionDict[(interactionControl.HorizontalAlignment, interactionControl.VerticalAlignment)] = interactionControl;
				
				ArrangeInteractionControl(interactionControl, finalSize);
			}
		}
		
		return finalSize;
	}
	
	public override void Render(DrawingContext context)
	{
		// Draw rect
		context.DrawRectangle(NetlistTheme.FillBrush, NetlistTheme.BorderPen, contentRect);
		
		// Draw dropshadow
		context.DrawLine(NetlistTheme.DropShadowPen, dsp1, dsp2);
		context.DrawLine(NetlistTheme.DropShadowPen, dsp2, dsp3);
	}

	private void RegenerateDrawnElements()
	{
		// Update the main rectangle
		contentRect = new Rect(0.0d, 0.0d, Width, Height);
		
		// Update the points of the dropshadow
		double l = ((NetlistTheme.BorderThickness + NetlistTheme.DropShadowThickness) / 2) * Scale;
		double r = l + Width;
		double t = ((NetlistTheme.BorderThickness + NetlistTheme.DropShadowThickness) / 2) * Scale;
		double b = t + Height;
		dsp1 = new Point(l, b);
		dsp2 = new Point(r, b);
		dsp3 = new Point(r, t);
	}

	#endregion

	#region Hittesting

	public bool HitTest(Point point)
	{
		return true;
		throw new NotImplementedException();
	}

	#endregion

	public GraphNodeControl()
	{
		Items.CollectionChanged += ItemsChanged;
		_interactionControls.CollectionChanged += ItemsChanged;
	}
}