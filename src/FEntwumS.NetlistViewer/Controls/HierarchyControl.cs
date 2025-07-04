using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using FEntwumS.NetlistViewer.Helpers;
using FEntwumS.NetlistViewer.Services;
using FEntwumS.NetlistViewer.Types.HierarchyView;
using FEntwumS.NetlistViewer.Types.Messages;

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

    private ulong _netlistId { get; set; }

    public ulong NetlistId
    {
        get => _netlistId;
        set => _netlistId = value;
    }

    public static readonly StyledProperty<ulong> NetlistIdProperty =
        AvaloniaProperty.Register<HierarchyControl, ulong>(nameof(NetlistId), defaultBindingMode: BindingMode.TwoWay);

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

    #region Variables

    private bool _pointerPressed = false;
    private Point _pointerPosition = new Point(0, 0);

    #endregion

    public HierarchyControl()
    {
        WeakReferenceMessenger.Default.Register<ZoomToFitmessage, int>(this,
            FentwumSNetlistViewerSettingsHelper.HierarchyMessageChannel, (recipient, message) => { ZoomToFit(); });
        
        PointerPressed += HierarchyControl_PointerPressed;
        PointerReleased += HierarchyControl_PointerReleased;
        PointerMoved += HierarchyControl_PointerMoved;
        PointerWheelChanged += HierarchyControl_PointerWheelChanged;
        Tapped += HierarchyControl_Tapped;
    }

    private void ZoomToFit()
    {
        ServiceManager.GetService<ICustomLogger>().Log("HierarchyControl: Received ZoomToFit Message");
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
    }

    #region Rendering

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

    #endregion

    #region Event handlers

    private void HierarchyControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        PointerPoint currentPoint = e.GetCurrentPoint(this);
        
        _pointerPressed = currentPoint.Properties.IsLeftButtonPressed;
        
        _pointerPosition = currentPoint.Position;
    }

    private void HierarchyControl_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        PointerPoint currentPoint = e.GetCurrentPoint(this);
        
        _pointerPressed = currentPoint.Properties.IsLeftButtonPressed;
        
        _pointerPosition = currentPoint.Position;
    }

    private void HierarchyControl_Tapped(object? sender, TappedEventArgs e)
    {
        ServiceManager.GetService<ICustomLogger>().Log("HierarchyControl: Tapped");
    }

    private void HierarchyControl_PointerMoved(object? sender, PointerEventArgs e)
    {
        IReadOnlyList<PointerPoint> pointerPoints = e.GetIntermediatePoints(this);
        double dx = 0,
            dy = 0;
        
        Point currentPos = e.GetPosition(this);

        if (_pointerPressed || pointerPoints.First().Properties.IsLeftButtonPressed)
        {
            dx = currentPos.X - _pointerPosition.X;
            dy = currentPos.Y - _pointerPosition.Y;
        }
        
        _pointerPosition = currentPos;

        DeltaX += dx;
        DeltaY += dy;
    }

    private void HierarchyControl_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        Vector wheel = e.Delta;

        DeltaScale += wheel.Y;
        
        _pointerPosition = e.GetPosition(this);
    }

    #endregion

    #region ICustomHitTest implementation

    public bool HitTest(Point point)
    {
        return true;
    }

    #endregion
    
}