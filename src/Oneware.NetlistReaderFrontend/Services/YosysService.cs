using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.ProjectSystem.Models;
using OneWare.UniversalFpgaProjectSystem.Models;

namespace Oneware.NetlistReaderFrontend.Services;

public class YosysService : IYosysService
{
    private IDockService _dockService;
    private ISettingsService _settingsService;
    private ICustomLogger _logger;
    private IChildProcessService _childProcessService;
    private IToolExecuterService _toolExecuterService;
    private IFpgaBbService _fpgaBbService;

    private string _yosysPath = string.Empty;

    public YosysService()
    {
        _dockService = ServiceManager.GetService<IDockService>();
        _settingsService = ServiceManager.GetService<ISettingsService>();
        _logger = ServiceManager.GetCustomLogger();
        _childProcessService = ServiceManager.GetService<IChildProcessService>();
        _toolExecuterService = ServiceManager.GetService<IToolExecuterService>();
        _fpgaBbService = ServiceManager.GetService<IFpgaBbService>();

        _settingsService.GetSettingObservable<string>("OssCadSuite_Path")
            .Subscribe(x => _yosysPath = Path.Combine(x, "bin", "yosys"));
    }

    public async Task<bool> LoadVhdlAsync(IProjectFile file)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> LoadVerilogAsync(IProjectFile file)
    {
        string workingDirectory = Path.Combine(file.Root!.FullPath, "build", "netlist");

        if (!Directory.Exists(workingDirectory))
        {
            Directory.CreateDirectory(workingDirectory);
        }

        string top = Path.GetFileNameWithoutExtension(file.FullPath);

        List<string> files = new List<string>();

        if (File.Exists(Path.Combine(workingDirectory, "design.v")))
        {
            files.Add(Path.Combine(workingDirectory, "design.v"));
        }
        else
        {
            UniversalFpgaProjectRoot root = file.Root as UniversalFpgaProjectRoot;
            IEnumerable<string> verilogFiles = root.Files
                .Where(x => !root.CompileExcluded.Contains(x)) // Exclude excluded files
                .Where(x => x.Extension is ".v") // Include only Verilog and SystemVerilog files
                .Where(x => !root.TestBenches.Contains(x)) // Exclude testbenches
                .Select(x => x.FullPath);
            // TODO
            // get verilog files

            files.AddRange(verilogFiles);
        }

        List<string> yosysArgs =
        [
            "-p",
            $"read_verilog \"{string.Join("\" \"", files)}\"; scratchpad -set flatten.separator \"/\"; {_fpgaBbService.getBbCommand()} hierarchy -check -top {top}; proc; memory -nomap; flatten -scopename; write_json -compat-int {top}.json"
        ];

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
        string workingDirectory = Path.Combine(file.Root!.FullPath, "build", "netlist");

        if (!Directory.Exists(workingDirectory))
        {
            Directory.CreateDirectory(workingDirectory);
        }

        string top = Path.GetFileNameWithoutExtension(file.FullPath);


        UniversalFpgaProjectRoot root = file.Root as UniversalFpgaProjectRoot;
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
            $"read_slang \"{string.Join("\" \"", files)}\"; scratchpad -set flatten.separator \"/\"; hierarchy -top {top}; proc; memory -nomap; opt -full; flatten -scopename; write_json -compat-int netlist.json"
        ];

        bool success = false;
        string stdout = string.Empty;
        string stderr = string.Empty;

        (success, stdout, stderr) =
            await _toolExecuterService.ExecuteToolAsync(_yosysPath, yosysArgs, workingDirectory);

        _logger.Log(stdout);

        return success;
    }

    public async Task<bool> CreateJsonNetlistAsync()
    {
        throw new NotImplementedException();
    }
}