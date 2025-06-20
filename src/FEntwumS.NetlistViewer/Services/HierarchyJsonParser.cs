using System.Text.Json.Nodes;
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

        HierarchySideBarElement? sidebarRoot = null;

        return (null, hierarchyViewElements);
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

        if (sidebarRoot != null)
        {
            sidebarRoot.Children.Add(newSidebarElement);
        }

        return null;
    }

    private void parseSubNode(JsonNode node, List<HierarchyViewElement> hierarchyViewElements, double xRef, double yRef,
        HierarchySideBarElement currentSidebarElement)
    {
    }
}