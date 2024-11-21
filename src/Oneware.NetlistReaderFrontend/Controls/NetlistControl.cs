using System.Collections;
using System.Globalization;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Oneware.NetlistReaderFrontend.Services;
using Oneware.NetlistReaderFrontend.Types;

namespace Oneware.NetlistReaderFrontend.Controls;

public class NetlistControl : TemplatedControl
{
    private static readonly FontFamily font = (Application.Current!.FindResource("MartianMono") as FontFamily)!;

    private static readonly Typeface typeface =
        new Typeface(font, FontStyle.Normal, FontWeight.Regular, FontStretch.Normal);

    static NetlistControl()
    {
        AffectsRender<NetlistControl>(ItemsProperty, IsEnabledProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == ItemsProperty || change.Property == OffsetXProperty ||
            change.Property == OffsetYProperty || change.Property == CurrentScaleProperty ||
            change.Property == IsPointerOverProperty)
        {
            return;
        }

        Console.WriteLine(change.Property);

        if (change.Property == IsLoadedProperty)
        {
            CurrentScale = 0.2;

            OffsetX = 0;
            OffsetY = 0;
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

        //Redraw();
        base.OnPropertyChanged(change);
    }

    private void ZoomToFit()
    {
        var dimensionService = ServiceManager.GetViewportDimensionService();

        double sx = this.Bounds.Width / dimensionService.GetWidth();
        double sy = this.Bounds.Height / dimensionService.GetHeight();

        if (sx < sy)
        {
            CurrentScale = sx;
        }
        else
        {
            CurrentScale = sy;
        }

        OffsetX = 0;
        OffsetY = 0;
    }

    public void Redraw()
    {
        _ = Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Render);
    }

    public static readonly DirectProperty<NetlistControl, IEnumerable> ItemsProperty =
        AvaloniaProperty.RegisterDirect<NetlistControl, IEnumerable>(
            nameof(Items),
            o => o.Items,
            (o, v) => o.Items = v);

    private IEnumerable _items = new AvaloniaList<object>();

    public IEnumerable Items
    {
        get => _items;
        set => SetAndRaise(ItemsProperty, ref _items, value);
    }

