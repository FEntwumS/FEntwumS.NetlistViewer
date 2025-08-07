namespace FEntwumS.NetlistViewer.Types.HierarchyView;

public class HierarchyViewShape : HierarchyViewElement
{
	private double _width;
	private double _height;

	public double Width
	{
		get => this._width;
		set => this._width = value;
	}

	public double Height
	{
		get => this._height;
		set => this._height = value;
	}
}