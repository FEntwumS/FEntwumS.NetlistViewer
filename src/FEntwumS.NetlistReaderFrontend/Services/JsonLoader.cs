using System.Text.Json.Nodes;
using Avalonia;
using FEntwumS.NetlistReaderFrontend.Types;
using FEntwumS.NetlistReaderFrontend.ViewModels;

namespace FEntwumS.NetlistReaderFrontend.Services;

public class JsonLoader : IJsonLoader
{
    private JsonNode rootnode { get; set; }
    private bool isLoading { get; set; }

    public double maxWidth { get; set; }
    public double maxHeight { get; set; }
    private double scale { get; set; }

    private long nodecnt { get; set; }
    private long edgecnt { get; set; }
    private long junctioncnt { get; set; }
    private long labelcnt { get; set; }
    private long portcnt { get; set; }
    private long bendcnt { get; set; }
    private long charcnt { get; set; }
    private ICustomLogger logger;
    
    private string clickedElementPath { get; set; }
    private string clickedElementParentPath { get; set; }
    private DRect clickedElementRect { get; set; }
    private DRect clickedElementParentRect { get; set; }
    private IViewportDimensionService viewportDimensionService { get; set; }

    public JsonLoader()
    {
        viewportDimensionService = ServiceManager.GetViewportDimensionService();
    }

    public async Task OpenJson(Stream netlist, UInt64 netlistId)
    {
        logger = ServiceManager.GetCustomLogger();
        isLoading = true;
        rootnode = await JsonNode.ParseAsync(netlist);
        isLoading = false;
        scale = 1.0d;
    }

    private async Task loadingDone()
    {
        while (isLoading)
        {
            await Task.Delay(1);
        }
    }

    public async Task<List<NetlistElement>> parseJson(double xRef, double yRef,
        FrontendViewModel mw, UInt64 netlistId)
    {
        await loadingDone();

        clickedElementPath = viewportDimensionService.GetClickedElementPath(netlistId);

        if (clickedElementPath == null)
        {
            clickedElementPath = string.Empty;
        }
        
        var clickedElementPathSplit = clickedElementPath.Split(' ');

        if (clickedElementPathSplit.Length < 2)
        {
            clickedElementParentPath = string.Empty;
        }
        else
        {
            clickedElementParentPath = string.Join(" ", clickedElementPathSplit, 0, clickedElementPathSplit.Length - 1);
        }
        
        logger.Log("Start loading elements");
        logger.Log("====");
        
        nodecnt = 0;
        labelcnt = 0;
        portcnt = 0;
        bendcnt = 0;
        edgecnt = 0;
        junctioncnt = 0;
        charcnt = 0;

        maxWidth = 0;
        maxHeight = 0;

        List<NetlistElement> items = new List<NetlistElement>();

        createNode(rootnode, items, xRef, yRef, 0);
        
        // check for clicked elements
        // TODO was anything clicked????
        if (viewportDimensionService.getCurrentElementCount(netlistId) == 0)
        {
            viewportDimensionService.SetZoomElementDimensions(netlistId, new DRect(0, 0, maxWidth, maxHeight, 0, null));
        }
        else if (items.Count > viewportDimensionService.getCurrentElementCount(netlistId))
        {
            // expansion
            
            viewportDimensionService.SetZoomElementDimensions(netlistId, clickedElementRect);
        }
        else
        {
            // collapse
            
            viewportDimensionService.SetZoomElementDimensions(netlistId, clickedElementParentRect);
        }
        
        viewportDimensionService.SetCurrentElementCount(netlistId, items.Count);
        
        viewportDimensionService.SetMaxHeight(netlistId, maxHeight);
        viewportDimensionService.SetMaxWidth(netlistId, maxWidth);
        
        
        logger.Log("====");
        logger.Log("All elements loaded");
        logger.Log("Statistics:");

        // Dispose of the JSON document, as we dont need to keep it around
        rootnode = new JsonObject();

        // Call the garbage collector to free dozens of MB of RAM
        // If the GC isn't called explicitly, the dead objects of the JSON file will just stay in the Gen 2 Heap
        GC.Collect();

        logger.Log("Number of Objects: " + items.Count);
        logger.Log("Number of nodes: " + nodecnt);
        logger.Log("Number of ports: " + portcnt);
        logger.Log("Number of edges: " + edgecnt);
        logger.Log("Average number of bendpoints per edge: " + ((float)bendcnt / (float)edgecnt));
        logger.Log("Number of junctions: " + junctioncnt);
        logger.Log("Number of labels: " + labelcnt);
        logger.Log("Average number of characters per label: " + ((float)charcnt / (float)labelcnt));
        logger.Log("Max width: " + maxWidth);
        logger.Log("Max height: " + maxHeight);
        
        

        //mw.UpdateScaleImpl();

        return items;
    }

