using System.Text.Json.Nodes;
using FEntwumS.Common.Controls;
using FEntwumS.Common.Types;

namespace FEntwumS.Common.Interfaces;

public interface IJsonLoader
{

	public (GraphNodeControl, GraphNodeControl?) ParseJson(double xRef, double yRef,
		Stream netlistStream, UInt64 netlistId, string clickedElementPath = "");

	public (GraphNodeControl?, GraphNodeControl?) CreateNode(JsonNode node, GraphNodeControl parent, double xRef, double
		yRef, ushort depth, string clickedElementPath, string clickedElementParentPath);

	public void CreateLabels(JsonArray labels, GraphNodeControl parent, double xRef,
		double yRef, ushort depth);

	public void CreatePorts(JsonArray ports, GraphNodeControl parent, double xRef,
		double yRef, ushort depth);

	public void CreateEdges(JsonArray edges, GraphNodeControl parent, double xRef,
		double yRef, ushort depth);

	public void CreateJunctionPoints(JsonArray junctionPoints, GraphNodeControl parent,
		double xRef, double yRef, ushort depth, JunctionShape junctionShape);

	public double GetMaxWidth();
	public double GetMaxHeight();
}