using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Media;

namespace FEntwumS.Common.Controls;

public class GraphLabelControl : GenericGraphElementControl
{
    #region Properties

    public string? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value ?? "");
    }
    
    public static readonly StyledProperty<string> ContentProperty =
        AvaloniaProperty.Register<GraphLabelControl, string>(nameof(Content),
            defaultBindingMode: BindingMode.TwoWay);

    public double Fontsize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }
    
    public static readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<GraphLabelControl, double>(nameof(Fontsize),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: 10.0d,
            enableDataValidation: true,
            validate: d => d > 0.0d);

    #endregion

    #region Variables

    private FormattedText _formattedContent = new FormattedText("",
        CultureInfo.InvariantCulture,
        FlowDirection.LeftToRight,
        new Typeface(new FontFamily("avares://FEntwumS.Common/Assets/Fonts#Martian Mono Std Rg")),
        10,
        Brushes.Black);

    #endregion

    #region Event handling

    #endregion
    
    #region Rendering

    public override void Render(DrawingContext context)
    {
	    if (Scale >= 0.2d)
	    {
		    context.DrawText(_formattedContent, new Point(0, 0));
	    }
    }

    protected override void RegenerateDrawnElements(double newScale)
    {
	    _formattedContent = new FormattedText(Content ?? "",
		    CultureInfo.InvariantCulture,
		    FlowDirection.LeftToRight,
		    NetlistTheme.Typeface,
		    Fontsize * newScale,
		    NetlistTheme.TextBrush);
    }

    #endregion
}