using OneWare.Essentials.Models;
using OneWare.Essentials.ViewModels;

namespace FEntwumS.Common.Services;

public interface IFrontendService
{
    public void SubscribeToSettings();

    public Task CreateVhdlNetlistAsync(IProjectFile vhdl);

    public Task CreateVerilogNetlistAsync(IProjectFile verilog);

    public Task CreateSystemVerilogNetlistAsync(IProjectFile sVerilog);

    public Task ShowViewerAsync(IProjectFile json);

    public Task ExpandNodeAsync(string? nodePath, ExtendedTool vm);

    public Task<bool> StartBackendIfNotStartedAsync();

    public Task<bool> ServerStartedAsync();

    public Task CloseNetlistOnServerAsync(UInt64 netlistId);
}