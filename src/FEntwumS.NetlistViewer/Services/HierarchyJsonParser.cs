using System.Text.Json.Nodes;
using Avalonia.Media;
using FEntwumS.NetlistViewer.Assets;
using FEntwumS.NetlistViewer.Types.HierarchyView;

namespace FEntwumS.NetlistViewer.Services;

public class HierarchyJsonParser : IHierarchyJsonParser
{
    private readonly ICustomLogger _logger;

    public HierarchyJsonParser()
    {
        this._logger = new CustomLogger();
    }


    public async Task<(HierarchySideBarElement? sidebarRoot, List<HierarchyViewElement>? hierarchyViewElements)>
        LoadHierarchyAsync(Stream hierarchyStream)
    {
        List<HierarchyViewElement> hierarchyViewElements = new List<HierarchyViewElement>();
        JsonNode? rootNode = await JsonNode.ParseAsync(hierarchyStream);

        if (rootNode == null)
        {
            _logger.Error("Failed to parse hierarchy JSON");

            return (null, null);
        }

        HierarchySideBarElement? sidebarRoot = parseRootNode(rootNode, hierarchyViewElements);

        return (sidebarRoot, hierarchyViewElements);
    }

    private HierarchySideBarElement? parseRootNode(JsonNode node, List<HierarchyViewElement> hierarchyViewElements)
    {
        double width = 0, height = 0;
        JsonArray? containerNodes = node["children"] as JsonArray;
        HierarchySideBarElement? sidebarRoot = null;

        if (node.AsObject().ContainsKey("width"))
        {
            width = node["width"]!.GetValue<double>();
        }

        if (node.AsObject().ContainsKey("height"))
        {
            height = node["height"]!.GetValue<double>();
        }

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
                    sidebarRoot = parseContainerNode(containerNode, hierarchyViewElements, 0, 0, null);
                }
                else
                {
                    parseContainerNode(containerNode, hierarchyViewElements, width, height, sidebarRoot);
                }
            }
        }

        return sidebarRoot;
    }

    private HierarchySideBarElement parseContainerNode(JsonNode node, List<HierarchyViewElement> hierarchyViewElements,
        double xRef, double yRef, HierarchySideBarElement? sidebarRoot)
    {
        HierarchySideBarElement newSidebarElement = new HierarchySideBarElement();
        JsonArray? subNodes = node["children"] as JsonArray;

        if (sidebarRoot != null)
        {
            sidebarRoot.Children.Add(newSidebarElement);
        }
        
        parseNode(node, hierarchyViewElements, xRef, yRef);

        if (subNodes != null)
        {
            foreach (JsonNode? subNode in subNodes)
            {
                parseSubNode(subNode!, hierarchyViewElements, xRef, yRef, newSidebarElement);
            }
        }

        return null;
    }

    private void parseSubNode(JsonNode node, List<HierarchyViewElement> hierarchyViewElements, double xRef, double yRef,
        HierarchySideBarElement currentSidebarElement)
    {
        JsonArray? labels = node["labels"] as JsonArray;
        JsonArray? ports = node["ports"] as JsonArray;
        JsonNode? layoutOptions = node["layoutOptions"] as JsonNode;

        if (layoutOptions is null)
        {
            return;
        }

        if (!layoutOptions.AsObject().ContainsKey("hierarchy-container-sub-node-type"))
        {
            return;
        }

        if (labels is null || ports is null)
        {
            return;
        }
        
        parseNode(node, hierarchyViewElements, xRef, yRef);

        if (labels.Count > 1)
        {
            _logger.Error("Multiple labels on subnode");
        }

        if (labels.Count == 0)
        {
            return;
        }

        switch (layoutOptions["hierarchy-container-sub-node-type"]!.GetValue<string>())
        {
            case "NAME":
                currentSidebarElement.Name = parseLabel(labels[0], hierarchyViewElements, xRef, yRef);
                break;
            case "TYPE":
                currentSidebarElement.Type = parseLabel(labels[0], hierarchyViewElements, xRef, yRef);
                break;
            case "PARAMETERS":
                foreach (JsonNode? parameter in ports)
                {
                    currentSidebarElement.Attributes.Add(parsePort(parameter, hierarchyViewElements, xRef, yRef));
                }

                break;
            case "PORTS":
                foreach (JsonNode? port in ports)
                {
                    currentSidebarElement.Ports.Add(parsePort(port, hierarchyViewElements, xRef, yRef));
                }

                break;
            default:
                return;
        }
    }

    private string parseLabel(JsonNode labelNode, List<HierarchyViewElement> hierarchyViewElements, double xRef,
        double yRef)
    {
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

    private string parsePort(JsonNode portNode, List<HierarchyViewElement> hierarchyViewElements, double xRef,
        double yRef)
    {
        JsonNode? layoutOptions = portNode["layoutOptions"];
        JsonArray? labels = portNode["labels"] as JsonArray;
        double x = 0,
            y = 0,
            width = 0,
            height = 0;
        StreamGeometry? geometry = null;

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
                _ => null
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
            return "";
        }

        return parseLabel(labels[0], hierarchyViewElements, xRef, yRef);
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
}