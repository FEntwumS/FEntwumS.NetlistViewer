using Avalonia.Media;
using FEntwumS.Common.Interfaces;
using FEntwumS.Common.Services;
using FEntwumS.NetlistViewer.Helpers;
using Microsoft.Extensions.Logging;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.UniversalFpgaProjectSystem.Models;

namespace FEntwumS.NetlistViewer.Services;

public class YosysService : IYosysService
{
	private ISettingsService _settingsService;
	private IToolExecuterService _toolExecuterService;
	private IFpgaBbService _fpgaBbService;
	private ILogger _logger;
	private bool _useHierarchicalBackend;

	private string _yosysPath = string.Empty;

	public YosysService()
	{
		_settingsService = ServiceManager.GetService<ISettingsService>();
		_toolExecuterService = ServiceManager.GetService<IToolExecuterService>();
		_fpgaBbService = ServiceManager.GetService<IFpgaBbService>();
		_logger = ServiceManager.GetService<ILogger>();
	}

	public void SubscribeToSettings()
	{
		_settingsService.GetSettingObservable<string>(FentwumSNetlistViewerSettingsHelper.OssCadSuitePathKey)
			.Subscribe(x => _yosysPath = Path.Combine(x, "bin", "yosys"));
		_settingsService.GetSettingObservable<bool>(FentwumSNetlistViewerSettingsHelper.UseHierarchicalBackendKey)
			.Subscribe(x => _useHierarchicalBackend = x);
	}

	public Task<bool> LoadVhdlAsync(IProjectFile file, string topEntityName)
	{
		// This method has not been implemented due to the Windows version of the oss cad suite not including the
		// ghdl-yosys plugin

		throw new NotImplementedException();
	}

	public async Task<bool> LoadVerilogAsync(IProjectFile file, string topEntityName)
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
			IEnumerable<string> verilogFiles = root.GetFiles("*.v")
				.Where(x => !root.IsCompileExcluded(x)) // Exclude excluded files
				.Where(x => !root.IsTestBench(x)) // Exclude testbenches
				.Select(x => Path.Combine(root.TopFolder.FullPath, x));
			
			IEnumerable<string> systemVerilogFiles = root.GetFiles("*.sv")
				.Where(x => !root.IsCompileExcluded(x)) // Exclude excluded files
				.Where(x => !root.IsTestBench(x)) // Exclude testbenches
				.Select(x => Path.Combine(root.TopFolder.FullPath, x));

			verilogFileList.AddRange(verilogFiles);
			systemVerilogFileList.AddRange(systemVerilogFiles);
		}

		if (verilogFileList.Count == 0 && systemVerilogFileList.Count == 0)
		{
			verilogFileList.Add(file.FullPath);
			ServiceManager.GetService<ILogger>().Warning("No files where included for netlist generation. This is due to all files being either marked as testbenches or as excluded from compilation", null, true);
			ServiceManager.GetService<ILogger>().Log("Including the selected file and proceeding", true, Brushes.White);
		}

		bool noErrors = true;

		var yosysCommand = ServiceManager.GetService<IToolExecutionDispatcherService>()
			.CreateToolCommandBuilder("yosys")
			.WithStatus("Executing yosys")
			.WithTimer(true)
			.WithWorkingDirectory(workingDirectory)
			.WithOutputHandler(x =>
			{
				_logger.Log(x);
				return true;
			})
			.WithErrorHandler(x =>
			{
				if (x.Contains("ERROR:") || x.Contains("error:"))
				{
					noErrors = false;
				}

				_logger.Error(x);
				return true;
			})
			.Add("-m")
			.Add("slang")
			.Add("-p")
			.AddScript("read_verilog -sv -nooverwrite {verilogFiles} {systemVerilogFiles}; "
			           + "scratchpad -set flatten.separator \";\"; " // Use the semicolon as hierarchy separator; See https://yosyshq.readthedocs.io/projects/yosys/en/v0.55/cmd/flatten.html
			           + "{blackBoxLoadingCommand} "
			           + "hierarchy -check -purge_lib -top {topEntityName}; " // Check and build the design hierarchy. Unused modules and blackboxes are discarded
			           + "proc; " // proc needs to run because JSON netlists produced by yosys may not contain RTLIL processes
			           + "memory -nomap; " // Converts memories into simple blocks instead of basic cells
			           + "select *; " // Remove unnecessary library elements from the netlist
			           + "write_json -compat-int {topEntityName}-hier.json; " // Write hierarchical JSON netlist to disk
			           + "scratchpad -set flatten.separator \';\'; "
			           + "flatten -scopename; " // Flatten the netlist
			           + "select *; "
			           + "clean; "
			           + "write_json -compat-int {topEntityName}-flat.json", // Write flattened JSON netlist to disk
				("{verilogFiles}",
					$"{(verilogFileList.Count > 0 ? "\"" + string.Join("\" \"", verilogFileList) + "\"" : "")}"),
				("{systemVerilogFiles}",
					$"{(systemVerilogFileList.Count > 0 ? "\"" + string.Join("\" \"", systemVerilogFileList) + "\"" : "")}"),
				("{blackBoxLoadingCommand}", $"{_fpgaBbService.getBbCommand(file)}"),
				("{topEntityName}", $"{top}"))
			.Build();

		(bool success, _) =
			await ServiceManager.GetService<IToolExecutionDispatcherService>().ExecuteAsync(yosysCommand);

		return success && noErrors;
	}

	public async Task<bool> LoadSystemVerilogAsync(IProjectFile file, string topEntityName)
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
		IEnumerable<string> files = root.GetFiles("*.sv")
			.Where(x => !root.IsCompileExcluded(x)) // Exclude excluded files
			.Where(x => !root.IsTestBench(x)) // Exclude testbenches
			.Select(x => x);
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