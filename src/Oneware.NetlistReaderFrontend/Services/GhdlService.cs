using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.UniversalFpgaProjectSystem.Models;

namespace Oneware.NetlistReaderFrontend.Services;

public class GhdlService : IGhdlService
{
    private IDockService _dockService;
    private ICustomLogger _logger;
    private ISettingsService _settingsService;
    private IChildProcessService _childProcessService;
    
    private string _ghdlPath = string.Empty;
    private string _vhdlStandard = string.Empty;
    
    public GhdlService()
    {
        _dockService = ServiceManager.GetService<IDockService>();
        _logger = ServiceManager.GetCustomLogger();
        _settingsService = ServiceManager.GetService<ISettingsService>();
        _childProcessService = ServiceManager.GetService<IChildProcessService>();

        // This key is copied from https://github.com/one-ware/OneWare.GhdlExtension/blob/21d98f5d948370d59b79fbe5be99d63fd044a633/src/OneWare.GhdlExtension/GhdlExtensionModule.cs#L140C44-L140C63
        // and might need updating in the future
        
        // TODO
        // check if actual interop between plugins is possible
        _settingsService.GetSettingObservable<string>("GhdlModule_GhdlPath").Subscribe(x => _ghdlPath = x);
        _settingsService.GetSettingObservable<string>("NetlistViewer_VHDL_Standard").Subscribe(x => _vhdlStandard = x);
    }
    
    public async Task<bool> AnalyseDesignAsync(IProjectFile file)
    {
        _dockService.Show<IOutputService>();

        if (file.Root is not UniversalFpgaProjectRoot root)
        {
            _logger.Error("Selected file is not the toplevel");
            return false;
        }

        if (_ghdlPath == string.Empty)
        {
            _logger.Error("No GHDL path specified. Please install the GHDL-Plugin and specify the path to the executable, if necessary.");
        }

        if (!File.Exists(_ghdlPath))
        {
            _logger.Error("GHDL path does not point to ghdl executable");
        }
        
        IEnumerable<string> vhdlFiles = root.Files
            .Where(x => !root.CompileExcluded.Contains(x))  // Exclude excluded files
            .Where(x => x.Extension is ".vhd" or ".vhdl")   // Include only VHDL files
            .Where(x=> !root.TestBenches.Contains(x))       // Exclude testbenches
            .Select(x => x.FullPath);
        
        string workingDirectory = Path.Combine(file.Root!.FullPath, "build", "netlist");

        if (Directory.Exists(workingDirectory))
        {
            Directory.Delete(workingDirectory, true);
        }
        
        Directory.CreateDirectory(workingDirectory);
            
        List<string> ghdlOptions = [];
        
        ghdlOptions.Add("--std=" + _vhdlStandard);

        List<string> ghdlAnalysisArgs = ["-a"];
        ghdlAnalysisArgs.AddRange(ghdlOptions);
        ghdlAnalysisArgs.AddRange(vhdlFiles);

        bool success = false;
        string output = string.Empty;
        
        (success, output) = await _childProcessService.ExecuteShellAsync(_ghdlPath, ghdlAnalysisArgs, workingDirectory, "", AppState.Loading, false, x =>
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
        
        _logger.Log(output, false);
        
        return success;
    }

    public async Task<bool> CrossCompileDesignAsync(IProjectFile file)
    {
        string workingDirectory = Path.Combine(file.Root!.FullPath, "build", "netlist");
        
        string top = Path.GetFileNameWithoutExtension(file.FullPath);

        List<string> ghdlOptions = [ "--out=verilog" ];
        // ghdlOptions.Add("-o=design.v");  // TODO rework
        // When a new version of GHDL is available, this parameter will allow us to write the result directly to a file,
        // instead of needing to use File.WriteAllTextAsync

        List<string> ghdlSynthArgs = ["--synth"];
        ghdlSynthArgs.AddRange(ghdlOptions);
        ghdlSynthArgs.Add(top);
        
        bool success = false;
        string output = string.Empty;
        
        (success, output) = await _childProcessService.ExecuteShellAsync(_ghdlPath, ghdlSynthArgs, workingDirectory, "", AppState.Loading, false, x =>
        {
            if (x.StartsWith("ghdl:error:"))
            {
                _logger.Error(x);
                return false;
            }

            _logger.Log(x);
            return true;
        }, x =>
        {
            if (x.StartsWith("ghdl:error:"))
            {
                _logger.Error(x);
                return false;
            }
                
            _logger.Log(x);
            return true;
        });
        
        _logger.Log(output);
        
        await File.WriteAllTextAsync(Path.Combine(workingDirectory, "design.v"), output);
        
        return success;
    }

    private async Task<bool> CheckIfGhdlIsInstalledAsync()
    {
        return true;
    }
}