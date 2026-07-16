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

    public double X { get; set; }

    /// <summary>
    /// The x-coordinate of the top-left corner relative to the parent element
    /// </summary>
    public static readonly StyledProperty<double> XProperty =
        AvaloniaProperty.Register<PositionableSubControl, double>(nameof(X),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: 0.0d);

    public double Y { get; set; }

    /// <summary>
    /// The y-coordinate of the top-left corner relative to the parent element
    /// </summary>
    public static readonly StyledProperty<double> YProperty =
        AvaloniaProperty.Register<PositionableSubControl, double>(nameof(Y),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: 0.0d);
    
    public double Scale { get; set; }
    
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

    #endregion
    
    #region Rendering

    

    #endregion
    
    #region Event handling

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
	    if (e.Property == ScaleProperty)
	    {
		    double scaleDifference = 1.0d + ((double) e.NewValue!) - ((double) e.OldValue!);
		    
		    this.Width *=  scaleDifference;
		    this.Height *=  scaleDifference;
		    this.X *= scaleDifference;
		    this.Y *= scaleDifference;
	    }
	    
	    base.OnPropertyChanged(e);
    }

    #endregion
}