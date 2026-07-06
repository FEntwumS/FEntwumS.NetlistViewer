namespace FEntwumS.Common.Types;

public class Signal
{
    public string Type { get; }
    public int BitWidth { get; }
    public string Id { get; }
    public string Name { get; }
    public string Value { get; private set; } = "0";

    public Signal(string type, int bitWidth, string id, string name)
    {
        Type = type;
        BitWidth = bitWidth;
        Id = id;
        Name = name;
    }
}