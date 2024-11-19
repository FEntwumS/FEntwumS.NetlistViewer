namespace Oneware.NetlistReaderFrontend.Services;

public interface IViewportDimensionService
{
    public void SetHeight(double Height);
    public double GetHeight();

    public void SetWidth(double Width);
    public double GetWidth();
}