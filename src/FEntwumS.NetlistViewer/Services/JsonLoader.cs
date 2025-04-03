using System.Text.Json.Nodes;
using Avalonia;
using FEntwumS.Common.Services;
using FEntwumS.Common.Types;
using FEntwumS.NetlistViewer.ViewModels;
using OneWare.Essentials.ViewModels;

namespace FEntwumS.NetlistViewer.Services;

public class JsonLoader : IJsonLoader
{
    private JsonNode? RootNode { get; set; }
    private bool IsLoading { get; set; }

    public double MaxWidth { get; set; }
    public double MaxHeight { get; set; }
    private double Scale { get; set; }

    private long NodeCnt { get; set; }
    private long EdgeCnt { get; set; }
    private long JunctionCnt { get; set; }
    private long LabelCnt { get; set; }
    private long PortCnt { get; set; }
    private long BendCnt { get; set; }
    private long CharCnt { get; set; }
    private readonly ICustomLogger _logger;
    
    private string? ClickedElementPath { get; set; }
    private string? ClickedElementParentPath { get; set; }
    private DRect? ClickedElementRect { get; set; }
    private DRect? ClickedElementParentRect { get; set; }
    private readonly IViewportDimensionService? _viewportDimensionService;

    public JsonLoader()
    {
        _viewportDimensionService = ServiceManager.GetViewportDimensionService();
        _logger = ServiceManager.GetCustomLogger();
    }

    public async Task OpenJsonAsync(Stream netlist, UInt64 netlistId)
    {
        IsLoading = true;
        RootNode = await JsonNode.ParseAsync(netlist);
        IsLoading = false;
        Scale = 1.0d;
    }

    private async Task LoadingDoneAsync()
    {
        while (IsLoading)
        {
            await Task.Delay(1);
        }
    }

    public async Task<List<NetlistElement>> ParseJsonAsync(double xRef, double yRef,
        ExtendedTool mw, UInt64 netlistId)
    {
        await LoadingDoneAsync();

        ClickedElementPath = _viewportDimensionService!.GetClickedElementPath(netlistId);

        if (ClickedElementPath == null)
        {
            ClickedElementPath = string.Empty;
        }
        
        var clickedElementPathSplit = ClickedElementPath.Split(' ');

        if (clickedElementPathSplit.Length < 2)
        {
            ClickedElementParentPath = string.Empty;
        }
        else
        {
            ClickedElementParentPath = string.Join(" ", clickedElementPathSplit, 0, clickedElementPathSplit.Length - 1);
        }
        
        _logger.Log("Start loading elements");
        _logger.Log("====");
        
        NodeCnt = 0;
        LabelCnt = 0;
        PortCnt = 0;
        BendCnt = 0;
        EdgeCnt = 0;
        JunctionCnt = 0;
        CharCnt = 0;

        MaxWidth = 0;
        MaxHeight = 0;

        List<NetlistElement> items = new List<NetlistElement>();

        if (RootNode is null) return items;
        
        CreateNode(RootNode, items, xRef, yRef, 0);
        
        // check for clicked elements
        // TODO was anything clicked????
        if (_viewportDimensionService.getCurrentElementCount(netlistId) == 0)
        {
            _viewportDimensionService.SetZoomElementDimensions(netlistId, new DRect(0, 0, MaxWidth, MaxHeight, 0, null));
        }
        else if (items.Count > _viewportDimensionService.getCurrentElementCount(netlistId))
        {
            // expansion
            
            _viewportDimensionService.SetZoomElementDimensions(netlistId, ClickedElementRect);
        }
        else
        {
            // collapse
            
            _viewportDimensionService.SetZoomElementDimensions(netlistId, ClickedElementParentRect);
        }
        
        _viewportDimensionService.SetCurrentElementCount(netlistId, items.Count);
        
        _viewportDimensionService.SetMaxHeight(netlistId, MaxHeight);
        _viewportDimensionService.SetMaxWidth(netlistId, MaxWidth);
        
        
        _logger.Log("====");
        _logger.Log("All elements loaded");
        _logger.Log("Statistics:");

        // Dispose of the JSON document, as we dont need to keep it around
        RootNode = new JsonObject();

        // Call the garbage collector to free dozens of MB of RAM
        // If the GC isn't called explicitly, the dead objects of the JSON file will just stay in the Gen 2 Heap
        GC.Collect();

        _logger.Log("Number of Objects: " + items.Count);
        _logger.Log("Number of nodes: " + NodeCnt);
        _logger.Log("Number of ports: " + PortCnt);
        _logger.Log("Number of edges: " + EdgeCnt);
        _logger.Log("Average number of bendpoints per edge: " + ((float)BendCnt / (float)EdgeCnt));
        _logger.Log("Number of junctions: " + JunctionCnt);
        _logger.Log("Number of labels: " + LabelCnt);
        _logger.Log("Average number of characters per label: " + ((float)CharCnt / (float)LabelCnt));
        _logger.Log("Max width: " + MaxWidth);
        _logger.Log("Max height: " + MaxHeight);
        
        

        //mw.UpdateScaleImpl();

        return items;
    }

