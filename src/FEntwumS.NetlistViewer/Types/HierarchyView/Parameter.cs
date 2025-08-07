namespace FEntwumS.NetlistViewer.Types.HierarchyView;

public class Parameter
{
	private string? _name;

	public string Name
	{
		get => _name ?? "";
		set => _name = value;
	}
}