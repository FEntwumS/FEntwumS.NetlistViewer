using System.Text.Json.Nodes;
using Avalonia.Collections;
using Avalonia.Platform.Storage;
using FEntwumS.NetlistViewer.Types;
using FEntwumS.NetlistViewer.ViewModels;

namespace FEntwumS.NetlistViewer.Services;

public interface IJsonLoader
{
	public Task OpenJsonAsync(Stream netlist, UInt64 netlistId);

	public Task<List<NetlistElement>> ParseJsonAsync(double xRef, double yRef,
		FrontendViewModel mw, UInt64 netlistId);

	public void CreateNode(JsonNode node, List<NetlistElement> items, double xRef, double
		yRef, ushort depth);

	public void CreateLabels(JsonArray labels, List<NetlistElement> items, double xRef,
		double yRef, ushort depth);

	public void CreatePorts(JsonArray ports, List<NetlistElement> items, double xRef,
		double yRef, ushort depth);

	public void CreateEdges(JsonArray edges, List<NetlistElement> items, double xRef,
		double yRef, ushort depth);

	public void CreateJunctionPoints(JsonArray junctionPoints, List<NetlistElement> items,
		double xRef, double yRef, ushort depth);

	public double GetMaxWidth();
	public double GetMaxHeight();
}