    public void createNode(JsonNode node, List<NetlistElement> items, double xRef, double yRef,
        ushort depth)
    {
        if (node.AsObject().ContainsKey("node"))
        {
            logger.Log(node["id"].ToString());
        }

        JsonArray children = node["children"] as JsonArray;
        double x = 0;
        double y = 0;
        double nWidth = 0;
        double nHeight = 0;

        string celltype = "";
        string cellname = "";
        string path = "";
        string src = "";

        JsonNode layoutOptions = node["layoutOptions"] as JsonNode;

        if (node.AsObject().ContainsKey("x"))
        {
            x = node["x"].GetValue<double>();
        }

        if (node.AsObject().ContainsKey("y"))
        {
            y = node["y"].GetValue<double>();
        }

        if (node.AsObject().ContainsKey("width"))
        {
            nWidth = node["width"].GetValue<double>();
        }

        if (node.AsObject().ContainsKey("height"))
        {
            nHeight = node["height"].GetValue<double>();
        }

        if (layoutOptions.AsObject().ContainsKey("celltype"))
        {
            celltype = layoutOptions["celltype"].GetValue<string>();
        }

        if (layoutOptions.AsObject().ContainsKey("cellname"))
        {
            cellname = layoutOptions["cellname"].GetValue<string>();
        }

        if (layoutOptions.AsObject().ContainsKey("location-path"))
        {
            path = layoutOptions["location-path"].GetValue<string>();
        }

        if (layoutOptions.AsObject().ContainsKey("src-location"))
        {
            src = layoutOptions["src-location"].GetValue<string>();
        }

        if (path != string.Empty)
        {
            if (path == clickedElementPath)
            {
                clickedElementRect = new DRect(xRef + x, yRef + y, nWidth, nHeight, depth, null);
            }
            else if (path == clickedElementParentPath)
            {
                clickedElementParentRect = new DRect(xRef + x, yRef + y, nWidth, nHeight, depth, null);
            }
        } else if (clickedElementPath.Length > 0 && clickedElementParentPath == string.Empty && depth == 1)
        {
            clickedElementParentRect = new DRect(0, 0, maxWidth, maxHeight, depth, null);
        }

        if (xRef + x + nWidth > maxWidth)
        {
            maxWidth = xRef + (x + nWidth) * scale;
        }

        if (yRef + y + nHeight > maxHeight)
        {
            maxHeight = yRef + (y + nHeight) * scale;
        }

        if (!(string.Equals((string)node["id"], "root", StringComparison.Ordinal)))
        {
            items.Add(new NetlistElement()
            {
                Height = nHeight * scale,
                Width = nWidth * scale,
                xPos = xRef + x * scale,
                yPos = yRef + y * scale,
                Type = 1,
                ZIndex = depth,
                Celltype = celltype,
                Cellname = cellname,
                SrcLocation = src,
                Path = path
            });

            nodecnt++;
        }

        JsonArray labels = node["labels"] as JsonArray;

        if (labels != null)
        {
            createLabels(labels, items, xRef + x * scale, yRef + y * scale, (ushort)(depth + 1));
        }

        JsonArray ports = node["ports"] as JsonArray;

        if (ports != null)
        {
            createPorts(ports, items, xRef + x * scale, yRef + y * scale, (ushort)(depth + 1));
        }

        JsonArray edges = node["edges"] as JsonArray;

        if (edges != null)
        {
            createEdges(edges, items, xRef + x * scale, yRef + y * scale, (ushort)(depth + 1));
        }

        if (children != null)
        {
            foreach (JsonNode child in children)
            {
                createNode(child, items, xRef + x * scale, yRef + y * scale, (ushort)(depth + 1));
            }
        }
    }

    public void createLabels(JsonArray labels, List<NetlistElement> items, double xRef,
        double yRef, ushort depth)
    {
        double x = 0;
        double y = 0;
        double w = 0;
        double h = 0;
        string text = "";
        foreach (JsonNode label in labels)
        {
            x = 0;
            y = 0;
            w = 0;
            h = 0;
            text = "";

            if (label.AsObject().ContainsKey("x"))
            {
                x = label["x"].GetValue<double>();
            }

            if (label.AsObject().ContainsKey("y"))
            {
                y = label["y"].GetValue<double>();
            }

            if (label.AsObject().ContainsKey("width"))
            {
                w = label["width"].GetValue<double>();
            }

            if (label.AsObject().ContainsKey("height"))
            {
                h = label["height"].GetValue<double>();
            }

            if (label.AsObject().ContainsKey("text"))
            {
                text = label["text"].GetValue<string>();

                charcnt += text.Length;
            }

            items.Add(new NetlistElement()
            {
                LabelText = text,
                xPos = xRef + x * scale + 1, // Add small offset to separate label from border
                yPos = yRef + y * scale,
                Type = 3,
                Width = w,
                Height = h,
                ZIndex = depth
            });

            labelcnt++;
        }
    }

    public void createPorts(JsonArray ports, List<NetlistElement> items, double xRef,
        double yRef, ushort depth)
    {
        double x = 0;
        double y = 0;
        foreach (JsonNode port in ports)
        {
            x = 0;
            y = 0;
            JsonArray labels = port["labels"] as JsonArray;

            if (port.AsObject().ContainsKey("x"))
            {
                x = port["x"].GetValue<double>();
            }

            if (port.AsObject().ContainsKey("y"))
            {
                y = port["y"].GetValue<double>();
            }

            if (labels != null)
            {
                createLabels(labels, items, xRef + x * scale, yRef + y * scale, (ushort)(depth + 1));
            }

            items.Add(new NetlistElement()
            {
                xPos = xRef + x * scale,
                yPos = yRef + y * scale,
                Type = 5,
                ZIndex = depth
            });

            portcnt++;
        }
    }

