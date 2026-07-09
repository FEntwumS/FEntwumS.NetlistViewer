using FEntwumS.Common.Types;
using OneWare.Essentials.Models;

namespace FEntwumS.Common.Interfaces;

public interface INetlistGenerator : ISettingsSubscriber
{
	public Task<bool> GenerateVhdlNetlistAsync(IProjectFile vhdlProject, string topEntityName);
	public Task<bool> GenerateVerilogNetlistAsync(IProjectFile verilogProject, string topEntityName);
	public Task<bool> GenerateSystemVerilogNetlistAsync(IProjectFile systemVerilogProject, string topEntityName);

	public Task<(IProjectFile? netlistFile, bool success)> GenerateNetlistAsync(IProjectFile projectFile,
		NetlistLanguage netlistLanguage, NetlistType netlistType, string topEntityName);

	public (IProjectFile? netlistFile, bool success) GetExistingNetlist(IProjectFile projectFile,
		NetlistType netlistType);
}