using Avalonia.Threading;
using FEntwumS.NetlistViewer.Helpers;
using FEntwumS.NetlistViewer.Types;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.ProjectSystem.Models;
using OneWare.UniversalFpgaProjectSystem.Models;

namespace FEntwumS.NetlistViewer.Services;

public class NetlistGenerator : INetlistGenerator
{
    private readonly ICustomLogger _logger;
    private readonly ISettingsService _settingsService;
    private readonly IProjectExplorerService _projectExplorerService;

    private bool _alwaysRegenerateNetlists = true;

    private DispatcherTimer _timer = new();
    private bool _timerNeeded = false;
    private double _generationInterval = 60.0d;

    private List<FileSystemWatcher> _watchers = new();
    private readonly Lock _lock = new();
    private HashSet<UniversalProjectRoot> _changedProjectSet = new();

    public NetlistGenerator()
    {
        _logger = ServiceManager.GetCustomLogger();
        _settingsService = ServiceManager.GetService<ISettingsService>();
        _projectExplorerService = ServiceManager.GetService<IProjectExplorerService>();
    }

    public async Task<bool> GenerateVhdlNetlistAsync(IProjectFile vhdlProject)
    {
        OneWare.GhdlExtension.Services.GhdlService ghdlService =
            ServiceManager.GetService<OneWare.GhdlExtension.Services.GhdlService>();

        string outputDir = Path.Combine(vhdlProject.Root!.FullPath, "build", "netlist");

        return await ghdlService.SynthAsync(vhdlProject, "verilog", outputDir);
    }

    public async Task<bool> GenerateVerilogNetlistAsync(IProjectFile verilogProject)
    {
        IYosysService yosysService = ServiceManager.GetService<IYosysService>();
        bool success;

        success = await yosysService.LoadVerilogAsync(verilogProject);

        return success;
    }

    public async Task<bool> GenerateSystemVerilogNetlistAsync(IProjectFile systemVerilogProject)
    {
        // TODO update with implementation using yosys_slang plugin
        return await GenerateVerilogNetlistAsync(systemVerilogProject);
    }

    public async Task<(IProjectFile? netlistFile, bool success)> GenerateNetlistAsync(IProjectFile projectFile,
        NetlistType netlistType)
    {
        bool success;
        IProjectFile? netlistFile;

        if (!_alwaysRegenerateNetlists)
        {
            (netlistFile, success) = GetExistingNetlist(projectFile);

            if (success)
            {
                return (netlistFile, true);
            }
        }

        switch (netlistType)
        {
            case NetlistType.VHDL:
                success = await GenerateVhdlNetlistAsync(projectFile) && await GenerateVerilogNetlistAsync(projectFile);
                break;

            case NetlistType.Verilog:
                success = await GenerateVerilogNetlistAsync(projectFile);
                break;

            case NetlistType.System_Verilog:
                success = await GenerateSystemVerilogNetlistAsync(projectFile);
                break;

            default:
                success = false;
                break;
        }

        if (!success)
        {
            return (null, false);
        }

        string top = Path.GetFileNameWithoutExtension(projectFile.FullPath);
        string netlistPath = Path.Combine(projectFile.Root!.FullPath, "build", "netlist", $"{top}.json");

        if (!File.Exists(netlistPath))
        {
            _logger.Error($"Netlist file not found: {netlistPath}");

            return (null, false);
        }

        netlistFile = new ProjectFile(netlistPath, projectFile.TopFolder!);

        return (netlistFile, true);
    }

    public (IProjectFile? netlistFile, bool success) GetExistingNetlist(IProjectFile projectFile)
    {
        if (projectFile.Root is not UniversalFpgaProjectRoot root)
        {
            return (null, false);
        }

        string top = Path.GetFileNameWithoutExtension(projectFile.FullPath);
        string netlistPath = Path.Combine(root.FullPath, "build", "netlist", $"{top}.json");

        FileInfo netlistFile = new FileInfo(netlistPath);
        bool newNetlistNecessary = false;

        foreach (string file in root.Files
                     .Where(x => !root.CompileExcluded.Contains(x))
                     .Where(x => x.Extension is ".v" or ".sv" or ".vhdl" or ".vhd")
                     .Where(x => !root.TestBenches.Contains(x))
                     .Select(x => x.FullPath))
        {
            FileInfo srcFileInfo = new FileInfo(file);

            if (netlistFile.LastWriteTimeUtc.CompareTo(srcFileInfo.LastWriteTimeUtc) < 0)
            {
                newNetlistNecessary = true;
                break;
            }
        }

        if (newNetlistNecessary)
        {
            return (null, false);
        }

        return (new ProjectFile(netlistPath, projectFile.TopFolder!), true);
    }

    private void timer_Tick(object? sender, EventArgs e)
    {
    }

    private async Task RegenerateNetlistAsync(IProjectFile projectFile)
    {
    }

    private void setupWatchers()
    {
        IEnumerable<IProjectRoot> missingWatchers =
            _projectExplorerService.Projects.Where(project => _watchers.All(watcher => watcher.Path != project.FullPath));
        IEnumerable<FileSystemWatcher> unnecessaryWatchers = _watchers.Where(watcher =>
            _projectExplorerService.Projects.All(project => watcher.Path != project.FullPath));

        List<FileSystemWatcher> unnecessaryFileSystemWatchers = unnecessaryWatchers.ToList();
        foreach (var w in unnecessaryFileSystemWatchers)
        {
            w.Dispose();
        }

        _watchers.RemoveAll(x => unnecessaryFileSystemWatchers.Any(w => w == x));
        
        List<IProjectRoot> projectsMissingWatchers = missingWatchers.ToList();
        foreach (var p in projectsMissingWatchers)
        {
            if (p is UniversalProjectRoot root)
            {
                FileSystemWatcher watcher = new FileSystemWatcher(root.FullPath)
                {
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
                };

                watcher.Changed += ProjectChanged;
                watcher.Created += ProjectChanged;
                watcher.Deleted += ProjectChanged;
                watcher.Renamed += ProjectChanged;

                watcher.Error += (sender, args) =>
                {
                    _logger.Error("Error in watcher");
                    if (sender is FileSystemWatcher watcher)
                    {
                        _logger.Error($"Watcher: {watcher.Path}");
                    }
                };
            }
        }
    }

    private void ProjectChanged(object sender, FileSystemEventArgs e)
    {
        if (sender is FileSystemWatcher watcher)
        {
            _logger.Log($"Sender: {watcher.Path}");
        }
    }

    public void SubscribeToSettings()
    {
        _settingsService.GetSettingObservable<bool>(FentwumSNetlistViewerSettingsHelper.AlwaysRegenerateNetlistsKey)
            .Subscribe((x) => _alwaysRegenerateNetlists = x);
    }
}