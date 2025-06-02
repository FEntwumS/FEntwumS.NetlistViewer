using FEntwumS.NetlistViewer.ViewModels;
using OneWare.Essentials.Models;

namespace FEntwumS.NetlistViewer.Services;

public interface IFrontendService : SettingsSubscriber
{
    public void SubscribeToSettings();

    public Task CreateVhdlNetlistAsync(IProjectFile vhdl);

    public Task CreateVerilogNetlistAsync(IProjectFile verilog);

    public Task CreateSystemVerilogNetlistAsync(IProjectFile sVerilog);

    public Task ShowViewerAsync(IProjectFile json);

    public Task ExpandNodeAsync(string? nodePath, FrontendViewModel vm);

    public Task<bool> StartBackendIfNotStartedAsync();

    public Task<bool> ServerStartedAsync();

    public Task CloseNetlistOnServerAsync(UInt64 netlistId);
}