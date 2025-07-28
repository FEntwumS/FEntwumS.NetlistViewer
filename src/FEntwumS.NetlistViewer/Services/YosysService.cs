using FEntwumS.NetlistViewer.Helpers;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.UniversalFpgaProjectSystem.Models;

namespace FEntwumS.NetlistViewer.Services;

public class YosysService : IYosysService
{
    private IDockService _dockService;
    private ISettingsService _settingsService;
    private ICustomLogger _logger;
    private IChildProcessService _childProcessService;
    private IToolExecuterService _toolExecuterService;
    private IFpgaBbService _fpgaBbService;
    private bool _useHierarchicalBackend;

    private string _yosysPath = string.Empty;

    public YosysService()
    {
        _dockService = ServiceManager.GetService<IDockService>();
        _settingsService = ServiceManager.GetService<ISettingsService>();
        _logger = ServiceManager.GetCustomLogger();
        _childProcessService = ServiceManager.GetService<IChildProcessService>();
        _toolExecuterService = ServiceManager.GetService<IToolExecuterService>();
        _fpgaBbService = ServiceManager.GetService<IFpgaBbService>();

        _settingsService.GetSettingObservable<string>(FentwumSNetlistViewerSettingsHelper.OssCadSuitePathKey)
            .Subscribe(x => _yosysPath = Path.Combine(x, "bin", "yosys"));
    }

    public void SubscribeToSettings()
    {
        _settingsService.GetSettingObservable<bool>(FentwumSNetlistViewerSettingsHelper.UseHierarchicalBackendKey)
            .Subscribe(x => _useHierarchicalBackend = x);
    }

    public Task<bool> LoadVhdlAsync(IProjectFile file)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> LoadVerilogAsync(IProjectFile file)
    {
        string workingDirectory = FentwumSNetlistViewerSettingsHelper.GetBuildDirectory(file);

        if (!Directory.Exists(workingDirectory))
        {
            Directory.CreateDirectory(workingDirectory);
        }

        string top = Path.GetFileNameWithoutExtension(file.FullPath);

        List<string> verilogFileList = new List<string>();

        List<string> systemVerilogFileList = new List<string>();

        string ccVerilogFilePath = FentwumSNetlistViewerSettingsHelper.GetNetlistFilePath(file);
        
        if (File.Exists(ccVerilogFilePath))
        {
            verilogFileList.Add(ccVerilogFilePath);
        }
        else
        {
            if (file.Root is not UniversalFpgaProjectRoot root) return false;
            IEnumerable<string> verilogFiles = root.Files
                .Where(x => !root.CompileExcluded.Contains(x)) // Exclude excluded files
                .Where(x => x.Extension is ".v") // Include only Verilog files
                .Where(x => !root.TestBenches.Contains(x)) // Exclude testbenches
                .Select(x => x.FullPath);

            IEnumerable<string> systemVerilogFiles = root.Files
                .Where(x => !root.CompileExcluded.Contains(x)) // Exclude excluded files
                .Where(x => x.Extension is ".sv") // Include only SystemVerilog files
                .Where(x => !root.TestBenches.Contains(x)) // Exclude testbenches
                .Select(x => x.FullPath);

            verilogFileList.AddRange(verilogFiles);
            systemVerilogFileList.AddRange(systemVerilogFiles);
        }

        List<string> yosysArgs =
        [
            "-p",
            $"read_verilog -sv -nooverwrite \"{string.Join("\" \"", verilogFileList)}\" {(systemVerilogFileList.Count > 0 ? "\"" + string.Join("\" \"", systemVerilogFileList) + "\"" : "")}; "
            + "scratchpad -set flatten.separator \";\"; "
            + $"{_fpgaBbService.getBbCommand(file)} hierarchy -check -purge_lib -top {top}; "
            + "proc; "
            + "memory -nomap; "
            + (_useHierarchicalBackend ?  "" : "flatten -scopename; ")
            + "select *; "
            + $"write_json -compat-int {top}.json"
        ];

        if (systemVerilogFileList.Count > 0)
        {
            yosysArgs.Insert(0, "-m");
            yosysArgs.Insert(1, "slang");
        }

        bool success = false;
        string stdout = string.Empty;
        string stderr = string.Empty;

        (success, stdout, stderr) =
            await _toolExecuterService.ExecuteToolAsync(_yosysPath, yosysArgs, workingDirectory);

        _logger.Log(stderr);

        return success;
    }

    public async Task<bool> LoadSystemVerilogAsync(IProjectFile file)
    {
        string workingDirectory = FentwumSNetlistViewerSettingsHelper.GetBuildDirectory(file);

        if (!Directory.Exists(workingDirectory))
        {
            Directory.CreateDirectory(workingDirectory);
        }

        string top = Path.GetFileNameWithoutExtension(file.FullPath);

        if (file.Root is not UniversalFpgaProjectRoot root) return false;
        IEnumerable<string> files = root.Files
            .Where(x => !root.CompileExcluded.Contains(x)) // Exclude excluded files
            .Where(x => x.Extension is ".sv") // Include only SystemVerilog files
            .Where(x => !root.TestBenches.Contains(x)) // Exclude testbenches
            .Select(x => x.FullPath);
        // TODO
        // get verilog files

        List<string> yosysArgs =
        [
            "-m", "slang", "-p",
            $"read_slang {string.Join(" ", files)}; scratchpad -set flatten.separator \";\"; hierarchy -top {top}; proc; memory -nomap; opt -full; flatten -scopename; write_json -compat-int netlist.json"
        ];

        bool success = false;
        string stdout = string.Empty;
        string stderr = string.Empty;

        (success, stdout, stderr) =
            await _toolExecuterService.ExecuteToolAsync(_yosysPath, yosysArgs, workingDirectory);

        return success;
    }

    public Task<bool> CreateJsonNetlistAsync()
    {
        throw new NotImplementedException();
    }
}