using System.Collections;
using System.Globalization;
using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Styling;
using Avalonia.Threading;
using FEntwumS.Common.Services;
using FEntwumS.Common.Types;
using FEntwumS.NetlistViewer.Services;
using OneWare.Essentials.Helpers;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;

namespace FEntwumS.NetlistViewer.Controls;

public class NetlistControl : TemplatedControl, ICustomHitTest
{
    #region Properties

    public static readonly DirectProperty<NetlistControl, IEnumerable> ItemsProperty =
        AvaloniaProperty.RegisterDirect<NetlistControl, IEnumerable>(
            nameof(Items),
            o => o.Items,
            (o, v) => o.Items = v, defaultBindingMode: BindingMode.TwoWay);

    private IEnumerable _items = new AvaloniaList<object>();

    private AvaloniaList<object?> _renderableItems = new();

    public IEnumerable Items
    {
        get => _items;
        set => SetAndRaise(ItemsProperty, ref _items, value);
    }

    public static readonly StyledProperty<double> StepSizeProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(StepSize), defaultBindingMode: BindingMode.TwoWay);

    public double StepSize
    {
        get => GetValue(StepSizeProperty);
        set => SetValue(StepSizeProperty, value);
    }

    public static readonly StyledProperty<double> DiagramWidthProperty = AvaloniaProperty.Register<NetlistControl,
        double>(nameof(DiagramWidth), defaultBindingMode: BindingMode.TwoWay);

    public double DiagramWidth
    {
        get => GetValue(DiagramWidthProperty);
        set => SetValue(DiagramWidthProperty, value);
    }

    public static readonly StyledProperty<double> DiagramHeightProperty = AvaloniaProperty.Register<NetlistControl,
        double>(nameof(DiagramHeight), defaultBindingMode: BindingMode.TwoWay);

    public double DiagramHeight
    {
        get => GetValue(DiagramHeightProperty);
        set => SetValue(DiagramHeightProperty, value);
    }

    public static readonly StyledProperty<double> CurrentScaleProperty = AvaloniaProperty.Register<NetlistControl,
        double>(nameof(CurrentScale), defaultBindingMode: BindingMode.TwoWay);

    public double CurrentScale
    {
        get => GetValue(CurrentScaleProperty);
        set => SetValue(CurrentScaleProperty, value);
    }

    public static readonly StyledProperty<double> OffsetXProperty = AvaloniaProperty.Register<NetlistControl, double>
        (nameof(OffsetX), defaultBindingMode: BindingMode.TwoWay);

    public double OffsetX
    {
        get => GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }

    public static readonly StyledProperty<ICommand> OnClickCommandProperty =
        AvaloniaProperty.Register<NetlistControl, ICommand>(nameof(OnClickCommand),
            defaultBindingMode: BindingMode.TwoWay);

    public ICommand OnClickCommand
    {
        get => GetValue(OnClickCommandProperty);
        set => SetValue(OnClickCommandProperty, value);
    }

    public static readonly StyledProperty<double> OffsetYProperty = AvaloniaProperty.Register<NetlistControl, double>
        (nameof(OffsetY), defaultBindingMode: BindingMode.TwoWay);

    public double OffsetY
    {
        get => GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    public double DeltaX
    {
        get => GetValue(DeltaXProperty);
        set => SetValue(DeltaXProperty, value);
    }

    public static readonly StyledProperty<double> DeltaXProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(DeltaX), defaultBindingMode: BindingMode.TwoWay);

    public double DeltaY
    {
        get => GetValue(DeltaYProperty);
        set => SetValue(DeltaYProperty, value);
    }

    public static readonly StyledProperty<double> DeltaYProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(DeltaY), defaultBindingMode: BindingMode.TwoWay);

    public double DeltaScale
    {
        get => GetValue(DeltaScaleProperty);
        set => SetValue(DeltaScaleProperty, value);
    }

    public static readonly StyledProperty<double> DeltaScaleProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(DeltaScale), defaultBindingMode: BindingMode.TwoWay);

    private double currentDeltaScale = 0;

    public double PointerX
    {
        get => GetValue(PointerXProperty);
        set => SetValue(PointerXProperty, value);
    }

    public static readonly StyledProperty<double> PointerXProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(PointerX), defaultBindingMode: BindingMode.TwoWay);

    public double PointerY
    {
        get => GetValue(PointerYProperty);
        set => SetValue(PointerYProperty, value);
    }

    public static readonly StyledProperty<double> PointerYProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(PointerY), defaultBindingMode: BindingMode.TwoWay);

    public double PortScaleClip
    {
        get => GetValue(PortScaleClipProperty);
        set => SetValue(PortScaleClipProperty, value);
    }

    public static readonly StyledProperty<double> PortScaleClipProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(PortScaleClip),
            defaultBindingMode: BindingMode.TwoWay);

    public double NodeScaleClip
    {
        get => GetValue(NodeScaleClipProperty);
        set => SetValue(NodeScaleClipProperty, value);
    }

    public static readonly StyledProperty<double> NodeScaleClipProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(NodeScaleClip),
            defaultBindingMode: BindingMode.TwoWay);

    public double LabelScaleClip
    {
        get => GetValue(LabelScaleClipProperty);
        set => SetValue(LabelScaleClipProperty, value);
    }

    public static readonly StyledProperty<double> LabelScaleClipProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(LabelScaleClip),
            defaultBindingMode: BindingMode.TwoWay);

    public double EdgeLengthScaleClip
    {
        get => GetValue(EdgeLengthScaleClipProperty);
        set => SetValue(EdgeLengthScaleClipProperty, value);
    }

    public static readonly StyledProperty<double> EdgeLengthScaleClipProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(EdgeLengthScaleClip),
            defaultBindingMode: BindingMode.TwoWay);

    public double JunctionScaleClip
    {
        get => GetValue(JunctionScaleClipProperty);
        set => SetValue(JunctionScaleClipProperty, value);
    }

    public static readonly StyledProperty<double> JunctionScaleClipProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(JunctionScaleClip),
            defaultBindingMode: BindingMode.TwoWay);

    public bool FitToZoom
    {
        get => GetValue(FitToZoomProperty);
        set => SetValue(FitToZoomProperty, value);
    }

    public static readonly StyledProperty<bool> FitToZoomProperty =
        AvaloniaProperty.Register<NetlistControl, bool>(nameof(FitToZoom), defaultBindingMode: BindingMode.TwoWay);

    public new bool IsLoaded
    {
        get => GetValue(IsLoadedProperty);
        set => SetValue(IsLoadedProperty, value);
    }

    public static readonly StyledProperty<bool> IsLoadedProperty =
        AvaloniaProperty.Register<NetlistControl, bool>(nameof(IsLoaded), defaultBindingMode: BindingMode.TwoWay);

    public NetlistElement CurrentElement
    {
        get => GetValue(CurrentElementProperty);
        set => SetValue(CurrentElementProperty, value);
    }

    public static readonly StyledProperty<NetlistElement> CurrentElementProperty =
        AvaloniaProperty.Register<NetlistControl, NetlistElement>(nameof(CurrentElement),
            defaultBindingMode: BindingMode.TwoWay);

    public string? ClickedElementPath
    {
        get => GetValue(ClickedElementPathProperty);
        set => SetValue(ClickedElementPathProperty, value);
    }

    public static readonly StyledProperty<string?> ClickedElementPathProperty =
        AvaloniaProperty.Register<NetlistControl, string?>(nameof(ClickedElementPath),
            defaultBindingMode: BindingMode.TwoWay);

    public UInt64 NetlistID
    {
        get => GetValue(NetlistIDProperty);
        set => SetValue(NetlistIDProperty, value);
    }

    public static readonly StyledProperty<UInt64> NetlistIDProperty =
        AvaloniaProperty.Register<NetlistControl, UInt64>(nameof(NetlistID));

    public bool FileLoaded
    {
        get => GetValue(FileLoadedProperty);
        set => SetValue(FileLoadedProperty, value);
    }

    public static readonly StyledProperty<bool> FileLoadedProperty =
        AvaloniaProperty.Register<NetlistControl, bool>(nameof(FileLoaded));

    #endregion

    public event ElementClickedEventHandler ElementClicked;

    private readonly List<DRect> _renderedNodeList = new List<DRect>();
    private readonly List<DRect> _renderedLabelList = new List<DRect>();
    private readonly List<DRect> _renderedPortList = new List<DRect>();
    private readonly List<DCircle> _renderedJunctionList = new List<DCircle>();
    private readonly List<DLine> _renderedEdgeList = new List<DLine>();
    private static IViewportDimensionService? _viewportDimensionService;

    private Typeface? _typeface;

    private bool _itemsInvalidated = false;
    private bool _pointerPressed = false;
    private Point _pointerPosition = new Point(0, 0);

    public NetlistControl()
    {
        AffectsRender<NetlistControl>(ItemsProperty, IsEnabledProperty);

        _viewportDimensionService = ServiceManager.GetViewportDimensionService();

        PointerPressed += NetlistControl_PointerPressed;
        PointerReleased += NetlistControl_PointerReleased;
        PointerMoved += NetlistControl_PointerMoved;
        PointerWheelChanged += NetlistControl_OnPointerWheelChanged;
        Tapped += NetlistControl_OnTapped;

        ElementClicked += NetlistControl_OnElementClicked;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == OffsetXProperty ||
            change.Property == OffsetYProperty || change.Property == CurrentScaleProperty ||
            change.Property == IsPointerOverProperty || change.Property == FileLoadedProperty)
        {
            return;
        }

        if (change.Property == ItemsProperty && Items != null)
        {
            _itemsInvalidated = true;
            Redraw();
        }

        if (!IsInitialized)
        {
            return;
        }

        if (change.Property == IsLoadedProperty)
        {
            Redraw();
        }
        else if (change.Property == FitToZoomProperty)
        {
            ZoomToFit();
            Redraw();
        }
        else if (change.Property == DeltaScaleProperty)
        {
            Redraw();
        }
        else if (change.Property == NetlistIDProperty)
        {
            Redraw();
        }
        else if (change.Property == BoundsProperty)
        {
            Redraw();
        }
        else if (change.Property == FontFamilyProperty)
        {
            _typeface = new Typeface(this.FontFamily, FontStyle.Normal, FontWeight.Regular, FontStretch.Normal);
        }

        if (IsInitialized && CurrentScale == 0)
        {
            CurrentScale = 0.2;
        }

        Redraw();
        base.OnPropertyChanged(change);
    }

    public void ZoomToFit()
    {
        

        DRect? elementBounds = _viewportDimensionService!.GetZoomElementDimensions(NetlistID);

        if (elementBounds != null)
        {
            double sx = this.Bounds.Width / elementBounds.Width;
            double sy = this.Bounds.Height / elementBounds.Height;

            if (sx < sy)
            {
                CurrentScale = sx;

                OffsetX = elementBounds.X * -CurrentScale;
                OffsetY = ((elementBounds.Y + (elementBounds.Height / 2.0d)) * -CurrentScale) +
                          (this.Bounds.Height / 2.0d);
            }
            else
            {
                CurrentScale = sy;

                OffsetX = ((elementBounds.X + (elementBounds.Width / 2.0d)) * -CurrentScale) +
                          (this.Bounds.Width / 2.0d);
                OffsetY = elementBounds.Y * -CurrentScale;
            }

            _viewportDimensionService!.SetZoomElementDimensions(NetlistID, null);
        }
        else
        {
            double sx = this.Bounds.Width / _viewportDimensionService!.GetMaxWidth(NetlistID);
            double sy = this.Bounds.Height / _viewportDimensionService!.GetMaxHeight(NetlistID);

            if (sx < sy)
            {
                CurrentScale = sx;

                OffsetX = 0;
                OffsetY = ((_viewportDimensionService!.GetMaxHeight(NetlistID) / 2) * -CurrentScale) +
                          (this.Bounds.Height / 2.0d);
            }
            else
            {
                CurrentScale = sy;

                OffsetX = ((_viewportDimensionService!.GetMaxWidth(NetlistID) / 2) * -CurrentScale) + (this.Bounds.Width / 2.0d);
                OffsetY = 0;
            }
        }
    }

    public void Redraw()
    {
        _ = Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Render);
    }

    public override void Render(DrawingContext context)
    {
        // Skip if there is nothing to see
        if (!IsInitialized)
        {
            return;
        }

        if (_renderableItems.Count != ((AvaloniaList<NetlistElement>)_items).Count)
        {
            _renderableItems.Clear();
            _renderableItems.AddRange((AvaloniaList<NetlistElement>)_items);
        }

        _typeface ??= new Typeface(this.FontFamily, FontStyle.Normal, FontWeight.Regular, FontStretch.Normal);

        double x = 0, y = 0, width = 0, height = 0, radius = 0, edgeLength = 0;
        List<Point> points;

        bool previousNodeInView = true;
        int lastVisibleNodeZIndex = 0;

        _renderedNodeList.Clear();
        _renderedLabelList.Clear();
        _renderedPortList.Clear();
        _renderedJunctionList.Clear();
        _renderedEdgeList.Clear();

        #region Brushes

        ThemeVariant theme = Application.Current!.ActualThemeVariant;

        Brush backgroundBrush =
            new SolidColorBrush(Application.Current!.FindResource(theme, "ThemeBackgroundColor") is Color
                ? (Color)Application.Current!.FindResource(theme, "ThemeBackgroundColor")!
                : Colors.LightGray);
        Pen highlightPen = new Pen(new SolidColorBrush(Colors.Yellow, 0.5d), 5.5 * CurrentScale, null, PenLineCap.Round,
            PenLineJoin.Miter, 10d);

        Pen borderPen = new Pen(
            Application.Current!.FindResource(theme, "ThemeBorderMidBrush") as IBrush ??
            new SolidColorBrush(Colors.MidnightBlue),
            1.5 * CurrentScale);
        Pen dropShadowPen =
            new Pen(
                new SolidColorBrush((Application.Current.FindResource(theme, "ThemeBorderHighColor") is Color
                    ? (Color)(Application.Current.FindResource(theme, "ThemeBorderHighColor") ??
                              new Color(0xFF, 0xA0, 0xA0, 0xA0))
                    : Colors.DarkGray)),
                2.5 * CurrentScale, null, PenLineCap.Square);
        Pen edgePen = new Pen(
            Application.Current.FindResource(theme, "ThemeAccentBrush") as IBrush ?? new SolidColorBrush(Colors.Black),
            1.2 * CurrentScale, null, PenLineCap.Square);
        Pen bundledEdgePen = new Pen(
            Application.Current.FindResource(theme, "ThemeAccentBrush") as IBrush ?? new SolidColorBrush(Colors.Black),
            2.8 * CurrentScale, null, PenLineCap.Square);
        Brush rectFillBrush =
            Application.Current!.FindResource(theme, "ThemeControlHighlightMidBrush") as SolidColorBrush ??
            new SolidColorBrush(Colors.LightBlue);
        Brush ellipseFillBrush =
            Application.Current!.FindResource(theme, "ThemeAccentBrush") as SolidColorBrush ??
            new SolidColorBrush(Colors.Black);
        Brush textBrush = new SolidColorBrush(Application.Current!.FindResource(theme, "ThemeAccentColor") is Color
            ? (Color)(Application.Current!.FindResource(theme, "ThemeAccentColor") ?? new Color(0xFF, 0x00, 0x7A, 0xB8))
            : Colors.Black);

        #endregion

        // Draw background
        context.DrawRectangle(backgroundBrush, null, new Rect(0, 0, this.Bounds.Width, this.Bounds.Height));

        Rect rect;
        Rect boundingBox;
        Point center;
        Point start, bend, end;
        bool previousPointInBounds, currentPointInBounds;
        bool drawLine;

        double deltaX = DeltaX,
            deltaY = DeltaY,
            deltaScale = DeltaScale - currentDeltaScale,
            step = 0.9d; // Zoom factor for a single step (rotation by one mouse while notch)

        // Apply offset from user interaction
        OffsetX += deltaX;
        OffsetY += deltaY;

        DeltaX -= deltaX;
        DeltaY -= deltaY;
        currentDeltaScale += deltaScale;

        if (deltaScale != 0)
        {
            if (deltaScale > 0)
            {
                step = 1.1d;
            }

            double completeStep = Math.Pow(step, Math.Abs(deltaScale));

            // Apply scale
            CurrentScale *= completeStep;

            OffsetX *= completeStep;
            OffsetY *= completeStep;

            OffsetX += PointerX - PointerX * completeStep;
            OffsetY += PointerY - PointerY * completeStep;
        }

        if (!_itemsInvalidated)
        {
            foreach (NetlistElement? element in _renderableItems)
            {
                if (element is null) continue;
                switch (element.Type)
                {
                    // Node
                    case 1:
                        if (element.ZIndex > lastVisibleNodeZIndex + 1)
                        {
                            continue;
                        }


                        height = element.Height * CurrentScale;
                        width = element.Width * CurrentScale;
                        x = ((element.xPos + element.Width / 2)) * CurrentScale;
                        y = ((element.yPos + element.Height / 2)) * CurrentScale;

                        x -= width / 2;
                        y -= height / 2;

                        x += OffsetX;
                        y += OffsetY;

                        rect = new Rect(x, y, width, height);

                        if ((height >= NodeScaleClip && width >= NodeScaleClip) &&
                            (containsBounds(rect) || intersectsBounds(rect)))
                        {
                            context.DrawRectangle(rectFillBrush, borderPen, rect);

                            _renderedNodeList.Add(new DRect(x, y, width, height, element.ZIndex, element));

                            // Dropshadow

                            // border width + drop shadow width / 2
                            x += 2d * CurrentScale;
                            y += 2d * CurrentScale;

                            start = new Point(x, y + height);
                            bend = new Point(x + width, y + height);
                            end = new Point(x + width, y);

                            context.DrawLine(dropShadowPen, start, bend);
                            context.DrawLine(dropShadowPen, bend, end);

                            previousNodeInView = true;

                            lastVisibleNodeZIndex = element.ZIndex;
                        }
                        else
                        {
                            previousNodeInView = false;
                        }

                        break;

                    // Edge
                    case 2:
                        if (element.Points == null || !previousNodeInView || element.ZIndex > lastVisibleNodeZIndex + 1)
                        {
                            continue;
                        }

                        points = new List<Point>(element.Points.Count);

                        previousPointInBounds = true;
                        drawLine = false;

                        foreach (Point point in element.Points)
                        {
                            points.Add(new Point((point.X + element.xPos) * CurrentScale + OffsetX,
                                (point.Y + element.yPos) * CurrentScale + OffsetY));
                        }

                        for (int i = 1; i < points.Count; i++)
                        {
                            drawLine = false;
                            currentPointInBounds = isInBounds(points[i]);

                            if ((points[i - 1].X - points[i].X) * (points[i - 1].X - points[i].X) +
                                (points[i - 1].Y - points[i].Y) * (points[i - 1].Y - points[i].Y) <=
                                EdgeLengthScaleClip * EdgeLengthScaleClip)
                            {
                                continue;
                            }

                            if (previousPointInBounds)
                            {
                                drawLine = true;
                            }
                            else if (currentPointInBounds)
                            {
                                drawLine = true;
                            }
                            else if (intersectsBounds(points[i - 1], points[i]))
                            {
                                drawLine = true;
                            }

                            previousPointInBounds = currentPointInBounds;

                            if (drawLine)
                            {
                                context.DrawLine(
                                    element.SignalType == "BUNDLED" || element.SignalType == "BUNDLED_CONSTANT"
                                        ? bundledEdgePen
                                        : edgePen, points[i - 1], points[i]);

                                if (element.IsHighlighted && i != points.Count - 2)
                                {
                                    context.DrawLine(highlightPen, points[i - 1], points[i]);
                                }

                                _renderedEdgeList.Add(new DLine(points[i - 1], points[i], element.ZIndex, element));
                            }
                        }


                        break;

                    // Label
                    case 3:
                        // Port Labels are two levels higher than the corresponding node, therefore +2 instead of +1 is used

                        if (element.ZIndex > lastVisibleNodeZIndex + 2)
                        {
                            continue;
                        }

                        height = element.Height * CurrentScale;
                        width = element.Width * CurrentScale;
                        x = (element.xPos + (element.Width / 2)) * CurrentScale;
                        y = (element.yPos + (element.Height / 2)) * CurrentScale;

                        x -= width / 2;
                        y -= height / 2;

                        x += OffsetX;
                        y += OffsetY;

                        boundingBox = new Rect(x, y, width, height);

                        if (height >= LabelScaleClip &&
                            (intersectsBounds(boundingBox) || containsBounds(boundingBox)) &&
                            element.LabelText is not null)
                        {
                            FormattedText text = new FormattedText(element.LabelText, CultureInfo.InvariantCulture,
                                FlowDirection.LeftToRight, (Typeface)_typeface, element.FontSize * CurrentScale,
                                textBrush);

                            context.DrawText(text, new Point(x, y));

                            _renderedLabelList.Add(new DRect(x, y, width, height, element.ZIndex, element));
                        }

                        break;

                    // Junction
                    case 4:
                        if (element.ZIndex > lastVisibleNodeZIndex + 2)
                        {
                            continue;
                        }

                        x = element.xPos * CurrentScale;
                        y = element.yPos * CurrentScale;
                        radius = 3.8d * CurrentScale;

                        x += OffsetX;
                        y += OffsetY;

                        center = new Point(x, y);

                        if (radius * 2 >= JunctionScaleClip && isInBounds(center))
                        {
                            context.DrawEllipse(ellipseFillBrush, null, center, radius, radius);

                            _renderedJunctionList.Add(new DCircle(x, y, radius, element.ZIndex, element));
                        }

                        break;

                    // Port
                    case 5:
                        if (element.ZIndex > lastVisibleNodeZIndex + 1)
                        {
                            continue;
                        }

                        edgeLength = 10.0d * CurrentScale;
                        x = (element.xPos + 5.0d) * CurrentScale;
                        y = (element.yPos + 5.0d) * CurrentScale;

                        x += OffsetX;
                        y += OffsetY;

                        rect = new Rect(x - edgeLength / 2, y - edgeLength / 2, edgeLength, edgeLength);

                        if (edgeLength >= PortScaleClip && (intersectsBounds(rect) || containsBounds(rect)))
                        {
                            context.DrawRectangle(rectFillBrush, borderPen, rect);

                            _renderedPortList.Add(new DRect(rect.X, rect.Y, edgeLength, edgeLength, element.ZIndex,
                                element));
                        }

                        break;

                    default:
                        continue;
                }

                // Check whether the underlying collection has been modified to prevent a crash

                if (_itemsInvalidated)
                {
                    break;
                }
            }
        }

        if (_itemsInvalidated)
        {
            _renderableItems.Clear();
            _renderableItems.AddRange((AvaloniaList<NetlistElement>)_items);
        }

        _itemsInvalidated = false;
    }

    #region IntersectionTests

    private bool isInBounds(Point toCheck)
    {
        if (toCheck.X >= 0 && toCheck.X <= this.Bounds.Width)
        {
            if (toCheck.Y >= 0 && toCheck.Y <= this.Bounds.Height)
            {
                return true;
            }
        }

        return false;
    }

    private bool intersectsBounds(Rect rect)
    {
        // check if vertices intersect
        if (rect.X >= 0 && rect.X <= this.Bounds.Width)
        {
            if (rect.Y >= 0 && rect.Y <= this.Bounds.Height)
            {
                return true;
            }
        }

        if (rect.X + rect.Width >= 0 && rect.X + rect.Width <= this.Bounds.Width)
        {
            if (rect.Y >= 0 && rect.Y <= this.Bounds.Height)
            {
                return true;
            }
        }

        if (rect.X >= 0 && rect.X <= this.Bounds.Width)
        {
            if (rect.Y + rect.Height >= 0 && rect.Y + rect.Height <= this.Bounds.Height)
            {
                return true;
            }
        }

        if (rect.X + rect.Width >= 0 && rect.X + rect.Width <= this.Bounds.Width)
        {
            if (rect.Y + rect.Height >= 0 && rect.Y + rect.Height <= this.Bounds.Height)
            {
                return true;
            }
        }

        // check if edges intersect

        // top edge
        if (rect.X <= 0 && rect.X + rect.Width >= 0)
        {
            if (rect.Y >= 0 && rect.Y <= this.Bounds.Height)
            {
                return true;
            }
        }

        // left edge
        if (rect.X >= 0 && rect.X <= this.Bounds.Width)
        {
            if (rect.Y <= 0 && rect.Y + rect.Height >= 0)
            {
                return true;
            }
        }

        // bottom edge
        if (rect.X <= 0 && rect.X + rect.Width >= 0)
        {
            if (rect.Y + rect.Height >= 0 && rect.Y + rect.Height <= this.Bounds.Height)
            {
                return true;
            }
        }

        // right edge
        if (rect.X + rect.Width >= 0 && rect.X + rect.Width <= this.Bounds.Width)
        {
            if (rect.Y <= 0 && rect.Y + rect.Height >= 0)
            {
                return true;
            }
        }

        return false;
    }

    private bool intersectsBounds(Point ps, Point pe)
    {
        double dx = ps.X - pe.X;
        double dy = ps.Y - pe.Y;

        double uY, lY, lX, rX;

        // check dx and dy for zero values
        if (dx == 0)
        {
            // check for vertical intersection
            if (ps.X >= 0 && ps.X <= this.Bounds.Width)
            {
                if (ps.Y > pe.Y)
                {
                    uY = pe.Y;
                    lY = ps.Y;
                }
                else
                {
                    uY = ps.Y;
                    lY = pe.Y;
                }

                if (uY <= 0 && lY >= 0 || uY <= this.Bounds.Height && lY >= this.Bounds.Height)
                {
                    return true;
                }
                else if (uY >= 0 && uY <= this.Bounds.Height && lY >= 0 && lY <= this.Bounds.Height)
                {
                    return true;
                }
            }

            return false;
        }

        // check for horizontal intersection
        if (dy == 0)
        {
            if (ps.Y >= 0 && ps.Y <= this.Bounds.Height)
            {
                if (ps.X < pe.X)
                {
                    lX = ps.X;
                    rX = pe.X;
                }
                else
                {
                    lX = pe.X;
                    rX = ps.X;
                }

                if (lX <= 0 && rX >= 0 || lX <= this.Bounds.Width && rX >= this.Bounds.Width)
                {
                    return true;
                }
                else if (lX >= 0 && lX <= this.Bounds.Width && rX >= 0 && rX <= this.Bounds.Width)
                {
                    return true;
                }
            }

            return false;
        }

        double ml = dy / dx;
        double y0l = (-ps.X) * ml + ps.Y;

        // first check against diagonal bottom left to top right
        double mr = (-this.Bounds.Height) / this.Bounds.Width;
        double y0r = (-this.Bounds.X) * mr + (this.Bounds.Y + this.Bounds.Height);

        for (int i = 0; i < 2; i++)
        {
            // get x coordinate of intersection, if it exists
            if (ml != mr)
            {
                double ix = (y0r - y0l) / (ml - mr);

                // get y coordinate
                double iy = mr * ix + y0r;

                //check for intersection
                if (ix >= 0 && ix <= this.Bounds.Width)
                {
                    if (iy >= 0 && iy <= this.Bounds.Height)
                    {
                        return true;
                    }
                }
            }

            mr *= -1.0d;
            y0r = (-this.Bounds.X) * mr + (this.Bounds.Y);
        }

        return false;
    }

    private bool containsBounds(Rect rect)
    {
        bool ret = true;

        if (rect.X <= 0 && rect.X <= this.Bounds.Width)
        {
            if (rect.Y <= 0 && rect.Y <= this.Bounds.Height)
            {
            }
            else
            {
                ret = false;
            }
        }
        else
        {
            ret = false;
        }

        if (rect.X + rect.Width >= 0 && rect.X + rect.Width >= this.Bounds.Width)
        {
            if (rect.Y <= 0 && rect.Y <= this.Bounds.Height)
            {
            }
            else
            {
                ret = false;
            }
        }
        else
        {
            ret = false;
        }

        if (rect.X <= 0 && rect.X <= this.Bounds.Width)
        {
            if (rect.Y + rect.Height >= 0 && rect.Y + rect.Height >= this.Bounds.Height)
            {
            }
            else
            {
                ret = false;
            }
        }
        else
        {
            ret = false;
        }

        if (rect.X + rect.Width >= 0 && rect.X + rect.Width >= this.Bounds.Width)
        {
            if (rect.Y + rect.Height >= 0 && rect.Y + rect.Height >= this.Bounds.Height)
            {
            }
            else
            {
                ret = false;
            }
        }
        else
        {
            ret = false;
        }

        return ret;
    }

    #endregion

    #region EventHandlers

    private void NetlistControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        PointerPoint currentPoint = e.GetCurrentPoint(this);

        _pointerPressed = currentPoint.Properties.IsLeftButtonPressed;

        _pointerPosition = currentPoint.Position;
    }

    private void NetlistControl_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        PointerPoint currentPoint = e.GetCurrentPoint(this);

        _pointerPressed = currentPoint.Properties.IsLeftButtonPressed;

        _pointerPosition = currentPoint.Position;
    }

    private void NetlistControl_OnTapped(object? sender, TappedEventArgs e)
    {
        NetlistElement? hn = null; // highest label
        NetlistElement? he = null; // highest label
        NetlistElement? hj = null; // highest label
        NetlistElement? // highest junction
            hp = null; // highest label
        NetlistElement? // highest junction
            hl = null; // highest label

        // Due to the underlying structure of the list of netlist elements,
        // the last element in the subset of elements below the cursor
        // has the highest z-index, meaning that it is rendered on top of all
        // preceding elements

        foreach (var elem in _renderedNodeList)
        {
            if (elem.Hittest(e.GetPosition(this)))
            {
                hn = elem.element;
            }
        }

        foreach (var elem in _renderedEdgeList)
        {
            if (elem.element is null) continue;
            if (elem.Hittest(e.GetPosition(this)))
            {
                elem.element.IsHighlighted = true;

                he = elem.element;
            }
            else
            {
                if (elem.element != he)
                {
                    elem.element.IsHighlighted = false;
                }
            }
        }

        foreach (var elem in _renderedPortList)
        {
            if (elem.Hittest(e.GetPosition(this)))
            {
                hp = elem.element;
            }
        }

        foreach (var elem in _renderedJunctionList)
        {
            if (elem.Hittest(e.GetPosition(this)))
            {
                hj = elem.Element;
            }
        }

        foreach (var elem in _renderedLabelList)
        {
            if (elem.Hittest(e.GetPosition(this)))
            {
                hl = elem.element;
            }
        }

        if (hn != null)
        {
            if (he != null)
            {
                CurrentElement = he;

                Redraw();
            }
            else if (hj != null)
            {
                CurrentElement = hj;
            }
            else if (hl != null)
            {
                CurrentElement = hl;
            }
            else if (hp != null)
            {
                CurrentElement = hp;
            }
            else
            {
                CurrentElement = hn;

                if (CurrentElement.Celltype is "HDL_ENTITY" or "" && CurrentElement.ZIndex is not 1)
                {
                    // kinda bad
                    if (ClickedElementPath == hn.Path)
                    {
                        ClickedElementPath += ' ';
                    }
                    else
                    {
                        ClickedElementPath = hn.Path;
                    }

                    _viewportDimensionService!.SetClickedElementPath(NetlistID, hn.Path);

                    ElementClicked?.Invoke(this, new ElementClickedEventArgs(hn.Path));
                }
                else
                {
                    string? srcloc = CurrentElement.SrcLocation;

                    _ = OpenHdlSourceAsync(srcloc);
                }
            }
        }
    }

    private async Task OpenHdlSourceAsync(string? srcLine)
    {
        if (srcLine is null || srcLine == "")
        {
            return;
        }

        string?[] srclineSplit = srcLine.Split('|');

        srcLine = srclineSplit.First();

        int lastpos = -1;

        if (PlatformHelper.Platform is PlatformId.WinArm64 or PlatformId.WinX64)
        {
            lastpos = srcLine!.LastIndexOfAny([':']);
        } else if (PlatformHelper.Platform is not PlatformId.Unknown or PlatformId.Wasm)
        {
            lastpos = srcLine!.IndexOf(':');
        }
        
        if (lastpos == -1)
        {
            lastpos = srcLine!.Length - 1;
        }

        long line = 1;
        string filename = "";

        // PMUXes somehow have the actual src attribute set two times; While both contain the correct source file, the
        // first does not contain the line number. Only the second does

        for (int i = 0; i < 2; i++)
        {
            filename = srcLine!.Substring(0, lastpos);
            string lines = srcLine.Substring(lastpos + 1);
            string[] linesSplit = lines.Split('.');

            if (linesSplit.Length > 0)
            {
                try
                {
                    line = long.Parse(linesSplit[0]);
                }
                catch (Exception)
                {
                    line = 1;
                }
            }

            if (line == 0)
            {
                if (srclineSplit.Length > 1)
                {
                    srcLine = srclineSplit[1];
                }
            }
            else
            {
                break;
            }
        }

        (string vhdlFilename, long vhdlLine, bool success) = ServiceManager.GetService<ICcVhdlFileIndexService>()
            .GetActualSource(line, NetlistID);

        if (success)
        {
            filename = vhdlFilename;
            line = vhdlLine;
        }

        var ds = ServiceManager.GetService<IDockService>();

        var document = await ds.OpenFileAsync(new ExternalFile(filename));

        (document as IEditor)?.JumpToLine((int)line);
    }

    private void NetlistControl_PointerMoved(object? sender, PointerEventArgs e)
    {
        var pointerpoints = e.GetIntermediatePoints(this);
        double dx, dy;

        Point currentPos = e.GetPosition(this);

        if (pointerpoints.First().Properties.IsLeftButtonPressed || _pointerPressed)
        {
            dx = currentPos.X - _pointerPosition.X;
            dy = currentPos.Y - _pointerPosition.Y;
        }
        else
        {
            dx = 0;
            dy = 0;
        }

        _pointerPosition = currentPos;

        this.DeltaX += dx;
        this.DeltaY += dy;
    }

    private void NetlistControl_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        var wheel = e.Delta;

        this.DeltaScale += wheel.Y;

        this.PointerX = e.GetPosition(this).X;
        this.PointerY = e.GetPosition(this).Y;
    }

    private void NetlistControl_OnElementClicked(object sender, ElementClickedEventArgs e)
    {
        ServiceManager.GetCustomLogger().Log($"Toggling entity at {e.NodePath}", false);
    }

    #endregion

    public bool HitTest(Point point)
    {
        return true;
    }
}