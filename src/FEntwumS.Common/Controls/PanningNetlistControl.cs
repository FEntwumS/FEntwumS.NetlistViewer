using Avalonia;
using Avalonia.Data;
using Avalonia.Media;

namespace FEntwumS.Common.Controls;

public class PanningNetlistControl : PanningControl
{
    #region Properties

    public ulong NetlistId
    {
        get => GetValue(NetlistIdProperty);
        set => SetValue(NetlistIdProperty, value);
    }
    
    public static readonly StyledProperty<ulong> NetlistIdProperty =
        AvaloniaProperty.Register<PanningNetlistControl, ulong>(nameof(NetlistId),
            defaultBindingMode: BindingMode.TwoWay);

    #endregion
    
    #region Rendering

    public override void Render(DrawingContext context)
    {
        context.DrawRectangle(Brushes.White, null, Bounds);
    }
    
    #endregion
}