    public void createEdges(JsonArray edges, List<NetlistElement> items, double xRef,
        double yRef, ushort depth)
    {
        double x = 0;
        double y = 0;
        JsonArray sections;
        JsonNode start;
        JsonNode end;
        JsonArray bendpoints;
        Point cPoint = new Point();
        Point ePoint = new Point();
        JsonNode layoutOptions;
        string locationpath;
        string src;
        string signalname;
        int indexInSignal;
        string signaltype;

        foreach (JsonNode edge in edges)
        {
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

            foreach (JsonNode section in sections)
            {
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
                        x = start["x"].GetValue<double>();
                    }

                    if (start.AsObject().ContainsKey("y"))
                    {
                        y = start["y"].GetValue<double>();
                    }

                    cPoint = new Point(x * scale, y * scale);

                    pointList.Add(cPoint);
                }

                // Add bends
                if (bendpoints != null)
                {
                    foreach (JsonNode bend in bendpoints)
                    {
                        x = 0;
                        y = 0;

                        if (bend.AsObject().ContainsKey("x"))
                        {
                            x = bend["x"].GetValue<double>();
                        }

                        if (bend.AsObject().ContainsKey("y"))
                        {
                            y = bend["y"].GetValue<double>();
                        }

                        cPoint = new Point(x * scale, y * scale);

                        pointList.Add(cPoint);

                        bendcnt++;
                    }
                }

                // Add end
                if (end != null)
                {
                    x = 0;
                    y = 0;

                    if (end.AsObject().ContainsKey("x"))
                    {
                        x = end["x"].GetValue<double>();
                    }

                    if (end.AsObject().ContainsKey("y"))
                    {
                        y = end["y"].GetValue<double>();
                    }

                    ePoint = new Point(x * scale, y * scale);

                    pointList.Add(ePoint);
                }

                // Create arrow tip
                double xDir = cPoint.X - ePoint.X;
                double yDir = cPoint.Y - ePoint.Y;

                double mag = Math.Sqrt(xDir * xDir + yDir * yDir);

                xDir /= mag;
                yDir /= mag;

                xDir *= 7 * scale;
                yDir *= 7 * scale;

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

                bendcnt += 3;

                if (layoutOptions != null)
                {
                    if (layoutOptions.AsObject().ContainsKey("src-location"))
                    {
                        src = layoutOptions["src-location"].GetValue<string>();
                    }

                    if (layoutOptions.AsObject().ContainsKey("location-path"))
                    {
                        locationpath = layoutOptions["location-path"].GetValue<string>();
                    }

                    if (layoutOptions.AsObject().ContainsKey("signalname"))
                    {
                        signalname = layoutOptions["signalname"].GetValue<string>();
                    }

                    if (layoutOptions.AsObject().ContainsKey("index-in-signal"))
                    {
                        // GetValue<int>() somehow does not (always?) work with negative integers
                        // Therefore this construct is used
                        indexInSignal = Convert.ToInt32(layoutOptions["index-in-signal"].GetValue<string>());
                    }

                    if (layoutOptions.AsObject().ContainsKey("signaltype"))
                    {
                        signaltype = layoutOptions["signaltype"].GetValue<string>();
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

                edgecnt++;
            }

            JsonArray labels = edge["labels"] as JsonArray;

            if (labels != null)
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

                createLabels(labels, items, xRef, yRef, (ushort)(depth + 1));
            }

            JsonArray junctionPoints = edge["junctionPoints"] as JsonArray;

            if (junctionPoints != null)
            {
                createJunctionPoints(junctionPoints, items, xRef, yRef, (ushort)(depth + 1));
            }
        }
    }

    public void createJunctionPoints(JsonArray junctionPoints, List<NetlistElement> items,
        double xRef, double yRef, ushort depth)
    {
        double x = 0;
        double y = 0;
        foreach (JsonNode junctionPoint in junctionPoints)
        {
            x = 0;
            y = 0;

            if (junctionPoint.AsObject().ContainsKey("x"))
            {
                x = junctionPoint["x"].GetValue<double>();
            }

            if (junctionPoint.AsObject().ContainsKey("y"))
            {
                y = junctionPoint["y"].GetValue<double>();
            }

            // Add offset to move junction point to line center
            // offset is the radius of the junction point symbol
            // x -= 3;
            // y -= 3;

            items.Add(new NetlistElement()
            {
                xPos = xRef + x * scale,
                yPos = yRef + y * scale,
                Type = 4,
                ZIndex = depth
            });

            junctioncnt++;
        }
    }

    public double GetMaxWidth()
    {
        logger.Log("Maxwidth: " + maxWidth);
        return maxWidth;
    }

    public double GetMaxHeight()
    {
        logger.Log("Maxheight: " + maxHeight);
        return maxHeight;
    }
}