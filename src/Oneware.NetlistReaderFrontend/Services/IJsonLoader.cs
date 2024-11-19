using System.Text.Json.Nodes;
using Avalonia.Collections;
using Avalonia.Platform.Storage;
using Oneware.NetlistReaderFrontend.Types;
using Oneware.NetlistReaderFrontend.ViewModels;

namespace Oneware.NetlistReaderFrontend.Services;

public interface IJsonLoader
{
    public Task OpenJson(IStorageFile netlist);

    public Task parseJson(AvaloniaList<NetlistElement> items, double xRef, double yRef,
        FrontendViewModel mw);

    public void createNode(JsonNode node, AvaloniaList<NetlistElement> items, double xRef, double
        yRef, ushort depth);

    public void createLabels(JsonArray labels, AvaloniaList<NetlistElement> items, double xRef,
        double yRef, ushort depth);

    public void createPorts(JsonArray ports, AvaloniaList<NetlistElement> items, double xRef,
        double yRef, ushort depth);

    public void createEdges(JsonArray edges, AvaloniaList<NetlistElement> items, double xRef,
        double yRef, ushort depth);

    public void createJunctionPoints(JsonArray junctionPoints, AvaloniaList<NetlistElement> items,
        double xRef, double yRef, ushort depth);

    public double GetMaxWidth();
    public double GetMaxHeight();
}