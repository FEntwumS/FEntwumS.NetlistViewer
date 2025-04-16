namespace FEntwumS.Common.Types;

public record ViewportInformation
{
    public double MaxWidth { get; set; }
    public double MaxHeight { get; set; }
    public DRect? ClickedElementBounds { get; set; }
    public string? ClickedElementPath { get; set; }
    public int CurrentElementCount { get; set; }
};