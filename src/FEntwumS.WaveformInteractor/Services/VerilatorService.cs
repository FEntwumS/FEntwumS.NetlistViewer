using System.Diagnostics;
using FEntwumS.Common.Services;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.UniversalFpgaProjectSystem.Models;
using Prism.Ioc;


namespace FEntwumS.WaveformInteractor.Services;

public class VerilatorService : IVerilatorService
{
    private ISettingsService _settingsService;
    private IChildProcessService _childProcessService;

    private string _verilator = string.Empty;
    
    private IProjectFile? _testbench;
    private readonly IYosysService _yosysService;
    private readonly IProjectExplorerService _projectExplorerService;

    public VerilatorService(IContainerProvider containerProvider)
    {
        _settingsService = containerProvider.Resolve<ISettingsService>();
        _childProcessService = containerProvider.Resolve<IChildProcessService>();
        _projectExplorerService = containerProvider.Resolve<IProjectExplorerService>();
        _yosysService = containerProvider.Resolve<IYosysService>();
        _settingsService.GetSettingObservable<string>("OssCadSuite_Path")
            .Subscribe(x => _verilator = Path.Combine(x, "bin", "verilator"));
    }
    
    // Verilates Preprocessed file
    public async Task<bool> VerilateAsync(IProjectFile file)
    {
        string workingDirectory = Path.Combine(file.Root!.FullPath, "build", "simulation");

        if (!Directory.Exists(workingDirectory))
        {
            Directory.CreateDirectory(workingDirectory);
        }
        
        string verilogFile = Path.Combine(workingDirectory, Path.GetFileNameWithoutExtension(file.FullPath) + "_preprocessed.v" );
        string testbenchFile = _testbench.FullPath;
        
        string top = Path.GetFileNameWithoutExtension(file.FullPath);
        
        
        List<string> yosysArgs =
        [
            "--timescale-override", "10ns/10ns", // TODO: add option to configure simulation timescale 
            "-top-module", top,
            "-Wall",
            "--trace",
            "--exe", // TODO: look into direct executable building. Probably it makes sense to add additional cc compile step
            "--build",
            "-cc", testbenchFile, verilogFile,
        ];

        bool success = false;
        string output = string.Empty;
        
        // TODO execute yosys cmd
        (success, output) = await ExecuteVerilatorCommandAsync(yosysArgs, workingDirectory);
        Console.WriteLine($"Output: {output}");

        return success;
    }
    
    public async Task<bool> CompileVerilatedAsync(IProjectFile topLevelFile)
    {
        var projectRootPath = topLevelFile.Root!.FullPath;
        var workingDirectory = Path.Combine(projectRootPath, "build", "simulation", "obj_dir");
        string top = Path.GetFileNameWithoutExtension(topLevelFile.FullPath);
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
    
    public async Task<bool> RunExecutableAsync(IProjectFile topLevelFile)
    {
        string top = Path.GetFileNameWithoutExtension(topLevelFile.FullPath);

        var projectRootPath = topLevelFile.Root!.FullPath;
        var workingDirectory = Path.Combine(projectRootPath);
        var executablePath = Path.Combine(workingDirectory, "build", "simulation", "obj_dir", $"V{top}");
        var processStartInfo = new ProcessStartInfo
        {
            FileName = executablePath,
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
    public void RegisterTestbench(IProjectFile? file)
    {
        if (file is not null)
        {
            UniversalFpgaProjectRoot? project = _projectExplorerService.ActiveProject?.Root as UniversalFpgaProjectRoot;
            project?.RegisterTestBench(file);
            if (project != null) _projectExplorerService.SaveProjectAsync(project);
        }
        _testbench = file;
    }
        
    public void UnregisterTestbench(IProjectFile file)
    {
        UniversalFpgaProjectRoot? project = _projectExplorerService.ActiveProject?.Root as UniversalFpgaProjectRoot;
        project?.UnregisterTestBench(file);
        if (project != null) _projectExplorerService.SaveProjectAsync(project);
        _testbench = null;
    }

    public IProjectFile? Testbench
    {
        get => _testbench;
        set => _testbench = value;
    }

}