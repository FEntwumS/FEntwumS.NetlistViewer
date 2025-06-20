namespace FEntwumS.NetlistViewer.Types.HierarchyView;

public class HierarchyViewLabel : HierarchyViewShape
{
    private string? _content;

    public string Content
    {
        get => this._content ?? "";
        set => this._content = value;
    }
}