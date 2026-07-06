using System.Text.Json.Nodes;
using FEntwumS.Common.Types;
using FEntwumS.Common.ViewModels;

namespace FEntwumS.Common.Interfaces;

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
		double xRef, double yRef, ushort depth, JunctionShape junctionShape);

	public double GetMaxWidth();
	public double GetMaxHeight();
}