    public static readonly StyledProperty<double> StepSizeProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(StepSize));

    public double StepSize
    {
        get => GetValue(StepSizeProperty);
        set => SetValue(StepSizeProperty, value);
    }

    public static readonly StyledProperty<double> DiagramWidthProperty = AvaloniaProperty.Register<NetlistControl,
        double>(nameof(DiagramWidth));

    public double DiagramWidth
    {
        get => GetValue(DiagramWidthProperty);
        set => SetValue(DiagramWidthProperty, value);
    }

    public static readonly StyledProperty<double> DiagramHeightProperty = AvaloniaProperty.Register<NetlistControl,
        double>(nameof(DiagramHeight));

    public double DiagramHeight
    {
        get => GetValue(DiagramHeightProperty);
        set => SetValue(DiagramHeightProperty, value);
    }

    public static readonly StyledProperty<double> CurrentScaleProperty = AvaloniaProperty.Register<NetlistControl,
        double>(nameof(CurrentScale));

    public double CurrentScale
    {
        get => GetValue(CurrentScaleProperty);
        set => SetValue(CurrentScaleProperty, value);
    }

    public static readonly StyledProperty<double> OffsetXProperty = AvaloniaProperty.Register<NetlistControl, double>
        (nameof(OffsetX));

    public double OffsetX
    {
        get => GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }

    public static readonly StyledProperty<double> OffsetYProperty = AvaloniaProperty.Register<NetlistControl, double>
        (nameof(OffsetY));

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
        AvaloniaProperty.Register<NetlistControl, double>(nameof(DeltaX));

    public double DeltaY
    {
        get => GetValue(DeltaYProperty);
        set => SetValue(DeltaYProperty, value);
    }

    public static readonly StyledProperty<double> DeltaYProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(DeltaY));

    public double DeltaScale
    {
        get => GetValue(DeltaScaleProperty);
        set => SetValue(DeltaScaleProperty, value);
    }

    public static readonly StyledProperty<double> DeltaScaleProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(DeltaScale), defaultBindingMode: BindingMode.TwoWay);

    public double PointerX
    {
        get => GetValue(PointerXProperty);
        set => SetValue(PointerXProperty, value);
    }

    public static readonly StyledProperty<double> PointerXProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(PointerX));

    public double PointerY
    {
        get => GetValue(PointerYProperty);
        set => SetValue(PointerYProperty, value);
    }

    public static readonly StyledProperty<double> PointerYProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(PointerY));

    public double PortScaleClip
    {
        get => GetValue(PortScaleClipProperty);
        set => SetValue(PortScaleClipProperty, value);
    }

    public static readonly StyledProperty<double> PortScaleClipProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(PortScaleClip));

    public double NodeScaleClip
    {
        get => GetValue(NodeScaleClipProperty);
        set => SetValue(NodeScaleClipProperty, value);
    }

    public static readonly StyledProperty<double> NodeScaleClipProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(NodeScaleClip));

    public double LabelScaleClip
    {
        get => GetValue(LabelScaleClipProperty);
        set => SetValue(LabelScaleClipProperty, value);
    }

    public static readonly StyledProperty<double> LabelScaleClipProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(LabelScaleClip));

    public double EdgeLengthScaleClip
    {
        get => GetValue(EdgeLengthScaleClipProperty);
        set => SetValue(EdgeLengthScaleClipProperty, value);
    }

    public static readonly StyledProperty<double> EdgeLengthScaleClipProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(EdgeLengthScaleClip));

    public double JunctionScaleClip
    {
        get => GetValue(JunctionScaleClipProperty);
        set => SetValue(JunctionScaleClipProperty, value);
    }

    public static readonly StyledProperty<double> JunctionScaleClipProperty =
        AvaloniaProperty.Register<NetlistControl, double>(nameof(JunctionScaleClip));

    public bool FitToZoom
    {
        get => GetValue(FitToZoomProperty);
        set => SetValue(FitToZoomProperty, value);
    }

    public static readonly StyledProperty<bool> FitToZoomProperty =
        AvaloniaProperty.Register<NetlistControl, bool>(nameof(FitToZoom));

    public bool IsLoaded
    {
        get => GetValue(IsLoadedProperty);
        set => SetValue(IsLoadedProperty, value);
    }

    public static readonly StyledProperty<bool> IsLoadedProperty =
        AvaloniaProperty.Register<NetlistControl, bool>(nameof(IsLoaded));

    public NetlistElement CurrentElement
    {
        get => GetValue(CurrentElementProperty);
        set => SetValue(CurrentElementProperty, value);
    }

    public static readonly StyledProperty<NetlistElement> CurrentElementProperty =
        AvaloniaProperty.Register<NetlistControl, NetlistElement>(nameof(CurrentElement),
            defaultBindingMode: BindingMode.TwoWay);

    private List<DRect> renderedNodeList = new List<DRect>();
    private List<DRect> renderedLabelList = new List<DRect>();
    private List<DRect> renderedPortList = new List<DRect>();
    private List<DCircle> renderedJunctionList = new List<DCircle>();
    private List<DLine> renderedEdgeList = new List<DLine>();

    public override void Render(DrawingContext context)
    {
        double x = 0, y = 0, width = 0, height = 0, radius = 0, edgeLength = 0;
        List<Point> points;

        bool previousNodeInView = true;
        int lastVisibleNodeZIndex = 0;

        renderedNodeList.Clear();
        renderedLabelList.Clear();
        renderedPortList.Clear();
        renderedJunctionList.Clear();
        renderedEdgeList.Clear();

        ThemeVariant theme = Application.Current.ActualThemeVariant;

        Brush backgroundBrush =
            new SolidColorBrush(Application.Current!.FindResource(theme, "ThemeBackgroundColor") is Color
                ? (Color)Application.Current!.FindResource(theme, "ThemeBackgroundColor")
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
                    ? (Color)Application.Current.FindResource(theme, "ThemeBorderHighColor")
                    : Colors.DarkGray)),
                2.5 * CurrentScale, null, PenLineCap.Square);
        Pen edgePen = new Pen(
            Application.Current.FindResource(theme, "ThemeAccentBrush") as IBrush ?? new SolidColorBrush(Colors.Black),
            1.2 * CurrentScale, null, PenLineCap.Square);
        Pen bundledEdgePen = new Pen(
            Application.Current.FindResource(theme, "ThemeAccentBrush") as IBrush ?? new SolidColorBrush(Colors.Black),
            2.2 * CurrentScale, null, PenLineCap.Square);
        Brush rectFillBrush = Application.Current!.FindResource(theme, "ThemeBackgroundBrush") as SolidColorBrush ??
                              new SolidColorBrush(Colors.LightBlue);
        Brush ellipseFillBrush =
            Application.Current!.FindResource(theme, "ThemeAccentBrush") as SolidColorBrush ??
            new SolidColorBrush(Colors.Black);
        Brush textBrush = new SolidColorBrush(Application.Current!.FindResource(theme, "ThemeAccentColor") is Color
            ? (Color)Application.Current!.FindResource(theme, "ThemeAccentColor")
            : Colors.Black);

        // Draw background
        context.DrawRectangle(backgroundBrush, null, new Rect(0, 0, this.Bounds.Width, this.Bounds.Height));

        Rect rect;
        Rect boundingBox;
        Point center;
        Point start, bend, end;
        bool previousPointInBounds, currentPointInBounds;
        bool drawLine;

        double deltaX = DeltaX, deltaY = DeltaY, deltaScale = DeltaScale, step = -0.9d;

        // Apply offset from user interaction
        OffsetX += deltaX;
        OffsetY += deltaY;

        if (deltaScale != 0)
        {
            if (deltaScale > 2)
            {
                deltaScale = 2;
            }
            else if (deltaScale < -2)
            {
                deltaScale = -2;
            }

            if (deltaScale > 0)
            {
                step = (-1) / step;
            }

            // Apply scale
            CurrentScale *= step * deltaScale;

            OffsetX *= step * deltaScale;
            OffsetY *= step * deltaScale;

            OffsetX += PointerX - PointerX * step * deltaScale;
            OffsetY += PointerY - PointerY * step * deltaScale;
        }

        foreach (NetlistElement element in _items)
        {
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

                        renderedNodeList.Add(new DRect(x, y, width, height, element.ZIndex, element));

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

                            if (element.IsHighlighted)
                            {
                                context.DrawLine(highlightPen, points[i - 1], points[i]);
                            }

                            renderedEdgeList.Add(new DLine(points[i - 1], points[i], element.ZIndex, element));
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

                    if (height >= LabelScaleClip && (intersectsBounds(boundingBox) || containsBounds(boundingBox)))
                    {
                        FormattedText text = new FormattedText(element.LabelText, CultureInfo.InvariantCulture,
                            FlowDirection.LeftToRight, typeface, 10 * CurrentScale, textBrush);

                        if (element.ZIndex == 2)
                        {
                            context.DrawRectangle(rectFillBrush, null, boundingBox);
                        }

                        context.DrawText(text, new Point(x, y));

                        renderedLabelList.Add(new DRect(x, y, width, height, element.ZIndex, element));
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
                    radius = 3.5d * CurrentScale;

                    x += OffsetX;
                    y += OffsetY;

                    center = new Point(x, y);

                    if (radius * 2 >= JunctionScaleClip && isInBounds(center))
                    {
                        context.DrawEllipse(ellipseFillBrush, null, center, radius, radius);

                        renderedJunctionList.Add(new DCircle(x, y, radius, element.ZIndex, element));
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

                        renderedPortList.Add(new DRect(rect.X, rect.Y, edgeLength, edgeLength, element.ZIndex,
                            element));
                    }

                    break;

                default:
                    continue;
            }
        }

        DeltaX -= deltaX;
        DeltaY -= deltaY;
        DeltaScale -= deltaScale;
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

    private bool isInBounds(double x, double y)
    {
        if (x >= 0 && x <= this.Bounds.Width)
        {
            if (y >= 0 && y <= this.Bounds.Height)
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

    public void NetlistControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        return;
    }

    public void NetlistControl_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        return;
    }

    public void NetlistControl_OnTapped(object? sender, TappedEventArgs e)
    {
        NetlistElement hn = null,
            he = null,
            hj = null,
            hp = null,
            hl = null;

        foreach (var elem in renderedNodeList)
        {
            if (elem.Hittest(e.GetPosition(this)))
            {
                Console.WriteLine("Node with Z-Index: " + elem.ZIndex);
                Console.WriteLine("Node attributes =>");
                Console.WriteLine("Source location: " + elem.element.SrcLocation);
                Console.WriteLine("Cell path: " + elem.element.Path);
                Console.WriteLine("Cell name: " + elem.element.Cellname);
                Console.WriteLine("Cell type: " + elem.element.Celltype);

                hn = elem.element;
            }
        }

        foreach (var elem in renderedEdgeList)
        {
            if (elem.Hittest(e.GetPosition(this)))
            {
                Console.WriteLine("Line with Z_Index: " + elem.ZIndex);
                Console.WriteLine("Edge attributes =>");
                Console.WriteLine("Source location: " + elem.element.SrcLocation);
                Console.WriteLine("Signal path: " + elem.element.Path);
                Console.WriteLine("Signal name: " + elem.element.Signalname);
                Console.WriteLine("Index in Signal: " + elem.element.IndexInSignal);
                Console.WriteLine("Type of Signal: " + elem.element.SignalType);

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

        foreach (var elem in renderedPortList)
        {
            if (elem.Hittest(e.GetPosition(this)))
            {
                Console.WriteLine("Port with Z-Index: " + elem.ZIndex);

                hp = elem.element;
            }
        }

        foreach (var elem in renderedJunctionList)
        {
            if (elem.Hittest(e.GetPosition(this)))
            {
                Console.WriteLine("Junction with Z-Index: " + elem.ZIndex);

                hj = elem.element;
            }
        }

        foreach (var elem in renderedLabelList)
        {
            if (elem.Hittest(e.GetPosition(this)))
            {
                Console.WriteLine("Label with Z-Index: " + elem.ZIndex);

                hl = elem.element;
            }
        }

        if (hn != null)
        {
            if (he != null)
            {
                CurrentElement = he;
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
            }
        }

        Redraw();
    }
}