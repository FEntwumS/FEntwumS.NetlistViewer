using System.Text.Json.Nodes;
using Avalonia.Collections;
using Avalonia.Platform.Storage;
using FEntwumS.NetlistViewer.Types;
using FEntwumS.NetlistViewer.ViewModels;

namespace FEntwumS.NetlistViewer.Services;

public interface IJsonLoader
{
    public Task OpenJson(Stream netlist, UInt64 netlistId);

    public Task<List<NetlistElement>> parseJson(double xRef, double yRef,
        FrontendViewModel mw, UInt64 netlistId);

    public void createNode(JsonNode node, List<NetlistElement> items, double xRef, double
        yRef, ushort depth);

    public void createLabels(JsonArray labels, List<NetlistElement> items, double xRef,
        double yRef, ushort depth);

    public void createPorts(JsonArray ports, List<NetlistElement> items, double xRef,
        double yRef, ushort depth);

    public void createEdges(JsonArray edges, List<NetlistElement> items, double xRef,
        double yRef, ushort depth);

    public void createJunctionPoints(JsonArray junctionPoints, List<NetlistElement> items,
        double xRef, double yRef, ushort depth);

    public double GetMaxWidth();
    public double GetMaxHeight();
}