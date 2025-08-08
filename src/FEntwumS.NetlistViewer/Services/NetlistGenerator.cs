using System.Globalization;
using Avalonia.Threading;
using DynamicData.Binding;
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
	private readonly IStorageService _storageService;

	private bool _alwaysRegenerateNetlists = true;

	private DispatcherTimer _timer = new();
	private double _generationInterval = 60.0d;

	private List<FileSystemWatcher> _watchers = new();
	private readonly Lock _lock = new();
	private HashSet<UniversalFpgaProjectRoot> _changedProjectSet = new();
	private AutomaticNetlistGenerationType _generationType = AutomaticNetlistGenerationType.Never;
	// private bool _settingsLoaded = false;

	public NetlistGenerator()
	{
		_logger = ServiceManager.GetCustomLogger();
		_settingsService = ServiceManager.GetService<ISettingsService>();
		_projectExplorerService = ServiceManager.GetService<IProjectExplorerService>();
		_storageService = ServiceManager.GetService<IStorageService>();

		// Add or remove watchers as necessary when loaded projects list changes
		_projectExplorerService.Projects.CollectionChanged += (sender, args) => { SetupWatchers(); };

		_timer.Interval = TimeSpan.FromSeconds(_generationInterval);
	}

	public async Task<bool> GenerateVhdlNetlistAsync(IProjectFile vhdlProject)
	{
		OneWare.GhdlExtension.Services.GhdlService ghdlService =
			ServiceManager.GetService<OneWare.GhdlExtension.Services.GhdlService>();

		string outputDir = FentwumSNetlistViewerSettingsHelper.GetBuildDirectory(vhdlProject);

		if (!Directory.Exists(outputDir))
		{
			Directory.CreateDirectory(outputDir);
		}

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

	public async Task<(IProjectFile? netlistFile, bool success)> GenerateNetlistAsync(IProjectFile? projectFile,
		NetlistLanguage netlistLanguage, NetlistType netlistType)
	{
		if (projectFile is null)
		{
			return (null, false);
		}

		bool success;
		IProjectFile? netlistFile;

		if (!_alwaysRegenerateNetlists)
		{
			(netlistFile, success) = GetExistingNetlist(projectFile, netlistType);

			if (success)
			{
				return (netlistFile, true);
			}
		}

		switch (netlistLanguage)
		{
			case NetlistLanguage.VHDL:
				success = await GenerateVhdlNetlistAsync(projectFile) && await GenerateVerilogNetlistAsync(projectFile);
				break;

			case NetlistLanguage.Verilog:
				success = await GenerateVerilogNetlistAsync(projectFile);
				break;

			case NetlistLanguage.System_Verilog:
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
		string netlistPath = FentwumSNetlistViewerSettingsHelper.GetNetlistFilePath(projectFile, netlistType);

		if (!File.Exists(netlistPath))
		{
			_logger.Error($"Netlist file not found: {netlistPath}");

			return (null, false);
		}

		netlistFile = new ProjectFile(netlistPath, projectFile.TopFolder!);

		return (netlistFile, true);
	}

	public (IProjectFile? netlistFile, bool success) GetExistingNetlist(IProjectFile projectFile,
		NetlistType netlistType)
	{
		if (projectFile.Root is not UniversalFpgaProjectRoot root)
		{
			return (null, false);
		}

		string top = Path.GetFileNameWithoutExtension(projectFile.FullPath);
		string netlistPath = FentwumSNetlistViewerSettingsHelper.GetNetlistFilePath(projectFile, netlistType);

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

		string? storedSettingsChangedTime = _storageService.GetKeyValuePairValue(FentwumSNetlistViewerSettingsHelper
			.NetlistGenerationSettingsChangedKey);

		// Date format "R" is the RFC1123 pattern
		DateTime settingsChangedTime = DateTime.ParseExact(
			storedSettingsChangedTime ?? DateTime.Now.ToString("R"), "R", new DateTimeFormatInfo());

		if (storedSettingsChangedTime is null)
		{
			_storageService.SetKeyValuePairValue(
				FentwumSNetlistViewerSettingsHelper.NetlistGenerationSettingsChangedKey, DateTime.Now.ToString("R"));
			_ = _storageService.SaveAsync();
		}
		else if (netlistFile.LastWriteTimeUtc.CompareTo(settingsChangedTime.ToUniversalTime()) <= 0)
		{
			newNetlistNecessary = true;
		}

		if (newNetlistNecessary)
		{
			return (null, false);
		}

		return (new ProjectFile(netlistPath, projectFile.TopFolder!), true);
	}

	private void TimerTick(object? sender, EventArgs e)
	{
		_ = ProcessQueueAsync();
	}

	private void SetupWatchers()
	{
		IEnumerable<IProjectRoot> missingWatchers =
			_projectExplorerService.Projects.Where(
				project => _watchers.All(watcher => watcher.Path != project.FullPath));
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

				_watchers.Add(watcher);
			}
		}
	}

	private void ProjectChanged(object sender, FileSystemEventArgs e)
	{
		// Don't process changes to projects, if the user wants this
		if (_generationType == AutomaticNetlistGenerationType.Never)
		{
			return;
		}

		// Only add project to regeneration queue if a supported HDL source file has changed
		if (Path.GetExtension(e.FullPath) is not (".vhdl" or ".vhd" or ".v" or ".sv"))
		{
			return;
		}

		if (sender is FileSystemWatcher watcher)
		{
			IEnumerable<IProjectRoot> candidates =
				_projectExplorerService.Projects.Where(project => watcher.Path == project.FullPath);
			List<IProjectRoot> candidatesList = candidates.ToList();

			UniversalFpgaProjectRoot? projectCandidate =
				candidatesList.FirstOrDefault(x => x is UniversalFpgaProjectRoot) as UniversalFpgaProjectRoot;

			if (projectCandidate is null)
			{
				_logger.Error($"Project {e.FullPath} was not found");
			}
			else
			{
				// Don't add the project to the regeneration queue if the changed file is the intermediate verilog file generated for VHDL designs
				if (projectCandidate.Files.All(x => x.FullPath != e.FullPath))
				{
					return;
				}

				lock (_lock)
				{
					_changedProjectSet.Add(projectCandidate);
				}

				// Directly process the change, if the user wants this
				if (_generationType == AutomaticNetlistGenerationType.Always)
				{
					_ = ProcessQueueAsync();
				}
			}
		}
	}

	private async Task ProcessQueueAsync()
	{
		List<UniversalFpgaProjectRoot> projects = new();

		lock (_lock)
		{
			projects.AddRange(_changedProjectSet);
		}

		foreach (UniversalFpgaProjectRoot projectRoot in projects)
		{
			NetlistLanguage netlistLanguage = NetlistLanguage.Verilog;

			if (projectRoot.Files.Any(x => x.Extension is ".vhdl" or ".vhd"))
			{
				netlistLanguage = NetlistLanguage.VHDL;
			}
			else if (projectRoot.Files.Any(x => x.Extension is ".sv"))
			{
				netlistLanguage = NetlistLanguage.System_Verilog;
			}

			// It is sadly necessary to run the netlist generation via the UI thread to avoid exceptions regarding
			// DockService.Show inside the GhdlService
			await Dispatcher.UIThread.InvokeAsync(async () =>
			{
				await GenerateNetlistAsync((IProjectFile?)projectRoot.TopEntity, netlistLanguage, NetlistType.Hier);
			});

			lock (_lock)
			{
				_changedProjectSet.Remove(projectRoot);
			}
		}
	}

	public void SubscribeToSettings()
	{
		_settingsService.GetSettingObservable<bool>(FentwumSNetlistViewerSettingsHelper.AlwaysRegenerateNetlistsKey)
			.Subscribe((x) => _alwaysRegenerateNetlists = x);

		_settingsService.GetSettingObservable<string>(FentwumSNetlistViewerSettingsHelper.AutomaticNetlistGenerationKey)
			.Subscribe(
				x =>
				{
					_generationType = x switch
					{
						"Never" => AutomaticNetlistGenerationType.Never,
						"Always" => AutomaticNetlistGenerationType.Always,
						"Interval" => AutomaticNetlistGenerationType.Interval,
						_ => AutomaticNetlistGenerationType.Never,
					};

					if (_generationType == AutomaticNetlistGenerationType.Interval)
					{
						_timer.IsEnabled = true;
						_timer.Tick += TimerTick;
						_timer.Interval = TimeSpan.FromSeconds(_generationInterval);
						_timer.Start();
					}
					else
					{
						_timer.IsEnabled = false;
						_timer.Tick -= TimerTick;
						_timer.Stop();
					}
				});

		_settingsService
			.GetSettingObservable<double>(FentwumSNetlistViewerSettingsHelper.AutomaticNetlistGenerationIntervalKey)
			.Subscribe(
				x =>
				{
					_generationInterval = x;

					if (_generationType == AutomaticNetlistGenerationType.Interval)
					{
						_timer.Interval = TimeSpan.FromSeconds(_generationInterval);
					}
				});

		// _settingsService.GetSettingObservable<bool>(FentwumSNetlistViewerSettingsHelper.UseHierarchicalBackendKey)
		//     .Subscribe(x =>
		//     {
		//         if (!_settingsLoaded)
		//         {
		//             _settingsLoaded = true;
		//             return;
		//         }
		//         
		//         _storageService.SetKeyValuePairValue(FentwumSNetlistViewerSettingsHelper.NetlistGenerationSettingsChangedKey, DateTime.Now.ToString("R"));
		//         _ = _storageService.SaveAsync();
		//     });
	}
}