using System.Text.Json.Nodes;
using Avalonia;
using Avalonia.Media;
using FEntwumS.NetlistViewer.Assets;
using FEntwumS.NetlistViewer.Types.HierarchyView;

namespace FEntwumS.NetlistViewer.Services;

public class HierarchyJsonParser : IHierarchyJsonParser
{
	private readonly ICustomLogger _logger;
	private readonly IHierarchyInformationService _hierarchyInformationService;

	public HierarchyJsonParser()
	{
		_logger = ServiceManager.GetService<ICustomLogger>();
		_hierarchyInformationService = ServiceManager.GetService<IHierarchyInformationService>();
	}


	public async Task<(HierarchySideBarElement? sidebarRoot, List<HierarchyViewElement>? hierarchyViewElements)>
		LoadHierarchyAsync(Stream hierarchyStream, ulong netlistId)
	{
		List<HierarchyViewElement> hierarchyViewElements = new List<HierarchyViewElement>();
		JsonNode? rootNode = await JsonNode.ParseAsync(hierarchyStream);

		Dictionary<string, HierarchySideBarElement> nodeNameMap = new Dictionary<string, HierarchySideBarElement>();

		if (rootNode == null)
		{
			_logger.Error("Failed to parse hierarchy JSON");

			return (null, null);
		}

		HierarchySideBarElement? sidebarRoot = parseRootNode(rootNode, hierarchyViewElements, nodeNameMap, netlistId);

		return (sidebarRoot, hierarchyViewElements);
	}

	private HierarchySideBarElement? parseRootNode(JsonNode node, List<HierarchyViewElement> hierarchyViewElements,
		Dictionary<string, HierarchySideBarElement> nodeNameMap, ulong netlistId)
	{
		double x = 0,
			y = 0,
			width = 0,
			height = 0;
		JsonArray? containerNodes = node["children"] as JsonArray;
		JsonArray? edges = node["edges"] as JsonArray;
		HierarchySideBarElement? sidebarRoot = null;

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
			width = node["width"]!.GetValue<double>();
		}

		if (node.AsObject().ContainsKey("height"))
		{
			height = node["height"]!.GetValue<double>();
		}

		_hierarchyInformationService.setMaxHeight(netlistId, height);
		_hierarchyInformationService.setMaxWidth(netlistId, width);

		bool firstNode = true;

		if (containerNodes != null)
		{
			foreach (JsonNode? containerNode in containerNodes)
			{
				if (containerNode == null)
				{
					continue;
				}

				if (firstNode == true)
				{
					firstNode = false;
					sidebarRoot = parseContainerNode(containerNode, hierarchyViewElements, 0, 0,
						new HierarchySideBarElement(), nodeNameMap);

					var firstElem = hierarchyViewElements.First();

					_hierarchyInformationService.setTopX(netlistId, firstElem.X);
					_hierarchyInformationService.setTopY(netlistId, firstElem.Y);
					_hierarchyInformationService.setTopHeight(netlistId, ((HierarchyViewNode)firstElem).Height);
					_hierarchyInformationService.setTopWidth(netlistId, ((HierarchyViewNode)firstElem).Width);
				}
				else
				{
					parseContainerNode(containerNode, hierarchyViewElements, x, y, sidebarRoot, nodeNameMap);
				}
			}
		}

		if (edges != null)
		{
			foreach (JsonNode? edge in edges)
			{
				if (edge == null)
				{
					continue;
				}

				parseEdge(edge, hierarchyViewElements, 0, 0);
			}
		}

