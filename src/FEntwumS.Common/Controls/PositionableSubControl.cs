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

    private double _elementWidth = 1.0d;

    public double ElementWidth
    {
	    get => _elementWidth;
	    set => _elementWidth = value;
    }
    
    private double _elementHeight = 1.0d;

    public double ElementHeight
    {
	    get => _elementHeight;
	    set => _elementHeight = value;
    }
    
    private double _scale = 1.0d;

    public double Scale
    {
	    get => _scale;
	    set
	    {
		    UpdateScale(value);
		    _scale = value;
	    }
    }

    #endregion
    
    #region Rendering

    

    #endregion
    
    #region Event handling

    protected virtual void UpdateScale(double newScale)
    {
	    double scaleDifference = newScale / _scale;
		    
	    this.ElementWidth *=  scaleDifference;
	    this.ElementHeight *=  scaleDifference;
	    this.X *= scaleDifference;
	    this.Y *= scaleDifference;
		    
	    RegenerateDrawnElements(newScale);
    }

    protected abstract void RegenerateDrawnElements(double newScale);

    #endregion
}