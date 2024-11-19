namespace Oneware.NetlistReaderFrontend.Services;

public class ViewportDimensionService : IViewportDimensionService
{
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
}