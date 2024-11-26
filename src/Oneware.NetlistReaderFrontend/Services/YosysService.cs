using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;

namespace Oneware.NetlistReaderFrontend.Services;

public class YosysService : IYosysService
{
    private IDockService _dockService;
    private ISettingsService _settingsService;
    private ICustomLogger _logger;
    private IChildProcessService _childProcessService;
    
    private string _yosysPath = string.Empty;

    public YosysService()
    {
        _dockService = ServiceManager.GetService<IDockService>();
        _settingsService = ServiceManager.GetService<ISettingsService>();
        _logger = ServiceManager.GetCustomLogger();
        _childProcessService = ServiceManager.GetService<IChildProcessService>();
        
        _settingsService.GetSettingObservable<string>("OssCadSuite_Path").Subscribe(x => _yosysPath = Path.Combine(x, "bin", "yosys.exe"));
    }
    
    public async Task<bool> LoadVhdlAsync(IProjectFile file)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> LoadVerilogAsync(IProjectFile file)
    {
        string workingDirectory = Path.Combine(file.Root!.FullPath, "build", "netlist");
        
        string top = Path.GetFileNameWithoutExtension(file.FullPath);

        List<string> files = new List<string>();

        if (File.Exists(Path.Combine(workingDirectory, "design.v")))
        {
            files.Add("\"" + Path.Combine(workingDirectory, "design.v") + "\"");
        }
        else
        {
            // TODO
            // get verilog files
        }

        List<string> yosysArgs =
            [ "-p", $"read_verilog {string.Join(' ', files)}; hierarchy -top {top}; proc; memory -nomap; flatten -scopename; write_json -compat-int netlist.json" ];
        
        bool success = false;
        string output = string.Empty;
        
        (success, output) = await _childProcessService.ExecuteShellAsync(_yosysPath, yosysArgs, workingDirectory, "", AppState.Loading, false, x =>
        {
            if (x.StartsWith("ghdl:error:"))
            {
                _logger.Error(x);
                return false;
            }

            _logger.Error(x);
            return true;
        }, x =>
        {
            if (x.StartsWith("ghdl:error:"))
            {
                _logger.Error(x);
                return false;
            }
                
            _logger.Error(x);
            return true;
        });
        
        return success;
    }

    public async Task<bool> CreateJsonNetlistAsync()
    {
        throw new NotImplementedException();
    }
}