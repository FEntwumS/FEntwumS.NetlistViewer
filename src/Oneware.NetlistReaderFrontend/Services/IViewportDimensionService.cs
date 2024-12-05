using Oneware.NetlistReaderFrontend.Types;

namespace Oneware.NetlistReaderFrontend.Services;

public interface IViewportDimensionService
{
    public void SetHeight(double Height);
    public double GetHeight();

    public void SetWidth(double Width);
    public double GetWidth();
    
    public DRect GetZoomElementDimensions();
    public void SetZoomElementDimensions(DRect ZoomElementDimensions);
    
    public string GetClickedElementPath();
    public void SetClickedElementPath(string ClickedElementName);
    
    public int getCurrentElementCount();
    public void SetCurrentElementCount(int CurrentElementCount);
}