    public void CreateNode(JsonNode node, List<NetlistElement> items, double xRef, double yRef,
        ushort depth)
    {
        JsonArray? children = node["children"] as JsonArray;
        double x = 0;
        double y = 0;
        double nWidth = 0;
        double nHeight = 0;

        string celltype = "";
        string cellname = "";
        string path = "";
        string src = "";

        JsonNode? layoutOptions = node["layoutOptions"] as JsonNode;

        if (node.AsObject().ContainsKey("x"))
        {
            x = node["x"]!.GetValue<double>();
        }

        if (node.AsObject().ContainsKey("y"))
        {
            y = node["y"]!.GetValue<double>();
        }

        if (node.AsObject().ContainsKey("width"))
        {
            nWidth = node["width"]!.GetValue<double>();
        }

        if (node.AsObject().ContainsKey("height"))
        {
            nHeight = node["height"]!.GetValue<double>();
        }

        if (layoutOptions is not null)
        {
            if (layoutOptions.AsObject().ContainsKey("celltype"))
            {
                celltype = layoutOptions["celltype"]!.GetValue<string>();
            }

            if (layoutOptions.AsObject().ContainsKey("cellname"))
            {
                cellname = layoutOptions["cellname"]!.GetValue<string>();
            }

            if (layoutOptions.AsObject().ContainsKey("location-path"))
            {
                path = layoutOptions["location-path"]!.GetValue<string>();
            }

            if (layoutOptions.AsObject().ContainsKey("src-location"))
            {
                src = layoutOptions["src-location"]!.GetValue<string>();
            }
        }

        if (path != string.Empty)
        {
            if (path == ClickedElementPath)
            {
                ClickedElementRect = new DRect(xRef + x, yRef + y, nWidth, nHeight, depth, null);
            }
            else if (path == ClickedElementParentPath)
            {
                ClickedElementParentRect = new DRect(xRef + x, yRef + y, nWidth, nHeight, depth, null);
            }
        } else if (ClickedElementPath is not null && ClickedElementPath.Length > 0 && ClickedElementParentPath == string.Empty && depth == 1)
        {
            ClickedElementParentRect = new DRect(0, 0, MaxWidth, MaxHeight, depth, null);
        }

        if (xRef + x + nWidth > MaxWidth)
        {
            MaxWidth = xRef + (x + nWidth) * Scale;
        }

        if (yRef + y + nHeight > MaxHeight)
        {
            MaxHeight = yRef + (y + nHeight) * Scale;
        }

        if (!(string.Equals((string)node["id"]!, "root", StringComparison.Ordinal)))
        {
            items.Add(new NetlistElement()
            {
                Height = nHeight * Scale,
                Width = nWidth * Scale,
                xPos = xRef + x * Scale,
                yPos = yRef + y * Scale,
                Type = 1,
                ZIndex = depth,
                Celltype = celltype,
                Cellname = cellname,
                SrcLocation = src,
                Path = path
            });

            NodeCnt++;
        }

        JsonArray? labels = node["labels"] as JsonArray;

        if (labels != null)
        {
            CreateLabels(labels, items, xRef + x * Scale, yRef + y * Scale, (ushort)(depth + 1));
        }

        JsonArray? ports = node["ports"] as JsonArray;

        if (ports != null)
        {
            CreatePorts(ports, items, xRef + x * Scale, yRef + y * Scale, (ushort)(depth + 1));
        }

        JsonArray? edges = node["edges"] as JsonArray;

        if (edges != null)
        {
            CreateEdges(edges, items, xRef + x * Scale, yRef + y * Scale, (ushort)(depth + 1));
        }

        if (children == null) return;
        foreach (JsonNode? child in children)
        {
            if (child is not null)
            {
                CreateNode(child, items, xRef + x * Scale, yRef + y * Scale, (ushort)(depth + 1));
            }
        }
    }

