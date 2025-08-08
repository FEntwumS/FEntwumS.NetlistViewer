using FEntwumS.NetlistViewer.Helpers;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.UniversalFpgaProjectSystem.Models;

namespace FEntwumS.NetlistViewer.Services;

public class YosysService : IYosysService
{
    private ISettingsService _settingsService;
    private ICustomLogger _logger;
    private IToolExecuterService _toolExecuterService;
    private IFpgaBbService _fpgaBbService;
    private bool _useHierarchicalBackend;

    private string _yosysPath = string.Empty;

    public YosysService()
    {
        _settingsService = ServiceManager.GetService<ISettingsService>();
        _logger = ServiceManager.GetCustomLogger();
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
        // This method has not been implemented due to the Windows version of the oss cad suite not including the
        // ghdl-yosys plugin
        
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

        string ccVerilogFilePath = FentwumSNetlistViewerSettingsHelper.GetCcVhdlFilePath(file);
        
        // If cross-compiled VHDL exists, only the cross-compiled code will be included in the yosys command.
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
            + "scratchpad -set flatten.separator \";\"; "   // Use the semicolon as hierarchy separator; See https://yosyshq.readthedocs.io/projects/yosys/en/v0.55/cmd/flatten.html
            + $"{_fpgaBbService.getBbCommand(file)} hierarchy -check -purge_lib -top {top}; "
            + "proc; "  // proc needs to run because JSON netlists produced by yosys may not contain RTLIL processes
            + "memory -nomap; " // Converts memories into simple blocks instead of basic cells
            + "select *; "  // Remove unnecessary library elements from the netlist
            + $"write_json -compat-int {top}-hier.json; " // Write hierarchical JSON netlist to disk
            + "scratchpad -set flatten.separator \";\"; "
            + "debug flatten -scopename; "                // Flatten the netlist
            + "select *; "
            + "clean; "
            + $"write_json -compat-int {top}-flat.json" // Write flattened JSON netlist to disk
        ];
        
        // Load the yosys_slang plugin if the design contains SystemVerilog files
        // The plugin is currently not used by the generated yosys command
        if (systemVerilogFileList.Count > 0)
        {
            yosysArgs.Insert(0, "-m");
            yosysArgs.Insert(1, "slang");
        }

        (bool success, _, _) =
            await _toolExecuterService.ExecuteToolAsync(_yosysPath, yosysArgs, workingDirectory);

        return success;
    }

    public async Task<bool> LoadSystemVerilogAsync(IProjectFile file)
    {
        // This method works essentially like LoadVerilogAsync(), but it uses the yosys_slang plugin as frontend for
        // loading the HDL sources. This did not work when last tested (March 2025). It is therefore recommended to use
        // LoadVerilogAsync() even for designs containing SystemVerilog source code, since yosys' limited SV support is
        // enabled
        
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
}