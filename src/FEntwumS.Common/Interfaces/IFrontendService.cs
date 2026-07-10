using FEntwumS.Common.ViewModels;
using OneWare.Essentials.Models;

namespace FEntwumS.Common.Interfaces;

public interface IFrontendService : ISettingsSubscriber
{
	public Task CreateVhdlNetlistAsync(IProjectFile vhdl, string topEntityName);

	public Task CreateVerilogNetlistAsync(IProjectFile verilog, string topEntityName);

	public Task CreateSystemVerilogNetlistAsync(IProjectFile sVerilog, string topEntityName);

	public Task ShowViewerAsync(IProjectFile json);

	public Task ExpandNodeAsync(string? nodePath, FrontendViewModel vm);

	public Task<bool> StartBackendIfNotStartedAsync();

	public Task<bool> ServerStartedAsync();

	public Task CloseNetlistOnServerAsync(UInt64 netlistId);

	public Task CreateVhdlHierarchyAsync(IProjectFile vhdlFile);
	public Task CreateVerilogHierarchyAsync(IProjectFile verilogFile);
	public Task CreateSystemVerilogHierarchyAsync(IProjectFile systemVerilogFile);
}