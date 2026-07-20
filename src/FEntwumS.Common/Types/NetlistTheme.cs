using Avalonia.Media;

namespace FEntwumS.Common.Types;

public struct NetlistTheme
{
    public Typeface Typeface { get; set; } = default;

    #region Brushes

    public Brush BackGroundBrush { get; set; } = new SolidColorBrush(Colors.White);
    public Brush BorderBrush { get; set; } = new SolidColorBrush(Colors.Black);
    public Brush DropShadowBrush { get; set; } = new SolidColorBrush(Colors.Black);
    public Brush EdgeBrush { get; set; } = new SolidColorBrush(Colors.Black);
    public Brush FillBrush { get; set; } = new SolidColorBrush(Colors.Gray);
    public Brush TextBrush { get; set; } = new SolidColorBrush(Colors.Black);
    public Brush HighlightBrush { get; set; } = new SolidColorBrush(Colors.Yellow);
    public Brush NotConnectedBrush { get; set; } = new SolidColorBrush(Colors.Gray);
    
    #endregion

    #region Pens

    public Pen BorderPen { get; set; } = new Pen(new SolidColorBrush(Colors.Black), 1);
    public Pen DropShadowPen { get; set; } = new Pen(new SolidColorBrush(Colors.Gray), 1);
    public Pen EdgePen { get; set; } = new Pen(new SolidColorBrush(Colors.Black), 1);
    public Pen BundledEdgePen { get; set; } = new Pen(new SolidColorBrush(Colors.Black), 2);
    public Pen NotConnectedPen { get; set; } = new Pen(new SolidColorBrush(Colors.Gray), 1);
    public Pen HighLightPen { get; set; } = new Pen(new SolidColorBrush(Colors.Yellow), 3);

    #endregion
    
    #region Colors
    
    public Color BackgroundColor { get; set; } = Color.FromRgb(255, 255, 255);
    public Color BorderColor { get; set; } = Color.FromRgb(0, 0, 0);
    public Color DropShadowColor { get; set; } = Color.FromRgb(128, 128, 128);
    public Color EdgeColor { get; set; } = Color.FromRgb(0, 0, 0);
    public Color FillColor { get; set; } = Color.FromArgb(200, 48, 48, 48);
    public Color TextColor { get; set; } = Color.FromRgb(0, 0, 0);
    public Color HighlightColor { get; set; } = Color.FromRgb(255, 255, 0);
    public Color NotConnectedColor { get; set; } = Color.FromRgb(80, 80, 80);
    
    #endregion
    
    #region Sizes

    public double BorderThickness = 1.5d;
    public double DropShadowThickness = 2.5d;
    public double EdgeThickness = 1.2d;
    public double BundledEdgeThickness = 2.8d;
    public double NotConnectedThickness = 2.0d;
    public double HighlightThickness = 5.0d;

    public NetlistTheme()
    {
    }

    #endregion

    public void RegenerateBrushesAndPens(double scale = 1.0)
    {
        // First, update brushes
        
        BackGroundBrush = new SolidColorBrush(BackgroundColor);
        BorderBrush = new SolidColorBrush(BorderColor);
        DropShadowBrush = new SolidColorBrush(DropShadowColor);
        EdgeBrush = new SolidColorBrush(EdgeColor);
        FillBrush = new SolidColorBrush(FillColor);
        TextBrush = new SolidColorBrush(TextColor);
        HighlightBrush = new SolidColorBrush(HighlightColor);
        NotConnectedBrush = new SolidColorBrush(NotConnectedColor);
        
        // Then, update pens with appropriate scale
        BorderPen = new Pen(BorderBrush, BorderThickness * scale);
        DropShadowPen = new Pen(DropShadowBrush, DropShadowThickness * scale);
        EdgePen = new Pen(EdgeBrush, EdgeThickness * scale);
        BundledEdgePen = new Pen(BorderBrush, BundledEdgeThickness * scale);
        NotConnectedPen = new Pen(NotConnectedBrush, NotConnectedThickness * scale);
        HighLightPen = new Pen(HighlightBrush, HighlightThickness * scale);
        
    }
    
}