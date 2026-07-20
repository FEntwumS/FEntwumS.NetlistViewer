using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;

namespace FEntwumS.Common.Controls;

public class PanningControl : Control
{
    #region Properties

    private double OffsetX
    {
	    get => GetValue(OffsetXProperty);
	    set => SetValue(OffsetXProperty, value);
    }
    
    public static readonly StyledProperty<double> OffsetXProperty =
	    AvaloniaProperty.Register<PanningControl, double>(nameof(OffsetX),
		    defaultBindingMode: BindingMode.TwoWay,
		    defaultValue: 0.0d);

    private double OffsetY
    {
	    get => GetValue(OffsetYProperty);
	    set => SetValue(OffsetYProperty, value);
    }
    
    public static readonly StyledProperty<double> OffsetYProperty =
	    AvaloniaProperty.Register<PanningControl, double>(nameof(OffsetY),
		    defaultBindingMode: BindingMode.TwoWay,
		    defaultValue: 0.0d);

    public PositionableSubControl? Child
    {
	    get => GetValue(ChildProperty);
	    set => SetValue(ChildProperty, value);
    }

    public static readonly StyledProperty<PositionableSubControl?> ChildProperty =
	    AvaloniaProperty.Register<PanningControl, PositionableSubControl?>(nameof(Child),
		    defaultBindingMode: BindingMode.TwoWay);

    public double ZoomStepSize
    {
	    get => GetValue(ZoomStepSizeProperty);
	    set => SetValue(ZoomStepSizeProperty, value);
    }
    
    public static readonly StyledProperty<double> ZoomStepSizeProperty =
	    AvaloniaProperty.Register<PanningControl, double>(nameof(ZoomStepSize),
		    defaultBindingMode: BindingMode.TwoWay,
		    defaultValue: 1.1d,
		    enableDataValidation: true,
		    validate: d => d > 1.0d);
    
    #endregion
    
    #region Rendering
    
    protected override Size MeasureOverride(Size availableSize)
    {
	    return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
	    return base.ArrangeOverride(finalSize);
    }

    public override void Render(DrawingContext context)
    {
	    base.Render(context);
    }

    #endregion
    
    #region Event handling

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
	    // Get the number of scrolled steps / mouse wheel notches as well as the current cursor position
	    var verticalDelta = e.Delta.Y;
	    var pointer = e.GetPosition(this);
	    
	    // Compute the total scale difference
	    double deltaScaleToApply = Math.Pow(ZoomStepSize, Math.Abs(verticalDelta));
	    
	    // Update the scale
	    if (Child is not null)
	    {
		    Child.Scale *= deltaScaleToApply;
	    }

	    // Update the offset in the root to visually zoom towards / away from the cursor
	    OffsetX *= deltaScaleToApply;
	    OffsetX += pointer.X - pointer.X * deltaScaleToApply;
	    OffsetY *= deltaScaleToApply;
	    OffsetY += pointer.Y - pointer.Y * deltaScaleToApply;
	    
	    e.Handled = true;
    }
    
    #endregion

    public PanningControl()
    {
	    // Subscribe to events
	    PointerWheelChanged += OnPointerWheelChanged;
    }
}