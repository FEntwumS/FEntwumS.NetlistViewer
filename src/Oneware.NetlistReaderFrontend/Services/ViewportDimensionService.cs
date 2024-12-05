using System.Collections.Concurrent;
using Oneware.NetlistReaderFrontend.Types;

namespace Oneware.NetlistReaderFrontend.Services;

public class ViewportDimensionService : IViewportDimensionService
{
    private ConcurrentDictionary<UInt64, ViewportInformation> specificData = new ConcurrentDictionary<UInt64, ViewportInformation>();
    private double width { get; set; }
    private double height { get; set; }

    public void SetHeight(double Height)
    {
        height = Height;
    }

    public double GetHeight()
    {
        return height;
    }

    public void SetWidth(double Width)
    {
        width = Width;
    }

    public double GetWidth()
    {
        return width;
    }

    public DRect GetZoomElementDimensions(UInt64 netlistId)
    {
        return specificData[netlistId].ClickedElementBounds;
    }

    public void SetZoomElementDimensions(UInt64 netlistId, DRect ZoomElementDimensions)
    {
        if (specificData.ContainsKey(netlistId))
        {
            specificData[netlistId].ClickedElementBounds = ZoomElementDimensions;
        }
        else
        {
            specificData[netlistId] = new ViewportInformation{ ClickedElementBounds = ZoomElementDimensions };
        }
    }

    public string GetClickedElementPath(UInt64 netlistId)
    {
        return specificData[netlistId].ClickedElementPath;
    }

    public void SetClickedElementPath(UInt64 netlistId, string ClickedElementName)
    {
        if (specificData.ContainsKey(netlistId))
        {
            specificData[netlistId].ClickedElementPath = ClickedElementName;
        }
        else
        {
            specificData[netlistId] = new ViewportInformation{ ClickedElementPath = ClickedElementName };
        }
    }

    public int getCurrentElementCount(UInt64 netlistId)
    {
        return specificData[netlistId].CurrentElementCount;
    }

    public void SetCurrentElementCount(UInt64 netlistId, int CurrentElementCount)
    {
        if (specificData.ContainsKey(netlistId))
        {
            specificData[netlistId].CurrentElementCount = CurrentElementCount;
        }
        else
        {
            specificData[netlistId] = new ViewportInformation{ CurrentElementCount = CurrentElementCount };
        }
    }
}