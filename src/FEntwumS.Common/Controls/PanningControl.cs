using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

namespace FEntwumS.Common.Controls;

public class PanningControl<T> : Control where T : PositionableSubControl
{
    #region Properties
    
    private double OffsetX { get; set; }
    
    public static readonly StyledProperty<double> OffsetXProperty =
	    AvaloniaProperty.Register<PanningControl<T>, double>(nameof(OffsetX),
		    defaultBindingMode: BindingMode.TwoWay,
		    defaultValue: 0.0d);
    
    private double OffsetY { get; set; }
    
    public static readonly StyledProperty<double> OffsetYProperty =
	    AvaloniaProperty.Register<PanningControl<T>, double>(nameof(OffsetY),
		    defaultBindingMode: BindingMode.TwoWay,
		    defaultValue: 0.0d);
    
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
    
    #endregion
}