    public void CreateLabels(JsonArray labels, List<NetlistElement> items, double xRef,
        double yRef, ushort depth)
    {
        double x = 0;
        double y = 0;
        double w = 0;
        double h = 0;
        string text = "";
        double fontSize = 10.0d;
        JsonNode? layoutOptions;
        foreach (JsonNode? label in labels)
        {
            if (label is null) continue;
            
            x = 0;
            y = 0;
            w = 0;
            h = 0;
            text = "";
            layoutOptions = label["layoutOptions"] as JsonNode;

            if (label.AsObject().ContainsKey("x"))
            {
                x = label["x"]!.GetValue<double>();
            }

            if (label.AsObject().ContainsKey("y"))
            {
                y = label["y"]!.GetValue<double>();
            }

            if (label.AsObject().ContainsKey("width"))
            {
                w = label["width"]!.GetValue<double>();
            }

            if (label.AsObject().ContainsKey("height"))
            {
                h = label["height"]!.GetValue<double>();
            }

            if (label.AsObject().ContainsKey("text"))
            {
                text = label["text"]!.GetValue<string>();

                CharCnt += text.Length;
            }

            if (layoutOptions != null && layoutOptions.AsObject().ContainsKey("font-size"))
            {
                fontSize = double.Parse(layoutOptions["font-size"]!.GetValue<string>(), System.Globalization.CultureInfo.InvariantCulture);
            }

            items.Add(new NetlistElement()
            {
                LabelText = text,
                xPos = xRef + x * Scale + 1, // Add small offset to separate label from border
                yPos = yRef + y * Scale,
                Type = 3,
                Width = w,
                Height = h,
                ZIndex = depth,
                FontSize = fontSize
            });

            LabelCnt++;
        }
    }

    public void CreatePorts(JsonArray ports, List<NetlistElement> items, double xRef,
        double yRef, ushort depth)
    {
        double x = 0;
        double y = 0;
        foreach (JsonNode? port in ports)
        {
            if (port is null) continue;
            
            x = 0;
            y = 0;
            JsonArray? labels = port["labels"] as JsonArray;

            if (port.AsObject().ContainsKey("x"))
            {
                x = port["x"]!.GetValue<double>();
            }

            if (port.AsObject().ContainsKey("y"))
            {
                y = port["y"]!.GetValue<double>();
            }

            if (labels is not null)
            {
                CreateLabels(labels, items, xRef + x * Scale, yRef + y * Scale, (ushort)(depth + 1));
            }

            items.Add(new NetlistElement()
            {
                xPos = xRef + x * Scale,
                yPos = yRef + y * Scale,
                Type = 5,
                ZIndex = depth
            });

            PortCnt++;
        }
    }

