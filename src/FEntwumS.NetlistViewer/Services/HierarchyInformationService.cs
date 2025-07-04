using System.Collections.Concurrent;
using FEntwumS.NetlistViewer.Types;

namespace FEntwumS.NetlistViewer.Services;

public class HierarchyInformationService : IHierarchyInformationService
{
    private ConcurrentDictionary<ulong, HierarchyInformation> storedData = new();

    public void setMaxHeight(ulong netlistId, double maxHeight)
    {
        if (storedData.ContainsKey(netlistId))
        {
            storedData[netlistId].MaxHeight = maxHeight;
        }
        else
        {
            storedData[netlistId] = new HierarchyInformation() { MaxHeight = maxHeight };
        }
    }

    public double getMaxHeight(ulong netlistId)
    {
        return storedData[netlistId].MaxHeight;
    }

    public void setMaxWidth(ulong netlistId, double maxWidth)
    {
        if (storedData.ContainsKey(netlistId))
        {
            storedData[netlistId].MaxWidth = maxWidth;
        }
        else
        {
            storedData[netlistId] = new HierarchyInformation() { MaxWidth = maxWidth };
        }
    }

    public double getMaxWidth(ulong netlistId)
    {
        return storedData[netlistId].MaxWidth;
    }

    public void setTopX(ulong netlistId, double topX)
    {
        if (storedData.ContainsKey(netlistId))
        {
            storedData[netlistId].TopX = topX;
        }
        else
        {
            storedData[netlistId] = new HierarchyInformation() { TopX = topX };
        }
    }

    public double getTopX(ulong netlistId)
    {
        return storedData[netlistId].TopX;
    }

    public void setTopY(ulong netlistId, double topY)
    {
        if (storedData.ContainsKey(netlistId))
        {
            storedData[netlistId].TopY = topY;
        }
        else
        {
            storedData[netlistId] = new HierarchyInformation() { TopY = topY };
        }
    }

    public double getTopY(ulong netlistId)
    {
        return storedData[netlistId].TopY;
    }

    public void setTopWidth(ulong netlistId, double topWidth)
    {
        if (storedData.ContainsKey(netlistId))
        {
            storedData[netlistId].TopWidth = topWidth;
        }
        else
        {
            storedData[netlistId] = new HierarchyInformation() { TopWidth = topWidth };
        }
    }

    public double getTopWidth(ulong netlistId)
    {
        return storedData[netlistId].TopWidth;
    }

    public void setTopHeight(ulong netlistId, double topHeight)
    {
        if (storedData.ContainsKey(netlistId))
        {
            storedData[netlistId].TopHeight = topHeight;
        }
        else
        {
            storedData[netlistId] = new HierarchyInformation() { TopHeight = topHeight };
        }
    }

    public double getTopHeight(ulong netlistId)
    {
        return storedData[netlistId].TopHeight;
    }
}