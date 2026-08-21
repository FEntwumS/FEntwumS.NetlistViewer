using System.Collections.Specialized;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Rendering;

namespace FEntwumS.Common.Controls;

public class GraphNodeControl : GenericGraphElementControl/*, ICustomHitTest*/
{
	#region Properties

	public string CellName
	{
		get => GetValue(CellNameProperty);
		set => SetValue(CellNameProperty, value);
	}
	
	public static readonly StyledProperty<string> CellNameProperty =
		AvaloniaProperty.Register<GraphNodeControl, string>(nameof(CellName),
			defaultBindingMode: BindingMode.TwoWay,
			defaultValue: "");

	public string CellType
	{
		get => GetValue(CellTypeProperty);
		set => SetValue(CellTypeProperty, value);
	}

	public static readonly StyledProperty<string> CellTypeProperty =
		AvaloniaProperty.Register<GraphNodeControl, string>(nameof(CellType),
			defaultBindingMode: BindingMode.TwoWay,
			defaultValue: "");
	
	public AvaloniaList<Control> _interactionControls = new AvaloniaList<Control>();
	
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

	public string? LocationPath
	{
		get => GetValue(LocationPathProperty);
		set => SetValue(LocationPathProperty, value);
	}

	public static readonly StyledProperty<string?> LocationPathProperty =
		AvaloniaProperty.Register<GraphNodeControl, string?>(nameof(LocationPath),
			defaultBindingMode: BindingMode.TwoWay);

	#endregion

	#region Variables

	private GeometryGroup _contentGeometry = new GeometryGroup()
	{
		Children = new GeometryCollection([new RectangleGeometry(new Rect(0, 0, 100, 100))])
	};

	#endregion

	#region Event handling

	protected override void OnInitialized()
	{
		var contentRect = new Rect(0.0d, 0.0d, Width, Height);
		
		// double l = ((NetlistTheme.BorderThickness + NetlistTheme.DropShadowThickness) / 2) * Scale;
		// double r = l + Width;
		// double t = ((NetlistTheme.BorderThickness + NetlistTheme.DropShadowThickness) / 2) * Scale;
		// double b = t + Height;
		// var dsp1 = new Point(l, b);
		// var dsp2 = new Point(r, b);
		// var dsp3 = new Point(r, t);

		_contentGeometry.Children =
		[
			new RectangleGeometry(contentRect)/*,
			new PolylineGeometry([
				dsp1,
				dsp2,
				dsp3
			], false)*/
		];
		
		// Add the child elements to the visual tree. This is deferred to initialization to prevent excessive visual
		// tree recalculations and property propagations
		AddChildrenToVisualTree();
		
		
		Items.CollectionChanged += ItemsChanged;
		_interactionControls.CollectionChanged += ItemsChanged;
		
		base.OnInitialized();
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == NetlistThemeProperty && e.NewValue is not null)
		{
			RegenerateDrawnElements();
		}

		if (e.Property == ScaleProperty && NetlistTheme is not null)
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
		AddChildrenToVisualTree();
	}

	#endregion

	#region Rendering

	protected override Size MeasureCore(Size availableSize)
	{
		var requiredSize = base.MeasureCore(availableSize);
		
		MeasureOverride(requiredSize);

		return requiredSize;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

		foreach (PositionableSubControl child in Items)
		{
			child.Measure(availableSize);
		}

		foreach (Control interaactionControl in _interactionControls)
		{
			interaactionControl.Measure(availableSize);
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
		double offset = (NetlistTheme.BorderThickness + 2.0d) * Scale;
		double x = interactionControl.HorizontalAlignment switch
		{
			HorizontalAlignment.Center => availableSize.Width / 2.0d - (interactionControl.DesiredSize.Width / 2.0d) - offset,
			HorizontalAlignment.Right => availableSize.Width - interactionControl.DesiredSize.Width,
			_ => offset
		};

		double y = interactionControl.VerticalAlignment switch
		{
			VerticalAlignment.Center => availableSize.Height / 2.0d - (interactionControl.DesiredSize.Height / 2.0d) - offset,
			VerticalAlignment.Bottom => availableSize.Height - interactionControl.DesiredSize.Height,
			_ => offset
		};
		
		interactionControl.Arrange(new Rect(x, y, interactionControl.DesiredSize.Width, interactionControl.DesiredSize.Height));
	}

	protected override void ArrangeCore(Rect finalRect)
	{
		ArrangeOverride(finalRect.Size);
		
		base.ArrangeCore(finalRect);
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
		context.DrawGeometry(NetlistTheme.FillBrush, NetlistTheme.BorderPen, _contentGeometry);

		foreach (Control child in _interactionControls)
		{
			//child.Render(context);
		}
	}

	private void RegenerateDrawnElements()
	{
		_contentGeometry.Transform = new ScaleTransform(Scale, Scale);
	}

	#endregion

	#region Hittesting

	public bool HitTest(Point point)
	{
		return Bounds.Contains(point);
		throw new NotImplementedException();
	}

	#endregion

	private void AddChildrenToVisualTree()
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
}