using FEntwumS.Common.Services;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.UniversalFpgaProjectSystem.Models;
using Prism.Ioc;

namespace FEntwumS.WaveformInteractor.Services;

public class YosysSimService : IYosysService
{
    private readonly IChildProcessService _childProcessService;
    private readonly ISettingsService _settingsService;

    private string _yosysPath = string.Empty;
    private readonly ILogger _logger;

    public YosysSimService(IContainerProvider containerProvider)
    {
        _settingsService = containerProvider.Resolve<ISettingsService>();
        _childProcessService = containerProvider.Resolve<IChildProcessService>();

        _logger = containerProvider.Resolve<ILogger>();
        _settingsService.GetSettingObservable<string>("OssCadSuite_Path")
            .Subscribe(x => _yosysPath = Path.Combine(x, "bin", "yosys"));
    }

    public Task<bool> LoadVhdlAsync(IProjectFile file)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> LoadVerilogAsync(IProjectFile file)
    {
        var workingDirectory = Path.Combine(file.Root!.FullPath, "build", "simulation");

        if (!Directory.Exists(workingDirectory)) Directory.CreateDirectory(workingDirectory);

        var top = Path.GetFileNameWithoutExtension(file.FullPath);

        List<string> files = new();

        if (File.Exists(Path.Combine(workingDirectory, "design.v")))
        {
            files.Add(Path.Combine(workingDirectory, "design.v"));
        }
        else
        {
            var root = file.Root as UniversalFpgaProjectRoot;
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
            $"read_verilog -nooverwrite \"{string.Join("\" \"", files)}\"; scratchpad -set flatten.separator \";\"; hierarchy -check -top {top}; proc; memory -nomap; flatten -scopename; write_verilog {file.Header}"
        ];

        var success = false;
        var output = string.Empty;

        (success, output) = await ExecuteYosysCommandAsync(yosysArgs, workingDirectory);
        _logger.Log($"{output}", ConsoleColor.White, true);

        return success;
    }

    public Task<bool> LoadSystemVerilogAsync(IProjectFile file)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CreateJsonNetlistAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> CreateVerilogAsync()
    {
        throw new NotImplementedException();
    }

    private async Task<(bool success, string output)> ExecuteYosysCommandAsync(List<string> yosysArgs,
        string workingDirectory)
    {
        var success = false;
        var output = string.Empty;

        try
        {
            var result = await _childProcessService.ExecuteShellAsync(
                _yosysPath,
                yosysArgs,
                workingDirectory,
                "Executing Yosys command");

            output = result.output;

            if (!string.IsNullOrEmpty(output))
                _logger.Log(output, ConsoleColor.White,true);

            success = !string.IsNullOrEmpty(output);
        }
        catch (Exception ex)
        {
            _logger.Error($"Error executing Yosys command: {ex.Message}");
        }

        (bool success, string output) retVal = (success, output);
        return retVal;
    }
}