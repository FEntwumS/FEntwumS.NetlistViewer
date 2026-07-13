using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace FEntwumS.Common.Controls;

public class MovableSubControl : TemplatedControl
{
    #region Properties

    private double X { get; set; }

    public static readonly StyledProperty<double> XProperty =
        AvaloniaProperty.Register<MovableSubControl, double>(nameof(X),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: 0.0d);

    private double Y { get; set; }

    public static readonly StyledProperty<double> YProperty =
        AvaloniaProperty.Register<MovableSubControl, double>(nameof(Y),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: 0.0d);

    private double Width { get; set; }

    public static readonly StyledProperty<double> WidthProperty =
        AvaloniaProperty.Register<MovableSubControl, double>(nameof(Width),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: 0.0d,
            enableDataValidation: true, validate: d => d >= 0.0d);

    private double Height { get; set; }

    public static readonly StyledProperty<double> HeightProperty =
        AvaloniaProperty.Register<MovableSubControl, double>(nameof(Height),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: 0.0d,
            enableDataValidation: true, validate: d => d >= 0.0d);

    private IEnumerable<MovableSubControl> _children = new AvaloniaList<MovableSubControl>();

    public static readonly DirectProperty<MovableSubControl, IEnumerable<MovableSubControl>> ChildrenProperty =
        AvaloniaProperty.RegisterDirect<MovableSubControl, IEnumerable<MovableSubControl>>(nameof(_children),
            control => control._children,
            (control, children) => control._children = children);

    #endregion
}