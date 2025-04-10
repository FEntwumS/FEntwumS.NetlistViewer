namespace FEntwumS.Common.Types;

public class VcdScope
{
    public string Name { get; set; }
    public List<VcdScope?> SubScopes { get; } = new();
    public List<Signal> Signals { get; } = new();
    public VcdScope? parent;

    public VcdScope(string name, VcdScope? parent)
    {
        Name = name;
        this.parent = parent;
    }
}