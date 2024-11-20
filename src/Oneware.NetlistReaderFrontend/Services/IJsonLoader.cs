using System.Text.Json.Nodes;
using Avalonia.Collections;
using Avalonia.Platform.Storage;
using Oneware.NetlistReaderFrontend.Types;
using Oneware.NetlistReaderFrontend.ViewModels;

namespace Oneware.NetlistReaderFrontend.Services;

public interface IJsonLoader
{
    public Task OpenJson(FileStream netlist);

    public Task<List<NetlistElement>> parseJson(double xRef, double yRef,
        FrontendViewModel mw);

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