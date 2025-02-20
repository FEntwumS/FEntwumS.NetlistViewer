using OneWare.Essentials.Models;

namespace FEntwumS.Common.Services;

public interface IVerilatorService
{
    public Task<bool> VerilateAsync(IProjectFile file);
    public Task<bool> CompileVerilatedAsync(IProjectFile topLevelFile);
    public Task<bool> RunExecutableAsync(IProjectFile topLevelFile);
    
    IProjectFile? Testbench { get; set; }
}