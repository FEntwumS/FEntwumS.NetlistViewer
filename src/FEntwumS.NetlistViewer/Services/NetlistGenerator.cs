using FEntwumS.NetlistViewer.Types;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.ProjectSystem.Models;

namespace FEntwumS.NetlistViewer.Services;

public class NetlistGenerator : INetlistGenerator
{
    private static readonly ICustomLogger _logger;
    private static readonly IApplicationStateService _applicationStateService;
    private static readonly ISettingsService _settingsService;
    
    private bool _alwaysRegenerateNetlists = true;
    
    static NetlistGenerator()
    {
        _logger = ServiceManager.GetCustomLogger();
        _applicationStateService = ServiceManager.GetService<IApplicationStateService>();
        _settingsService = ServiceManager.GetService<ISettingsService>();
    }
    
    public async Task<bool> GenerateVhdlNetlistAsync(IProjectFile vhdlProject)
    {
        IGhdlService ghdlService = ServiceManager.GetService<IGhdlService>();
        bool success;

        success = await ghdlService.ElaborateDesignAsync(vhdlProject);

        if (!success)
        {
            return false;
        }
        
        success = await ghdlService.CrossCompileDesignAsync(vhdlProject);
        
        return success;
    }

    public async Task<bool> GenerateVerilogNetlistAsync(IProjectFile verilogProject)
    {
        IYosysService yosysService = ServiceManager.GetService<IYosysService>();
        bool success;
        
        success = await yosysService.LoadVerilogAsync(verilogProject);
        
        return success;
    }

    public async Task<bool> GenerateSystemVerilogNetlistAsync(IProjectFile systemVerilogProject)
    {
        // TODO update with implementation using yosys_slang plugin
        return await GenerateVerilogNetlistAsync(systemVerilogProject);
    }

    public async Task<(IProjectFile? netlistFile, bool success)> GenerateNetlistAsync(IProjectFile projectFile, NetlistType netlistType)
    {
        bool success;
        IProjectFile? netlistFile;

        if (!_alwaysRegenerateNetlists)
        {
            (netlistFile, success) = GetExistingNetlist(projectFile);

            if (success)
            {
                return (netlistFile, true);
            }
        }

        switch (netlistType)
        {
            case NetlistType.VHDL:
                success =  await GenerateVhdlNetlistAsync(projectFile) && await GenerateVerilogNetlistAsync(projectFile);
                break;
            
            case NetlistType.Verilog:
                success = await GenerateVerilogNetlistAsync(projectFile);
                break;
            
            case NetlistType.System_Verilog:
                success = await GenerateSystemVerilogNetlistAsync(projectFile);
                break;
            
            default:
                success = false;
                break;
        }

        if (!success)
        {
            return (null, false);
        }
        
        string top = Path.GetFileNameWithoutExtension(projectFile.FullPath);
        string netlistPath = Path.Combine(projectFile.Root!.FullPath, "build", "netlist", $"{top}.json");
        
        if (!File.Exists(netlistPath))
        {
            _logger.Error($"Netlist file not found: {netlistPath}");

            return (null, false);
        }

        netlistFile = new ProjectFile(netlistPath, projectFile.TopFolder!);
        
        return (netlistFile, true);
    }

    public (IProjectFile? netlistFile, bool success) GetExistingNetlist(IProjectFile projectFile)
    {
        string top = Path.GetFileNameWithoutExtension(projectFile.FullPath);
        string netlistPath = Path.Combine(projectFile.Root!.FullPath, "build", "netlist", $"{top}.json");
        
        if (!File.Exists(netlistPath))
        {
            return (null, false);
        }
        
        FileInfo netlistFile = new FileInfo(netlistPath);
        if (netlistFile.CreationTime.CompareTo(projectFile.LastSaveTime) > 0)
        {
            // netlist is newer
            // therefore we dont need to re-generate the netlist, saving the user lots of time
            return (new ProjectFile(netlistPath, projectFile.TopFolder!), true);
        }

        return (null, false);
    }

    public void SubscribeToSettings()
    {
        _settingsService.GetSettingObservable<bool>("NetlistViewer_AlwaysRegenerateNetlists")
            .Subscribe((x) => _alwaysRegenerateNetlists = x);
    }
}