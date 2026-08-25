using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace FEntwumS.Common.Controls;

public abstract class PositionableSubControl : UserControl
{
    #region Properties

    public double X
    {
	    get => GetValue(XProperty);
	    set => SetValue(XProperty, value);
    }

    /// <summary>
    /// The x-coordinate of the top-left corner relative to the parent element
    /// </summary>
    public static readonly StyledProperty<double> XProperty =
        AvaloniaProperty.Register<PositionableSubControl, double>(nameof(X),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: 0.0d);

    public double Y
    {
	    get => GetValue(YProperty);
	    set => SetValue(YProperty, value);
    }

    /// <summary>
    /// The y-coordinate of the top-left corner relative to the parent element
    /// </summary>
    public static readonly StyledProperty<double> YProperty =
        AvaloniaProperty.Register<PositionableSubControl, double>(nameof(Y),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: 0.0d);

    public double Scale
    {
	    get => GetValue(ScaleProperty);
	    set => SetValue(ScaleProperty, value);
    }
    
    /// <summary>
    /// The scale of the element. Since the scale is inherited, it only needs to be set on the root element
    /// </summary>
    public static readonly StyledProperty<double> ScaleProperty =
	    AvaloniaProperty.Register<PositionableSubControl, double>(nameof(Scale),
		    defaultBindingMode: BindingMode.TwoWay,
		    defaultValue: 1.0d,
		    inherits: true);

    #endregion
    
    #region Rendering

    

    #endregion
    
    #region Event handling

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
	    if (e.Property == ScaleProperty)
	    {
		    double scaleDifference = ((double)e.NewValue!) / ((double)e.OldValue!);
		    
		    this.Width *=  scaleDifference;
		    this.Height *=  scaleDifference;
		    this.X *= scaleDifference;
		    this.Y *= scaleDifference;
		    
		    RegenerateDrawnElements();

		    return;
	    }
	    
	    base.OnPropertyChanged(e);
    }

    protected abstract void RegenerateDrawnElements();

    #endregion
}