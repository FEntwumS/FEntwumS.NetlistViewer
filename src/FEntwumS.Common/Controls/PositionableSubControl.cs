using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace FEntwumS.Common.Controls;

public abstract class PositionableSubControl : UserControl
{
    #region Properties

    private double _x = 0.0d;
    
    public double X
    {
	    get => _x;
	    set => _x = value;
    }
    
    private double _y = 0.0d;

    public double Y
    {
	    get => _y;
	    set => _y = value;
    }

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