using OneWare.Essentials.Models;

namespace FEntwumS.Common.Interfaces;

public interface IYosysService : ISettingsSubscriber
{
	Task<bool> LoadVhdlAsync(IProjectFile file);
	Task<bool> LoadVerilogAsync(IProjectFile file);
	Task<bool> LoadSystemVerilogAsync(IProjectFile file);
}