namespace FEntwumS.NetlistViewer.Types.HierarchyView;

public class HierarchyViewLabel : HierarchyViewShape
{
    private string? _content;
    private double _fontSize;

    public string Content
    {
        get => this._content ?? "";
        set => this._content = value;
    }

    public double FontSize
    {
        get => this._fontSize;
        set => this._fontSize = value;
    }
}