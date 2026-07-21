using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Media;

namespace FEntwumS.Common.Controls;

public class GraphLabelControl : GenericGraphElementControl
{
    #region Properties

    public string? Content { get; set; }
    
    public static readonly StyledProperty<string> ContentProperty =
        AvaloniaProperty.Register<GraphLabelControl, string>(nameof(Content),
            defaultBindingMode: BindingMode.TwoWay);
    
    public double Fontsize { get; set; }
    
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
        new Typeface(FontFamily.Default),
        10,
        null);

    #endregion

    #region Event handling

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ContentProperty)
        {
            if (e.NewValue is string newContent)
            {
                _formattedContent = new FormattedText(newContent,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    NetlistTheme.Typeface,
                    Fontsize * Scale,
                    NetlistTheme.TextBrush);
            }
        } else if (e.Property == NetlistThemeProperty)
        {
	        _formattedContent = new FormattedText(Content ?? "",
		        CultureInfo.InvariantCulture,
		        FlowDirection.LeftToRight,
		        NetlistTheme.Typeface,
		        Fontsize * Scale,
		        NetlistTheme.TextBrush);
        }
        
        base.OnPropertyChanged(e);
    }

    #endregion
    
    #region Rendering

    public override void Render(DrawingContext context)
    {
	    context.DrawText(_formattedContent, new Point(X * Scale, Y * Scale));
    }

    #endregion
}