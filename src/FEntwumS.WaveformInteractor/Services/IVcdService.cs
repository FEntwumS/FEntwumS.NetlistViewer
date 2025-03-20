namespace FEntwumS.WaveformInteractor.Services;

public interface IVcdService
{
    void LoadVcd(string filePath);
}

public interface IVcdScope
{
    IReadOnlyList<IVcdScope> SubScopes { get; }
    IReadOnlyList<string> Signals { get; }
}