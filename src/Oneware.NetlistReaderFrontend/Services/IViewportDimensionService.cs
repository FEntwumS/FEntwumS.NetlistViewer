using Oneware.NetlistReaderFrontend.Types;

namespace Oneware.NetlistReaderFrontend.Services;

public interface IViewportDimensionService
{
    public void SetHeight(double Height);
    public double GetHeight();

    public void SetWidth(double Width);
    public double GetWidth();
    
    public DRect GetZoomElementDimensions(UInt64 netlistId);
    public void SetZoomElementDimensions(UInt64 netlistId, DRect ZoomElementDimensions);
    
    public string GetClickedElementPath(UInt64 netlistId);
    public void SetClickedElementPath(UInt64 netlistId, string ClickedElementName);
    
    public int getCurrentElementCount(UInt64 netlistId);
    public void SetCurrentElementCount(UInt64 netlistId, int CurrentElementCount);
    
    public void SetMaxHeight(UInt64 netlistId, Double MaxHeight);
    public double GetMaxHeight(UInt64 netlistId);
    public void SetMaxWidth(UInt64 netlistId, Double MaxWidth);
    public double GetMaxWidth(UInt64 netlistId);
}