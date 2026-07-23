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

    public Rect? ZoomBounds
    {
	    get => GetValue(ZoomBoundsProperty);
	    set => SetValue(ZoomBoundsProperty, value);
    }

    public static readonly StyledProperty<Rect?> ZoomBoundsProperty =
	    AvaloniaProperty.Register<PanningControl, Rect?>(nameof(ZoomBounds),
		    defaultBindingMode: BindingMode.TwoWay);
    
    #endregion
    
    #region Variables

    private bool _pointerPressed = false;
    private Point _pointerPosition = new Point(0, 0);
    
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
    
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
	    PointerPoint currentPoint = e.GetCurrentPoint(this);

	    _pointerPressed = currentPoint.Properties.IsLeftButtonPressed;

	    _pointerPosition = currentPoint.Position;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
	    PointerPoint currentPoint = e.GetCurrentPoint(this);

	    _pointerPressed = currentPoint.Properties.IsLeftButtonPressed;

	    _pointerPosition = currentPoint.Position;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
	    var pointerPoints = e.GetIntermediatePoints(this);
	    Point currentPointerPosition = e.GetPosition(this);

	    if (pointerPoints.First().Properties.IsLeftButtonPressed || _pointerPressed)
	    {
		    OffsetX += currentPointerPosition.X - _pointerPosition.X;
		    OffsetY += currentPointerPosition.Y - _pointerPosition.Y;
	    }
	    
	    _pointerPosition = currentPointerPosition;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
	    if (e.Property == ZoomBoundsProperty)
	    {
		    if (e.NewValue is Rect newZoomBounds)
		    {
			    ZoomToRect(newZoomBounds);
			    ZoomBounds = null;
		    }
	    }
	    
	    base.OnPropertyChanged(e);
    }

    #endregion
    
    #region Utility methods

    public void ZoomToFit()
    {
	    if (Child is not null)
	    {
		    ZoomToRect(Child.Bounds);
	    }
    }

    private void ZoomToRect(Rect shownBounds)
    {
	    if (Child is null)
	    {
		    return;
	    }
	    
	    if (shownBounds.Width == 0 || shownBounds.Height == 0)
	    {
		    return;
	    }
	    
	    double scaleX = Bounds.Width / shownBounds.Width;
	    double scaleY = Bounds.Height / shownBounds.Height;

	    if (scaleX < scaleY) // Bounds to be shown are relatively wider than the viewport area
	    {
		    // Set the new scale
		    Child.Scale = scaleX;
		    
		    // Set the correct offset to vertically center the child
		    OffsetX = shownBounds.X * -scaleX;
		    OffsetY = ((shownBounds.Y + (shownBounds.Height / 2.0d)) * -scaleX) + (Bounds.Height / 2.0d);
	    }
	    else
	    {
		    Child.Scale = scaleY;
		    
		    OffsetX = ((shownBounds.X + (shownBounds.Width / 2.0d)) * -scaleY)  + (Bounds.Width / 2.0d);
		    OffsetY = shownBounds.Y * -scaleY;
	    }
    }
    
    #endregion

    public PanningControl()
    {
	    // Subscribe to events
	    PointerWheelChanged += OnPointerWheelChanged;
	    PointerMoved += OnPointerMoved;
	    PointerPressed += OnPointerPressed;
	    PointerReleased += OnPointerReleased;
    }
}