using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Styling;
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

    private ObservableCollection<HierarchyViewElement> _items = new();

    public ObservableCollection<HierarchyViewElement> Items
    {
        get => _items ??= new ObservableCollection<HierarchyViewElement>();
        set => SetAndRaise(ItemsProperty, ref _items, value);
    }

    public static readonly DirectProperty<HierarchyControl, ObservableCollection<HierarchyViewElement>> ItemsProperty =
        AvaloniaProperty.RegisterDirect<HierarchyControl, ObservableCollection<HierarchyViewElement>>(nameof(Items),
            control => control.Items, (control, items) => control.Items = items,
            defaultBindingMode: BindingMode.TwoWay);

    public ulong NetlistId
    {
        get => GetValue(NetlistIdProperty);
        set => SetValue(NetlistIdProperty, value);
    }

    public static readonly StyledProperty<ulong> NetlistIdProperty =
        AvaloniaProperty.Register<HierarchyControl, ulong>(nameof(NetlistId), defaultBindingMode: BindingMode.TwoWay);

    public double StepSize
    {
        get => GetValue(StepSizeProperty);
        set => SetValue(StepSizeProperty, value);
    }

    public static readonly StyledProperty<double> StepSizeProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(StepSize), defaultBindingMode: BindingMode.TwoWay);

    public double Scale
    {
        get => GetValue(ScaleProperty);
        set => SetValue(ScaleProperty, value);
    }

    public static readonly StyledProperty<double> ScaleProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(Scale), defaultValue: 1.0d,
            defaultBindingMode: BindingMode.TwoWay);

    public double OffsetX
    {
        get => GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }

    public static readonly StyledProperty<double> OffsetXProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(OffsetX), defaultValue: 0.0d,
            defaultBindingMode: BindingMode.TwoWay);

    public double OffsetY
    {
        get => GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    public static readonly StyledProperty<double> OffsetYProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(OffsetY), defaultValue: 0.0d,
            defaultBindingMode: BindingMode.TwoWay);

    public double DeltaX
    {
        get => GetValue(DeltaXProperty);
        set => SetValue(DeltaXProperty, value);
    }

    public static readonly StyledProperty<double> DeltaXProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(DeltaX), defaultValue: 0.0d,
            defaultBindingMode: BindingMode.TwoWay);

    public double DeltaY
    {
        get => GetValue(DeltaYProperty);
        set => SetValue(DeltaYProperty, value);
    }

    public static readonly StyledProperty<double> DeltaYProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(DeltaY), defaultValue: 0.0d,
            defaultBindingMode: BindingMode.TwoWay);

    public double DeltaScale
    {
        get => GetValue(DeltaScaleProperty);
        set => SetValue(DeltaScaleProperty, value);
    }

    public static readonly StyledProperty<double> DeltaScaleProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(DeltaScale), defaultValue: 0.0d,
            defaultBindingMode: BindingMode.TwoWay);

    public double NodeScaleClip
    {
        get => GetValue(NodeScaleClipProperty);
        set => SetValue(NodeScaleClipProperty, value);
    }

    public static readonly StyledProperty<double> NodeScaleClipProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(NodeScaleClip),
            defaultBindingMode: BindingMode.TwoWay);

    public double PortScaleClip
    {
        get => GetValue(PortScaleClipProperty);
        set => SetValue(PortScaleClipProperty, value);
    }

    public static readonly StyledProperty<double> PortScaleClipProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(PortScaleClip),
            defaultBindingMode: BindingMode.TwoWay);

    public double LabelScaleClip
    {
        get => GetValue(LabelScaleClipProperty);
        set => SetValue(LabelScaleClipProperty, value);
    }

    public static readonly StyledProperty<double> LabelScaleClipProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(LabelScaleClip),
            defaultBindingMode: BindingMode.TwoWay);

    public double EdgeScaleClip
    {
        get => GetValue(EdgeScaleClipProperty);
        set => SetValue(EdgeScaleClipProperty, value);
    }

    public static readonly StyledProperty<double> EdgeScaleClipProperty =
        AvaloniaProperty.Register<HierarchyControl, double>(nameof(EdgeScaleClip),
            defaultBindingMode: BindingMode.TwoWay);
    
    #endregion

    #region Variables

    private bool _pointerPressed = false;
    private Point _pointerPosition = new Point(0, 0);
    private Typeface? _typeface;

    #endregion

    public HierarchyControl()
    {
        WeakReferenceMessenger.Default.Register<ZoomToFitmessage, int>(this,
            FentwumSNetlistViewerSettingsHelper.HierarchyMessageChannel, (recipient, message) =>
            {
                if (message.Value == NetlistId)
                {
                    ZoomToFit();
                }
            });
        
        WeakReferenceMessenger.Default.Register<ZoomToToplevelMessage, int>(this,
            FentwumSNetlistViewerSettingsHelper.HierarchyMessageChannel, (recipient, message) =>
            {
                if (message.Value == NetlistId)
                {
                    ZoomToToplevel();
                }
            });

        PointerPressed += HierarchyControl_PointerPressed;
        PointerReleased += HierarchyControl_PointerReleased;
        PointerMoved += HierarchyControl_PointerMoved;
        PointerWheelChanged += HierarchyControl_PointerWheelChanged;
        Tapped += HierarchyControl_Tapped;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (IsInitialized && Scale == 0)
        {
            Scale = 0.4;
        }

        if (e.Property == FontFamilyProperty)
        {
            _typeface = new Typeface(this.FontFamily, FontStyle.Normal, FontWeight.Regular, FontStretch.Normal);
            ;
        } else if (e.Property == DeltaXProperty || e.Property == DeltaYProperty || e.Property == DeltaScaleProperty)
        {
            Redraw();
        }
        
        Redraw();

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

        _typeface ??= new Typeface(this.FontFamily, FontStyle.Normal, FontWeight.Regular, FontStretch.Normal);

        double height,
            width,
            x,
            y;

        Rect drawnRect;
        Geometry drawnGeometry;

        double dropshadowThickness = 2.5d * Scale,
            deltaX = DeltaX,
            deltaY = DeltaY,
            deltaScale = DeltaScale,
            step = 0.9d;
        
        OffsetX += deltaX;
        OffsetY += deltaY;

        if (deltaScale != 0)
        {
            if (deltaScale > 0)
            {
                step = 1.1d;
            }
            
            double allSteps = Math.Pow(step, Math.Abs(deltaScale));
            
            // Apply rescale
            Scale *= allSteps;
            
            OffsetX *= allSteps;
            OffsetY *= allSteps;

            OffsetX += _pointerPosition.X - _pointerPosition.X * allSteps;
            OffsetY += _pointerPosition.Y - _pointerPosition.Y * allSteps;
        }
        
        DeltaX -= deltaX;
        DeltaY -= deltaY;
        DeltaScale -= deltaScale;

        #region Brushes

        ThemeVariant theme = Application.Current!.ActualThemeVariant;
        Brush backgroundBrush =
            new SolidColorBrush(Application.Current!.FindResource(theme, "ThemeBackgroundColor") is Color
                ? (Color)Application.Current!.FindResource(theme, "ThemeBackgroundColor")!
                : Colors.LightGray);
        Pen highlightPen = new Pen(new SolidColorBrush(Colors.Yellow, 0.5d), 5.5 * Scale, null, PenLineCap.Round,
            PenLineJoin.Miter, 10d);
        Pen notConnectedPen = new Pen(new SolidColorBrush(Colors.Red), 2 * Scale, null, PenLineCap.Round);

        Pen borderPen = new Pen(
            Application.Current!.FindResource(theme, "ThemeBorderMidBrush") as IBrush ??
            new SolidColorBrush(Colors.MidnightBlue),
            1.5 * Scale);
        Pen symbolPen = new Pen(
            Application.Current!.FindResource(theme, "ThemeBorderMidBrush") as IBrush ??
            new SolidColorBrush(Colors.MidnightBlue),
            0.1d * Scale);
        Pen dropShadowPen =
            new Pen(
                new SolidColorBrush((Application.Current.FindResource(theme, "ThemeBorderHighColor") is Color
                    ? (Color)(Application.Current.FindResource(theme, "ThemeBorderHighColor") ??
                              new Color(0xFF, 0xA0, 0xA0, 0xA0))
                    : Colors.DarkGray)),
                dropshadowThickness, null, PenLineCap.Square);
        Pen edgePen = new Pen(
            Application.Current.FindResource(theme, "ThemeAccentBrush") as IBrush ?? new SolidColorBrush(Colors.Black),
            1.2 * Scale, null, PenLineCap.Square);
        Pen bundledEdgePen = new Pen(
            Application.Current.FindResource(theme, "ThemeAccentBrush") as IBrush ?? new SolidColorBrush(Colors.Black),
            2.8 * Scale, null, PenLineCap.Square);
        Brush rectFillBrush =
            Application.Current!.FindResource(theme, "ThemeControlHighlightMidBrush") as SolidColorBrush ??
            new SolidColorBrush(Colors.LightBlue);
        Brush symbolFillBrush =
            Application.Current!.FindResource(theme, "ThemeBorderMidBrush") as SolidColorBrush ??
            new SolidColorBrush(Colors.Black);
        Brush ellipseFillBrush =
            Application.Current!.FindResource(theme, "ThemeAccentBrush") as SolidColorBrush ??
            new SolidColorBrush(Colors.Black);
        Brush textBrush = new SolidColorBrush(Application.Current!.FindResource(theme, "ThemeAccentColor") is Color
            ? (Color)(Application.Current!.FindResource(theme, "ThemeAccentColor") ?? new Color(0xFF, 0x00, 0x7A, 0xB8))
            : Colors.Black);

        #endregion
        
        // Draw background
        context.DrawRectangle(backgroundBrush, null, new Rect(0, 0, this.Bounds.Width, this.Bounds.Height));

        try
        {
            foreach (HierarchyViewElement element in Items)
            {
                if (element is HierarchyViewNode node)
                {
                    height = node.Height * Scale;
                    width = node.Width * Scale;

                    x = (node.X + node.Width / 2) * Scale;
                    y = (node.Y + node.Height / 2) * Scale;

                    x -= width / 2;
                    y -= height / 2;
                    
                    x += OffsetX;
                    y += OffsetY;

                    drawnRect = new Rect(x, y, width, height);

                    if ((NodeScaleClip < height && NodeScaleClip < width) &&
                        (drawnRect.Contains(this.Bounds) || drawnRect.Intersects(this.Bounds) ||
                         this.Bounds.Contains(drawnRect)))
                    {
                        context.DrawRectangle(rectFillBrush, borderPen, drawnRect);
                    }
                }
                else if (element is HierarchyViewPort port)
                {
                    if (port.Geometry is null)
                    {
                        continue;
                    }
                    
                    height = port.Height * Scale;
                    width = port.Width * Scale;

                    x = (port.X + port.Width / 2) * Scale;
                    y = (port.Y + port.Height / 2) * Scale;

                    x -= width / 2;
                    y -= height / 2;
                    
                    x += OffsetX;
                    y += OffsetY;

                    drawnRect = new Rect(x, y, width, height);

                    if ((PortScaleClip < width && PortScaleClip < height) && (drawnRect.Contains(this.Bounds) || drawnRect.Intersects(this.Bounds) ||
                        this.Bounds.Contains(drawnRect)))
                    {

                        drawnGeometry = port.Geometry.Clone();

                        // TODO: Scale factor needs to be determined
                        drawnGeometry.Transform =
                            new MatrixTransform(new Matrix(Scale * 0.3d, 0.0d, 0.0d, Scale * 0.3d, x, y));
                        context.DrawGeometry(symbolFillBrush, symbolPen, drawnGeometry);
                    }
                }
                else if (element is HierarchyViewEdge edge)
                {
                    if (edge.Points is null)
                    {
                        continue;
                    }
                    
                    List<Point> points = new List<Point>();

                    bool previousPointInBounds = true;
                    bool currentPointInBounds = false;
                    bool drawLine = true;

                    foreach (Point point in edge.Points)
                    {
                        points.Add(new Point((point.X + edge.X) * Scale + OffsetX,
                            (point.Y + edge.Y) * Scale + OffsetY));
                    }
                    
                    for (int i = 1; i < points.Count; i++)
                    {
                        drawLine = false;
                        currentPointInBounds = Bounds.Contains(points[i]);

                        if ((points[i - 1].X - points[i].X) * (points[i - 1].X - points[i].X) +
                            (points[i - 1].Y - points[i].Y) * (points[i - 1].Y - points[i].Y) <=
                            EdgeScaleClip * EdgeScaleClip)
                        {
                            continue;
                        }

                        double lx = 0.0d,
                            ty = 0.0d,
                            rx = 0.0d,
                            by = 0.0d;

                        if (points[i].X < points[i - 1].X)
                        {
                            lx = points[i].X;
                            rx = points[i - 1].X;
                        }
                        else
                        {
                            lx = points[i - 1].X;
                            rx = points[i].X;
                        }

                        if (points[i].Y < points[i - 1].Y)
                        {
                            ty = points[i].Y;
                            by = points[i - 1].Y;
                        }
                        else
                        {
                            ty = points[i - 1].Y;
                            by = points[i].Y;
                        }
                        
                        drawnRect = new Rect(new Point(lx, ty), new Point(rx, by));

                        if (previousPointInBounds)
                        {
                            drawLine = true;
                        }
                        else if (currentPointInBounds)
                        {
                            drawLine = true;
                        }
                        else if (lx < Bounds.Right && rx > Bounds.Left && ty < Bounds.Bottom && ty > Bounds.Top && by < Bounds.Bottom && by > Bounds.Top
                                 || ty < Bounds.Bottom && by > Bounds.Top && lx > Bounds.Left && lx < Bounds.Right && rx > Bounds.Left && rx < Bounds.Right)
                        {
                            drawLine = true;
                        }

                        previousPointInBounds = currentPointInBounds;

                        if (drawLine)
                        {
                            context.DrawLine(edgePen, points[i - 1], points[i]);
                        }
                    }
                }
                else if (element is HierarchyViewLabel label)
                {
                    x = (label.X + label.Width / 2) * Scale;
                    y = (label.Y + label.Height / 2) * Scale;

                    height = label.Height * Scale;
                    width = label.Width * Scale;

                    x -= width / 2;
                    y -= height / 2;
                    
                    x += OffsetX;
                    y += OffsetY;
                    
                    drawnRect = new Rect(x, y, width, height);

                    if ((LabelScaleClip < height && LabelScaleClip < width) && (drawnRect.Contains(this.Bounds) || drawnRect.Intersects(this.Bounds) ||
                        this.Bounds.Contains(drawnRect)))
                    {

                        FormattedText labeltext = new FormattedText(label.Content, CultureInfo.InvariantCulture,
                            FlowDirection.LeftToRight, (Typeface)_typeface, label.FontSize * Scale, textBrush);

                        context.DrawText(labeltext, new Point(x, y));
                    }
                }
            }
        }
        catch
        {
        }
    }

    #endregion

    #region Message handlers

    private void ZoomToFit()
    {
        IHierarchyInformationService _hierarchyInformationService = ServiceManager.GetService<IHierarchyInformationService>();

        double sx = Bounds.Width / _hierarchyInformationService.getMaxWidth(NetlistId);
        double sy = Bounds.Height / _hierarchyInformationService.getMaxHeight(NetlistId);

        if (sx < sy)
        {
            Scale = sx;

            OffsetX = 0;
            OffsetY = ((_hierarchyInformationService.getMaxHeight(NetlistId) / 2) * -Scale) + (Bounds.Height / 2);
        }
        else
        {
            Scale = sy;
            
            OffsetX = ((_hierarchyInformationService.getMaxWidth(NetlistId) / 2) * -Scale) + (Bounds.Width / 2);
            OffsetY = 0;
        }
    }

    private void ZoomToToplevel()
    {
        IHierarchyInformationService _hierarchyInformationService = ServiceManager.GetService<IHierarchyInformationService>();
        double sx = Bounds.Width / _hierarchyInformationService.getTopWidth(NetlistId);
        double sy = Bounds.Height / _hierarchyInformationService.getTopHeight(NetlistId);

        if (sx < sy)
        {
            Scale = sx;
            
            OffsetX = _hierarchyInformationService.getTopX(NetlistId) * -Scale;
            OffsetY = _hierarchyInformationService.getTopY(NetlistId) * -Scale + ((_hierarchyInformationService.getTopHeight(NetlistId) / 2) * -Scale) + (Bounds.Height / 2);
        }
        else
        {
            Scale = sy;
            
            OffsetX = _hierarchyInformationService.getTopX(NetlistId) * -Scale + ((_hierarchyInformationService.getTopWidth(NetlistId) / 2) * -Scale) + (Bounds.Width / 2);
            OffsetY = _hierarchyInformationService.getTopY(NetlistId) * -Scale;
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