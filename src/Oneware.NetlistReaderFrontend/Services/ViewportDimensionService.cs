using Oneware.NetlistReaderFrontend.Types;

namespace Oneware.NetlistReaderFrontend.Services;

public class ViewportDimensionService : IViewportDimensionService
{
    private double width { get; set; }
    private double height { get; set; }
    
    private string clickedElementPath { get; set; }
    private int currentElementCount { get; set; }
    private DRect zoomElement { get; set; }

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

    public DRect GetZoomElementDimensions()
    {
        return zoomElement;
    }

    public void SetZoomElementDimensions(DRect ZoomElementDimensions)
    {
        zoomElement = ZoomElementDimensions;
    }

    public string GetClickedElementPath()
    {
        return clickedElementPath;
    }

    public void SetClickedElementPath(string ClickedElementName)
    {
        clickedElementPath = ClickedElementName;
    }

    public int getCurrentElementCount()
    {
        return currentElementCount;
    }

    public void SetCurrentElementCount(int CurrentElementCount)
    {
        currentElementCount = CurrentElementCount;
    }
}