		return sidebarRoot;
	}

	private HierarchySideBarElement? parseContainerNode(JsonNode node, List<HierarchyViewElement> hierarchyViewElements,
		double xRef, double yRef, HierarchySideBarElement? sidebarRoot,
		Dictionary<string, HierarchySideBarElement> nodeNameMap)
	{
		HierarchySideBarElement newSidebarElement = new HierarchySideBarElement();
		JsonArray? subNodes = node["children"] as JsonArray;
		JsonNode? layoutOptions = node["layoutOptions"] as JsonNode;
		string ancestorName = "";
		HierarchySideBarElement? ancestor = null;

		double x = 0,
			y = 0;

		if (layoutOptions is null)
		{
			return newSidebarElement;
		}

		if (node.AsObject().ContainsKey("x"))
		{
			x = node["x"]!.GetValue<double>();
		}

		if (node.AsObject().ContainsKey("y"))
		{
			y = node["y"]!.GetValue<double>();
		}

		if (layoutOptions.AsObject().ContainsKey("hierarchy-ancestor-path"))
		{
			ancestorName = layoutOptions["hierarchy-ancestor-path"]!.GetValue<string>();

			if (ancestorName.Contains(" "))
			{
				ancestorName = ancestorName.Substring(0, ancestorName.LastIndexOf(" ", StringComparison.Ordinal));
			}

			ancestor = nodeNameMap!.GetValueOrDefault(ancestorName, null);
		}

		if (ancestor is not null)
		{
			ancestor.Children.Add(newSidebarElement);
		}
		else if (sidebarRoot != null)
		{
			sidebarRoot.Children.Add(newSidebarElement);
		}

		parseNode(node, hierarchyViewElements, xRef, yRef);

		if (subNodes != null)
		{
			foreach (JsonNode? subNode in subNodes)
			{
				parseSubNode(subNode!, hierarchyViewElements, xRef + x, yRef + y, newSidebarElement, nodeNameMap);
			}
		}

		return sidebarRoot;
	}

	private void parseSubNode(JsonNode node, List<HierarchyViewElement> hierarchyViewElements, double xRef, double yRef,
		HierarchySideBarElement currentSidebarElement, Dictionary<string, HierarchySideBarElement> nodeNameMap)
	{
		JsonArray? labels = node["labels"] as JsonArray;
		JsonArray? ports = node["ports"] as JsonArray;
		JsonNode? layoutOptions = node["layoutOptions"] as JsonNode;
		double x = 0, y = 0;
		string namePath = "";

		if (layoutOptions is null)
		{
			return;
		}

		if (node.AsObject().ContainsKey("x"))
		{
			x = node["x"]!.GetValue<double>();
		}

		if (node.AsObject().ContainsKey("y"))
		{
			y = node["y"]!.GetValue<double>();
		}

		if (!layoutOptions.AsObject().ContainsKey("hierarchy-container-sub-node-type"))
		{
			return;
		}

		if (layoutOptions.AsObject().ContainsKey("hierarchy-ancestor-path"))
		{
			namePath = layoutOptions["hierarchy-ancestor-path"]!.GetValue<string>();
		}

		if (labels is null && ports is null)
		{
			return;
		}

		parseNode(node, hierarchyViewElements, xRef, yRef);

		if (labels is not null && labels.Count == 0)
		{
			return;
		}

		switch (layoutOptions["hierarchy-container-sub-node-type"]!.GetValue<string>())
		{
			case "NAME":
				string name = parseLabel(labels![0], hierarchyViewElements, xRef + x, yRef + y);
				nodeNameMap.Add(namePath, currentSidebarElement);
				currentSidebarElement.Name = name;
				parseLabel(labels[1], hierarchyViewElements, xRef + x, yRef + y);
				break;
			case "TYPE":
				currentSidebarElement.Type = parseLabel(labels![0], hierarchyViewElements, xRef + x, yRef + y);
				parseLabel(labels[1], hierarchyViewElements, xRef + x, yRef + y);
				break;
			case "PARAMETERS":
				parseLabel(labels![0], hierarchyViewElements, xRef + x, yRef + y);

				if (ports is not null)
				{
					foreach (JsonNode? parameter in ports)
					{
						if (parameter is null)
						{
							continue;
						}

						
						currentSidebarElement.Attributes.Add(new Parameter()
							{ Name = parsePort(parameter, hierarchyViewElements, xRef + x, yRef + y).Name });
					}
				}

				break;
			case "PORTS":
				parseLabel(labels![0], hierarchyViewElements, xRef + x, yRef + y);

				if (ports is not null)
				{
					foreach (JsonNode? port in ports)
					{
						if (port is null)
						{
							continue;
						}
						
						currentSidebarElement.Ports.Add(parsePort(port, hierarchyViewElements, xRef + x, yRef + y));
					}
				}

				break;
			default:
				return;
		}
	}

	private string parseLabel(JsonNode? labelNode, List<HierarchyViewElement> hierarchyViewElements, double xRef,
		double yRef)
	{
		if (labelNode is null)
		{
			return string.Empty;
		}
		
		JsonNode? layoutOptions = labelNode["layoutOptions"];
		double x = 0,
			y = 0,
			width = 0,
			height = 0,
			fontSize = 10;
		string text = "";

		if (labelNode.AsObject().ContainsKey("x"))
		{
			x = labelNode["x"]!.GetValue<double>();
		}

		if (labelNode.AsObject().ContainsKey("y"))
		{
			y = labelNode["y"]!.GetValue<double>();
		}

		if (labelNode.AsObject().ContainsKey("width"))
		{
			width = labelNode["width"]!.GetValue<double>();
		}

		if (labelNode.AsObject().ContainsKey("height"))
		{
			height = labelNode["height"]!.GetValue<double>();
		}

		if (labelNode.AsObject().ContainsKey("text"))
		{
			text = labelNode["text"]!.GetValue<string>();
		}

		if (layoutOptions != null && layoutOptions.AsObject().ContainsKey("font-size"))
		{
			fontSize = double.Parse(layoutOptions["font-size"]!.GetValue<string>(),
				System.Globalization.CultureInfo.InvariantCulture);
		}

		hierarchyViewElements.Add(new HierarchyViewLabel()
		{
			Content = text,
			X = xRef + x,
			Y = yRef + y,
			Width = width,
			Height = height,
			FontSize = fontSize
		});

		return text;
	}

	private Port parsePort(JsonNode portNode, List<HierarchyViewElement> hierarchyViewElements, double xRef,
		double yRef)
	{
		JsonNode? layoutOptions = portNode["layoutOptions"];
		JsonArray? labels = portNode["labels"] as JsonArray;
		double x = 0,
			y = 0,
			width = 0,
			height = 0;
		StreamGeometry geometry = AppIcons.INOUT;

		if (portNode.AsObject().ContainsKey("x"))
		{
			x = portNode["x"]!.GetValue<double>();
		}

		if (portNode.AsObject().ContainsKey("y"))
		{
			y = portNode["y"]!.GetValue<double>();
		}

		if (portNode.AsObject().ContainsKey("width"))
		{
			width = portNode["width"]!.GetValue<double>();
		}

		if (portNode.AsObject().ContainsKey("height"))
		{
			height = portNode["height"]!.GetValue<double>();
		}

		if (layoutOptions != null && layoutOptions.AsObject().ContainsKey("port-direction"))
		{
			geometry = layoutOptions["port-direction"]!.GetValue<string>() switch
			{
				"IN" => AppIcons.IN,
				"OUT" => AppIcons.OUT,
				"INOUT" => AppIcons.INOUT,
				_ => AppIcons.INOUT
			};
		}

		hierarchyViewElements.Add(new HierarchyViewPort()
		{
			Geometry = geometry,
			Height = height,
			Width = width,
			X = xRef + x,
			Y = yRef + y
		});

		if (labels is null || labels.Count == 0)
		{
			return new Port();
		}

		return new Port()
		{
			Geometry = geometry,
			Name = parseLabel(labels[0], hierarchyViewElements, xRef + x, yRef + y)
		};
	}

	private void parseNode(JsonNode node, List<HierarchyViewElement> hierarchyViewElements, double xRef, double yRef)
	{
		double x = 0,
			y = 0,
			width = 0,
			height = 0;

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
			width = node["width"]!.GetValue<double>();
		}

		if (node.AsObject().ContainsKey("height"))
		{
			height = node["height"]!.GetValue<double>();
		}

		hierarchyViewElements.Add(new HierarchyViewNode()
		{
			X = xRef + x,
			Y = yRef + y,
			Width = width,
			Height = height
		});
	}

	private void parseEdge(JsonNode edge, List<HierarchyViewElement> hierarchyViewElements, double xRef, double yRef)
	{
		JsonArray? sections = edge["sections"] as JsonArray;

		if (sections is null)
		{
			return;
		}

		foreach (JsonNode? section in sections)
		{
			if (section is null)
			{
				continue;
			}

			JsonNode? start = section["startPoint"], end = section["endPoint"];
			JsonArray? bendpoints = section["bendPoints"] as JsonArray;
			List<Point> pointList = new List<Point>();
			double x = 0.0d, y = 0.0d;
			Point cPoint = new Point(), ePoint = new Point();

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

				cPoint = new Point(x, y);

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

					cPoint = new Point(x, y);

					pointList.Add(cPoint);
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

				ePoint = new Point(x, y);

				pointList.Add(ePoint);
			}

			// Create arrow tip
			double xDir = cPoint.X - ePoint.X;
			double yDir = cPoint.Y - ePoint.Y;

			double mag = Math.Sqrt(xDir * xDir + yDir * yDir);

			xDir /= mag;
			yDir /= mag;

			xDir *= 7;
			yDir *= 7;

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

			hierarchyViewElements.Add(new HierarchyViewEdge()
			{
				X = xRef,
				Y = yRef,
				Points = pointList
			});
		}
	}
}