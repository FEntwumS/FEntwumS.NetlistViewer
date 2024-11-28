using OneWare.Essentials.Models;

namespace Oneware.NetlistReaderFrontend.Services;

public interface IYosysService
{
    Task<bool> LoadVhdlAsync(IProjectFile file);
    Task<bool> LoadVerilogAsync(IProjectFile file);
    Task<bool> LoadSystemVerilogAsync(IProjectFile file);
    Task<bool> CreateJsonNetlistAsync();
}