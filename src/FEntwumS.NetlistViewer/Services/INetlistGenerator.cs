using FEntwumS.NetlistViewer.Types;
using OneWare.Essentials.Models;

namespace FEntwumS.NetlistViewer.Services;

public interface INetlistGenerator: SettingsSubscriber
{
    public Task<bool> GenerateVhdlNetlistAsync(IProjectFile vhdlProject);
    public Task<bool> GenerateVerilogNetlistAsync(IProjectFile verilogProject);
    public Task<bool> GenerateSystemVerilogNetlistAsync(IProjectFile systemVerilogProject);
    public Task<(IProjectFile? netlistFile, bool success)> GenerateNetlistAsync(IProjectFile projectFile, NetlistType netlistType);
    public (IProjectFile? netlistFile, bool success) GetExisitingNetlist(IProjectFile projectFile);
}