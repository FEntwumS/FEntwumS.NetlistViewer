using Avalonia.Logging;
using FEntwumS.Common.Services;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.ProjectSystem.Models;
using OneWare.UniversalFpgaProjectSystem.Models;
using Prism.Ioc;

namespace FEntwumS.WaveformInteractor.Services;

public class YosysSimService : IYosysService
{
    private ISettingsService _settingsService;
    private IChildProcessService _childProcessService;

    private string _yosysPath = string.Empty;

    public YosysSimService(IContainerProvider containerProvider)
    {
        _settingsService = containerProvider.Resolve<ISettingsService>();
        _childProcessService = containerProvider.Resolve<IChildProcessService>();

        _settingsService.GetSettingObservable<string>("OssCadSuite_Path")
            .Subscribe(x => _yosysPath = Path.Combine(x, "bin", "yosys"));
    }

    public Task<bool> LoadVhdlAsync(IProjectFile file)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> LoadVerilogAsync(IProjectFile file)
    {
        string workingDirectory = Path.Combine(file.Root!.FullPath, "build", "simulation");

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
            $"read_verilog -nooverwrite \"{string.Join("\" \"", files)}\"; scratchpad -set flatten.separator \";\"; hierarchy -check -top {top}; proc; memory -nomap; flatten -scopename; write_verilog {top}_preprocessed.v"
        ];

        bool success = false;
        string output = string.Empty;
        
        // TODO execute yosys cmd
        (success, output) = await ExecuteYosysCommandAsync(yosysArgs, workingDirectory);
        Console.WriteLine($"Output: {output}");

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

    async Task<(bool success, string output)> ExecuteYosysCommandAsync(List<string> yosysArgs, string workingDirectory)
    {
        bool success = false;
        string output = string.Empty;

        try
        {
            var result = await _childProcessService.ExecuteShellAsync(
                _yosysPath, 
                yosysArgs, 
                workingDirectory, 
                "Executing Yosys command", 
                AppState.Loading);

            output = result.output;

            if (!string.IsNullOrEmpty(output))
                Console.WriteLine(output);

            success = !string.IsNullOrEmpty(output);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing Yosys command: {ex.Message}");
        }

        (bool success, string output) retVal = (success, output);
        return retVal;
    }

}