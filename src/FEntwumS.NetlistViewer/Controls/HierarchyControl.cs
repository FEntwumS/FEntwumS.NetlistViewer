using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Threading;
using FEntwumS.NetlistViewer.Types.HierarchyView;

namespace FEntwumS.NetlistViewer.Controls;

public class HierarchyControl : TemplatedControl, ICustomHitTest
{
    #region Properties

    private ObservableCollection<HierarchyViewElement>? _items = new();

    public ObservableCollection<HierarchyViewElement> Items
    {
        get => _items ??= new ObservableCollection<HierarchyViewElement>();
        set => _items = value;
    }

    public static readonly DirectProperty<HierarchyControl, ObservableCollection<HierarchyViewElement>> ItemsProperty =
        AvaloniaProperty.RegisterDirect<HierarchyControl, ObservableCollection<HierarchyViewElement>>(nameof(Items),
            control => control.Items, (control, items) => control.Items = items,
            defaultBindingMode: BindingMode.TwoWay);

    private double _stepSize { get; set; }

    public double StepSize
    {
        get => _stepSize;
        set => _stepSize = value;
    }

    public static readonly StyledProperty<double> StepSizeProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(StepSize), defaultBindingMode: BindingMode.TwoWay);

    private double _scale { get; set; }

    public double Scale
    {
        get => _scale;
        set => _scale = value;
    }

    public static readonly StyledProperty<double> ScaleProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(Scale), defaultValue: 1.0d,
            defaultBindingMode: BindingMode.TwoWay);

    private double _offsetX { get; set; }

    public double OffsetX
    {
        get => _offsetX;
        set => _offsetX = value;
    }

    public static readonly StyledProperty<double> OffsetXProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(OffsetX), defaultValue: 0.0d,
            defaultBindingMode: BindingMode.TwoWay);

    private double _offsetY { get; set; }

    public double OffsetY
    {
        get => _offsetY;
        set => _offsetY = value;
    }

    public static readonly StyledProperty<double> OffsetYProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(OffsetY), defaultValue: 0.0d,
            defaultBindingMode: BindingMode.TwoWay);

    private double _deltaX { get; set; }

    public double DeltaX
    {
        get => _deltaX;
        set => _deltaX = value;
    }

    public static readonly StyledProperty<double> DeltaXProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(DeltaX), defaultValue: 0.0d,
            defaultBindingMode: BindingMode.TwoWay);

    private double _deltaY { get; set; }

    public double DeltaY
    {
        get => _deltaY;
        set => _deltaY = value;
    }

    public static readonly StyledProperty<double> DeltaYProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(DeltaY), defaultValue: 0.0d,
            defaultBindingMode: BindingMode.TwoWay);

    private double _deltaScale { get; set; }

    public double DeltaScale
    {
        get => _deltaScale;
        set => _deltaScale = value;
    }

    public static readonly StyledProperty<double> DeltaScaleProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(DeltaScale), defaultValue: 0.0d,
            defaultBindingMode: BindingMode.TwoWay);

    #endregion

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
    }

    public void Redraw()
    {
        _ = Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Render);
    }

    public override void Render(DrawingContext context)
    {
        if (!IsInitialized)
        {
            return;
        }
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}