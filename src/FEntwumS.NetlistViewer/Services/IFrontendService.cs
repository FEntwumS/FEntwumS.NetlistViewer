using FEntwumS.NetlistViewer.ViewModels;
using OneWare.Essentials.Models;

namespace FEntwumS.NetlistViewer.Services;

public interface IFrontendService
{
    public void SubscribeToSettings();

    public Task CreateVhdlNetlist(IProjectFile vhdl);

    public Task CreateVerilogNetlist(IProjectFile verilog);

    public Task CreateSystemVerilogNetlist(IProjectFile sVerilog);

    public Task ShowViewer(IProjectFile json);

    public Task ExpandNode(string nodePath, FrontendViewModel vm);

    public Task<bool> StartBackendIfNotStartedAsync();

    public Task<bool> ServerStartedAsync();

    public Task CloseNetlistOnServerAsync(UInt64 netlistId);
}