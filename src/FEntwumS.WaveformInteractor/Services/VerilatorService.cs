using System.Diagnostics;
using FEntwumS.Common.Services;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using Prism.Ioc;


namespace FEntwumS.WaveformInteractor.Services;

public class VerilatorService : IVerilatorService
{
    private ISettingsService _settingsService;
    private IChildProcessService _childProcessService;

    private string _verilator = string.Empty;
    
    private string _testbench = string.Empty;

    public VerilatorService(IContainerProvider containerProvider)
    {
        _settingsService = containerProvider.Resolve<ISettingsService>();
        _childProcessService = containerProvider.Resolve<IChildProcessService>();

        _settingsService.GetSettingObservable<string>("OssCadSuite_Path")
            .Subscribe(x => _verilator = Path.Combine(x, "bin", "verilator"));
    }
    
    public async Task<bool> VerilateAsync(IProjectFile file)
    {
        string workingDirectory = Path.Combine(file.Root!.FullPath, "build", "simulation");

        if (!Directory.Exists(workingDirectory))
        {
            Directory.CreateDirectory(workingDirectory);
        }

        string verilog_file = Path.Combine(workingDirectory, Path.GetFileNameWithoutExtension(file.FullPath) + "_preprocessed.v" );
        string top = Path.GetFileNameWithoutExtension(file.FullPath);
        
        List<string> yosysArgs =
        [
            "--timescale-override", "10ns/10ns", // TODO: add option to configure simulation timescale 
            "-top-module", top,
            "-Wall",
            "--trace",
            "--exe", // TODO: look into direct executable building. Probably it makes sense to add additional cc compile step
            "--build",
            "-cc", _testbench, verilog_file,
        ];

        bool success = false;
        string output = string.Empty;
        
        // TODO execute yosys cmd
        (success, output) = await ExecuteVerilatorCommandAsync(yosysArgs, workingDirectory);
        Console.WriteLine($"Output: {output}");

        return success;
    }

    
    // TODO: Test and check compilation
    public async Task<bool> CompileVerilatedAsync(IProjectFile file)
    {
        // Use Path.Combine for OS-independent directory paths
        var workingDirectory = Path.Combine("build", "simulation", "obj_dir");
        string top = Path.GetFileNameWithoutExtension(file.FullPath);
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "make",
            Arguments = $"-f V{top}.mk",
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = new Process { StartInfo = processStartInfo };

        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine(e.Data);
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"Error: {e.Data}");
        };

        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();
        return true;
    }
    
    async Task<(bool success, string output)> ExecuteVerilatorCommandAsync(List<string> args, string workingDirectory)
    {
        bool success = false;
        string output = string.Empty;

        try
        {
            var result = await _childProcessService.ExecuteShellAsync(
                _verilator, 
                args, 
                workingDirectory, 
                "Executing Verilator command", 
                AppState.Loading);

            output = result.output;

            if (!string.IsNullOrEmpty(output))
                Console.WriteLine(output);

            success = !string.IsNullOrEmpty(output);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing Verilator command: {ex.Message}");
        }

        (bool success, string output) retVal = (success, output);
        return retVal;
    }

    public void SetTestbench(string file)
    {
        _testbench = file;
    }

    public string GetTestbench()
    {
        return _testbench;
    }
}