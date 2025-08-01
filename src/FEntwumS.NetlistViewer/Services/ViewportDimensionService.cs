using System.Collections.Concurrent;
using FEntwumS.NetlistViewer.Types;

namespace FEntwumS.NetlistViewer.Services;

public class ViewportDimensionService : IViewportDimensionService
{
    private ConcurrentDictionary<UInt64, ViewportInformation> _viewportData = new ConcurrentDictionary<UInt64, ViewportInformation>();
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

    public DRect? GetZoomElementDimensions(UInt64 netlistId)
    {
        return _viewportData[netlistId].ClickedElementBounds;
    }

    public void SetZoomElementDimensions(UInt64 netlistId, DRect? ZoomElementDimensions)
    {
        if (_viewportData.ContainsKey(netlistId))
        {
            _viewportData[netlistId].ClickedElementBounds = ZoomElementDimensions;
        }
        else
        {
            _viewportData[netlistId] = new ViewportInformation{ ClickedElementBounds = ZoomElementDimensions };
        }
    }

    public string? GetClickedElementPath(UInt64 netlistId)
    {
        return _viewportData[netlistId].ClickedElementPath;
    }

    public void SetClickedElementPath(UInt64 netlistId, string? ClickedElementName)
    {
        if (_viewportData.ContainsKey(netlistId))
        {
            _viewportData[netlistId].ClickedElementPath = ClickedElementName;
        }
        else
        {
            _viewportData[netlistId] = new ViewportInformation{ ClickedElementPath = ClickedElementName };
        }
    }

    public int getCurrentElementCount(UInt64 netlistId)
    {
        return _viewportData[netlistId].CurrentElementCount;
    }

    public void SetCurrentElementCount(UInt64 netlistId, int CurrentElementCount)
    {
        if (_viewportData.ContainsKey(netlistId))
        {
            _viewportData[netlistId].CurrentElementCount = CurrentElementCount;
        }
        else
        {
            _viewportData[netlistId] = new ViewportInformation{ CurrentElementCount = CurrentElementCount };
        }
    }

    public void SetMaxHeight(UInt64 netlistId, double MaxHeight)
    {
        _viewportData[netlistId].MaxHeight = MaxHeight;
    }

    public double GetMaxHeight(UInt64 netlistId)
    {
        return _viewportData[netlistId].MaxHeight;
    }

    public void SetMaxWidth(UInt64 netlistId, double MaxWidth)
    {
        _viewportData[netlistId].MaxWidth = MaxWidth;
    }

    public double GetMaxWidth(UInt64 netlistId)
    {
        return _viewportData[netlistId].MaxWidth;
    }
}