    public void CreateEdges(JsonArray edges, List<NetlistElement> items, double xRef,
        double yRef, ushort depth)
    {
        double x = 0;
        double y = 0;
        JsonArray? sections;
        JsonNode? start;
        JsonNode? end;
        JsonArray? bendpoints;
        Point cPoint = new Point();
        Point ePoint = new Point();
        JsonNode? layoutOptions;
        string locationpath;
        string src;
        string signalname;
        int indexInSignal;
        string signaltype;

        foreach (JsonNode? edge in edges)
        {
            if (edge is null) continue;
            
            locationpath = "";
            src = "";
            signalname = "";
            indexInSignal = 0;
            signaltype = "";

            sections = edge["sections"] as JsonArray;

            if (sections == null)
            {
                continue;
            }

            foreach (JsonNode? section in sections)
            {
                if (section is null) continue;
                
                start = section["startPoint"];
                end = section["endPoint"];
                bendpoints = section["bendPoints"] as JsonArray;
                List<Point> pointList = new List<Point>();
                layoutOptions = edge["layoutOptions"] as JsonNode;

                // Move to start
                if (start != null)
                {
                    if (start.AsObject().ContainsKey("x"))
                    {
                        x = start["x"]!.GetValue<double>();
                    }

                    if (start.AsObject().ContainsKey("y"))
                    {
                        y = start["y"]!.GetValue<double>();
                    }

                    cPoint = new Point(x * Scale, y * Scale);

                    pointList.Add(cPoint);
                }

                // Add bends
                if (bendpoints != null)
                {
                    foreach (JsonNode? bend in bendpoints)
                    {
                        if (bend is null) continue;
                        
                        x = 0;
                        y = 0;

                        if (bend.AsObject().ContainsKey("x"))
                        {
                            x = bend["x"]!.GetValue<double>();
                        }

                        if (bend.AsObject().ContainsKey("y"))
                        {
                            y = bend["y"]!.GetValue<double>();
                        }

                        cPoint = new Point(x * Scale, y * Scale);

                        pointList.Add(cPoint);

                        BendCnt++;
                    }
                }

                // Add end
                if (end != null)
                {
                    x = 0;
                    y = 0;

                    if (end.AsObject().ContainsKey("x"))
                    {
                        x = end["x"]!.GetValue<double>();
                    }

                    if (end.AsObject().ContainsKey("y"))
                    {
                        y = end["y"]!.GetValue<double>();
                    }

                    ePoint = new Point(x * Scale, y * Scale);

                    pointList.Add(ePoint);
                }

                // Create arrow tip
                double xDir = cPoint.X - ePoint.X;
                double yDir = cPoint.Y - ePoint.Y;

                double mag = Math.Sqrt(xDir * xDir + yDir * yDir);

                xDir /= mag;
                yDir /= mag;

                xDir *= 7 * Scale;
                yDir *= 7 * Scale;

                // Angle of 30 degrees
                double xUp = 0.86 * xDir - (0.5) * yDir;
                double yUp = (0.5) * xDir + 0.86 * yDir;
                double xDown = (-0.86) * xDir - (0.5) * yDir;
                double yDown = (0.5) * xDir + (-0.86) * yDir;

                Point upPoint = new Point(ePoint.X + xUp, ePoint.Y + yUp);
                Point downPoint = new Point(ePoint.X - xDown, ePoint.Y - yDown);

                pointList.Add(upPoint);
                pointList.Add(ePoint);
                pointList.Add(downPoint);

                BendCnt += 3;

                if (layoutOptions is not null)
                {
                    if (layoutOptions.AsObject().ContainsKey("src-location"))
                    {
                        src = layoutOptions["src-location"]!.GetValue<string>();
                    }

                    if (layoutOptions.AsObject().ContainsKey("location-path"))
                    {
                        locationpath = layoutOptions["location-path"]!.GetValue<string>();
                    }

                    if (layoutOptions.AsObject().ContainsKey("signalname"))
                    {
                        signalname = layoutOptions["signalname"]!.GetValue<string>();
                    }

                    if (layoutOptions.AsObject().ContainsKey("index-in-signal"))
                    {
                        // GetValue<int>() somehow does not (always?) work with negative integers
                        // Therefore this construct is used
                        indexInSignal = Convert.ToInt32(layoutOptions["index-in-signal"]!.GetValue<string>());
                    }

                    if (layoutOptions.AsObject().ContainsKey("signaltype"))
                    {
                        signaltype = layoutOptions["signaltype"]!.GetValue<string>();
                    }
                }

                items.Add(new NetlistElement()
                {
                    xPos = xRef,
                    yPos = yRef,
                    Type = 2,
                    Points = pointList,
                    ZIndex = depth,
                    SrcLocation = src,
                    Path = locationpath,
                    Signalname = signalname,
                    IndexInSignal = indexInSignal,
                    SignalType = signaltype
                });

                EdgeCnt++;
            }

            if (edge["labels"] is JsonArray labels)
            {
                // foreach (JsonNode label in labels)
                // {
                //     string text = "";
                //
                //     if (label.AsObject().ContainsKey("text"))
                //     {
                //         text = label["text"].GetValue<string>();
                //     }
                //
                //     if (string.Equals(text, "1"))
                //     {
                //         items.Last().Color = new SolidColorBrush(Color.FromArgb(255, 0, 125, 0));
                //     }
                //     else if (string.Equals(text, "0"))
                //     {
                //         items.Last().Color = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                //     }
                //     else if (string.Equals(text, "z"))
                //     {
                //         items.Last().Color = new SolidColorBrush(Color.FromArgb(255, 150, 75, 0));
                //     }
                //     else if (string.Equals(text, "x"))
                //     {
                //         items.Last().Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                //     }
                //     else
                //     {
                //         items.Last().Color = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                //     }
                // }

                CreateLabels(labels, items, xRef, yRef, (ushort)(depth + 1));
            }

            JsonArray? junctionPoints = edge["junctionPoints"] as JsonArray;

            if (junctionPoints is not null)
            {
                CreateJunctionPoints(junctionPoints, items, xRef, yRef, (ushort)(depth + 1));
            }
        }
    }

    public void CreateJunctionPoints(JsonArray junctionPoints, List<NetlistElement> items,
        double xRef, double yRef, ushort depth)
    {
        double x = 0;
        double y = 0;
        foreach (JsonNode? junctionPoint in junctionPoints)
        {
            if (junctionPoint is null) continue;
            
            x = 0;
            y = 0;

            if (junctionPoint.AsObject().ContainsKey("x"))
            {
                x = junctionPoint["x"]!.GetValue<double>();
            }

            if (junctionPoint.AsObject().ContainsKey("y"))
            {
                y = junctionPoint["y"]!.GetValue<double>();
            }

            // Add offset to move junction point to line center
            // offset is the radius of the junction point symbol
            // x -= 3;
            // y -= 3;

            items.Add(new NetlistElement()
            {
                xPos = xRef + x * Scale,
                yPos = yRef + y * Scale,
                Type = 4,
                ZIndex = depth
            });

            JunctionCnt++;
        }
    }

    public double GetMaxWidth()
    {
        _logger.Log("Maxwidth: " + MaxWidth);
        return MaxWidth;
    }

    public double GetMaxHeight()
    {
        _logger.Log("Maxheight: " + MaxHeight);
        return MaxHeight;
    }
}