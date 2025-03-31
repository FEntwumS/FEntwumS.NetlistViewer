using System.Diagnostics;
using FEntwumS.Common.Services;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.UniversalFpgaProjectSystem.Models;
using Prism.Ioc;

namespace FEntwumS.WaveformInteractor.Services;

public class VerilatorService : IVerilatorService
{
    private readonly IProjectExplorerService _projectExplorerService;
    private readonly IYosysService _yosysService;
    private readonly IChildProcessService _childProcessService;
    private readonly ISettingsService _settingsService;

    private string _verilator = string.Empty;

    private ILogger _logger;
    
    public VerilatorService(IContainerProvider containerProvider)
    {
        _settingsService = containerProvider.Resolve<ISettingsService>();
        _childProcessService = containerProvider.Resolve<IChildProcessService>();
        _projectExplorerService = containerProvider.Resolve<IProjectExplorerService>();
        _yosysService = containerProvider.Resolve<IYosysService>();
        _settingsService.GetSettingObservable<string>("OssCadSuite_Path")
            .Subscribe(x => _verilator = Path.Combine(x, "bin", "verilator"));
        
        _logger = containerProvider.Resolve<ILogger>();
    }

    // Verilates Preprocessed file
    // expects set Testbench
    public async Task<bool> VerilateAsync(IProjectFile file)
    {
        if (Testbench == null)
        {
            _logger.Error($"Register .cpp Testbench to Verilate!");
            return false;
        }
        
        var workingDirectory = Path.Combine(file.Root!.FullPath, "build", "simulation");

        if (!Directory.Exists(workingDirectory)) Directory.CreateDirectory(workingDirectory);

        // string verilogFile = Path.Combine(workingDirectory, Path.GetFileNameWithoutExtension(file.FullPath) + "_preprocessed.v" );
        var verilogFile = Path.Combine(workingDirectory, file.Header);
        var testbenchFile = Testbench.FullPath;

        var top = Path.GetFileNameWithoutExtension(file.FullPath);

        List<string> verilatorArgs =
        [
            "--timescale-override", "10ns/10ns", // TODO: add option to configure simulation timescale 
            "-top-module", top, // takes top module from OneWare project
            "-Wall",
            "--trace",
            "--exe", // TODO: look into direct executable building. Probably it makes sense to add additional cc compile step
            "--build",
            "-cc", testbenchFile, verilogFile
        ];

        var success = false;
        var output = string.Empty;

        (success, output) = await ExecuteVerilatorCommandAsync(verilatorArgs, workingDirectory);
        _logger.Log(output, ConsoleColor.White, true);
        return success;
    }
    
    // compiles verilated project into binary. Uses generated Cmake file.
    // expects prior verilation with .cpp testbench
    public async Task<bool> CompileVerilatedAsync(IProjectFile topLevelFile)
    {
        var projectRootPath = topLevelFile.Root!.FullPath;
        var workingDirectory = Path.Combine(projectRootPath, "build", "simulation", "obj_dir");
        var top = Path.GetFileNameWithoutExtension(topLevelFile.FullPath);
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
                _logger.Log(e.Data, ConsoleColor.White, true);
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                _logger.Error($"Error: {e.Data}");
        };

        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();
        return true;
    }

    public async Task<bool> RunExecutableAsync(IProjectFile topLevelFile)
    {
        var top = Path.GetFileNameWithoutExtension(topLevelFile.FullPath);

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
                _logger.Log(e.Data, ConsoleColor.White, true);
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                _logger.Error($"Error: {e.Data}");
        };

        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();
        return true;
    }

    public void RegisterTestbench(IProjectFile? file)
    {
        if (file is not null && (file.Extension is ".cpp" or ".cxx" or ".cc" or ".C"))
        {
            var project = _projectExplorerService.ActiveProject?.Root as UniversalFpgaProjectRoot;
            // dont add to OneWare Testbenches, if already present
            if (!project.TestBenches.Any(tb => tb.Name.Equals(file.Name)))
            {
                project?.RegisterTestBench(file);
            }
            if (project != null) _projectExplorerService.SaveProjectAsync(project);
            Testbench = file;
        }
        else
        {
            _logger.Error($"Testbench has to be a C++ file!");
        }
    }

    public void UnregisterTestbench(IProjectFile file)
    {
        var project = _projectExplorerService.ActiveProject?.Root as UniversalFpgaProjectRoot;
        project?.UnregisterTestBench(file);
        if (project != null) _projectExplorerService.SaveProjectAsync(project);
        Testbench = null;
    }

    public IProjectFile? Testbench { get; set; }

    private async Task<(bool success, string output)> ExecuteVerilatorCommandAsync(List<string> args,
        string workingDirectory)
    {
        var success = false;
        var output = string.Empty;

        try
        {
            var result = await _childProcessService.ExecuteShellAsync(
                _verilator,
                args,
                workingDirectory,
                "Executing Verilator command");

            output = result.output;

            if (!string.IsNullOrEmpty(output))
                _logger.Log(output, ConsoleColor.White, true);

            success = !string.IsNullOrEmpty(output);
        }
        catch (Exception ex)
        {
            _logger.Error($"Error executing Verilator command: {ex.Message}", ex);
        }

        (bool success, string output) retVal = (success, output);
        return retVal;
    }
    
    // requires verilator testbench, and toplevel entity to be set.
    public async Task CreateVerilatorBinaryAllStepsAsync()
    {
        var projectRoot = _projectExplorerService.ActiveProject.Root as UniversalFpgaProjectRoot;
        var path = projectRoot.TopEntity.FullPath;
        var topFile = projectRoot.Files.FirstOrDefault(file => file.FullPath == path);

        if (topFile != null && Testbench != null)
        {
            await _yosysService.LoadVerilogAsync(topFile);
            await VerilateAsync(topFile);
            await CompileVerilatedAsync(topFile);
        }
        else
        {
            if (topFile == null && Testbench != null)
                _logger.Error("Toplevel Entity must be set!", null, true, true);
            if (topFile != null && Testbench == null)
                _logger.Error("Verilator Testbench must be set!", null, true, true);
            if (topFile == null && Testbench == null)
                _logger.Error("Toplevel Entity and Verilator Testbench must be set!", null, true, true);
        }
    }
}