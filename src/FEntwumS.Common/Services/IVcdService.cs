
using FEntwumS.Common.Types;
namespace FEntwumS.Common.Services;

public interface IVcdService
{
    VcdScope? RootScope { get; }
    void LoadVcd(string filePath);
    void WriteVcd(string inputFilePath, string outputFilePath);
    void Reset();
    void RecreateVcdHierarchy();
    string LoadVcdAndHashBody(string vcdPath);
    string HashVcd(StreamReader reader);
}

public interface IVcdScope
{
    string Name { get; }
    IReadOnlyList<IVcdScope> SubScopes { get; }
    IReadOnlyList<ISignal> Signals { get; }
}

public interface ISignal
{
    string Type { get; }
    int BitWidth { get; }
    string Id { get; }
    string Name { get; }
    string Value { get; }
}