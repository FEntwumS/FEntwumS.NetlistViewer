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
}