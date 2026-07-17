using Avalonia.Media;

namespace FEntwumS.Common.Types;

public struct NetlistTheme
{
    public Typeface Typeface { get; set; }
    
    #region Brushes
    
    public Brush BackGroundBrush { get; set; }
    public Brush BorderBrush { get; set; }
    public Brush DropShadowBrush { get; set; }
    public Brush EdgeBrush { get; set; }
    public Brush FillBrush { get; set; }
    public Brush TextBrush { get; set; }
    public Brush HighlightBrush { get; set; }
    
    #endregion

    #region Pens

    public Pen BorderPen { get; set; }
    public Pen DropShadowPen { get; set; }
    public Pen EdgePen { get; set; }
    public Pen BundledEdgePen { get; set; }
    public Pen NotConnectedPen { get; set; }
    public Pen HighLightPen { get; set; }

    #endregion
    
    #region Colors
    
    #endregion

    public void RegenerateBrushesAndPens(double scale = 1.0)
    {
        
    }
    
}