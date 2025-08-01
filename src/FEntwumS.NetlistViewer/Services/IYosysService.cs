using OneWare.Essentials.Models;

namespace FEntwumS.NetlistViewer.Services;

public interface IYosysService : ISettingsSubscriber
{
    Task<bool> LoadVhdlAsync(IProjectFile file);
    Task<bool> LoadVerilogAsync(IProjectFile file);
    Task<bool> LoadSystemVerilogAsync(IProjectFile file);
}