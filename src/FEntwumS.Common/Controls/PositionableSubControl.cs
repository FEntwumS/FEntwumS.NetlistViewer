using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Metadata;

namespace FEntwumS.Common.Controls;

public class PositionableSubControl : Control
{
    #region Properties

    private double X { get; set; }

    /// <summary>
    /// The x-coordinate of the top-left corner relative to the parent element
    /// </summary>
    public static readonly StyledProperty<double> XProperty =
        AvaloniaProperty.Register<PositionableSubControl, double>(nameof(X),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: 0.0d);

    private double Y { get; set; }

    /// <summary>
    /// The y-coordinate of the top-left corner relative to the parent element
    /// </summary>
    public static readonly StyledProperty<double> YProperty =
        AvaloniaProperty.Register<PositionableSubControl, double>(nameof(Y),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: 0.0d);
    
    private double Scale { get; set; }
    
    /// <summary>
    /// The scale of the element. Since the scale is inherited, it only needs to be set on the root element
    /// </summary>
    public static readonly StyledProperty<double> ScaleProperty =
	    AvaloniaProperty.Register<PositionableSubControl, double>(nameof(Scale),
		    defaultBindingMode: BindingMode.TwoWay,
		    defaultValue: 1.0d,
		    enableDataValidation: true,
		    validate: d => d > 0.0d,
		    inherits: true);

    private IEnumerable<PositionableSubControl> _items = new AvaloniaList<PositionableSubControl>();

    /// <summary>
    /// The items displayed within
    /// </summary>
    public static readonly DirectProperty<PositionableSubControl, IEnumerable<PositionableSubControl>> ItemsProperty =
        AvaloniaProperty.RegisterDirect<PositionableSubControl, IEnumerable<PositionableSubControl>>(nameof(_items),
            control => control._items,
            (control, children) => control._items = children);

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