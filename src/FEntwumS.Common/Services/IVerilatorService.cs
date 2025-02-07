using OneWare.Essentials.Models;

namespace FEntwumS.Common.Services;

public interface IVerilatorService
{
    public Task<bool> VerilateAsync(IProjectFile file);
    public Task<bool> CompileVerilatedAsync(IProjectFile file);

    void SetTestbench(string file);
    public string GetTestbench();
}