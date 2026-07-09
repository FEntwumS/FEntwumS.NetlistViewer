using OneWare.Essentials.Models;

namespace FEntwumS.Common.Interfaces;

public interface IYosysService : ISettingsSubscriber
{
	Task<bool> LoadVhdlAsync(IProjectFile file, string topEntityName);
	Task<bool> LoadVerilogAsync(IProjectFile file, string topEntityName);
	Task<bool> LoadSystemVerilogAsync(IProjectFile file, string topEntityName);
}