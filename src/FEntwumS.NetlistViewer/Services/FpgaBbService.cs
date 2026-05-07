using FEntwumS.NetlistViewer.Helpers;
using Microsoft.Extensions.Logging;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.UniversalFpgaProjectSystem.Models;

namespace FEntwumS.NetlistViewer.Services;

public class FpgaBbService : IFpgaBbService
{
	private static string _currentBbCommand = "";

	private string _currentManufacturer = "";
	private string _currentDeviceFamily = "";

	private readonly ISettingsService _settingsService;
	private readonly ILogger _logger;

	public FpgaBbService()
	{
		_settingsService = ServiceManager.GetService<ISettingsService>();
		_logger = ServiceManager.GetService<ILogger>();
	}


	public void SubscribeToSettings()
	{
		_settingsService.GetSettingObservable<string>(FentwumSNetlistViewerSettingsHelper.FpgaManufacturerKey)
			.Subscribe(x =>
			{
				_currentManufacturer = x;
				UpdateBbCommand(_currentManufacturer, _currentDeviceFamily);

				_logger.Log($"Manufacturer: {_currentManufacturer}");
				_logger.Log($"Command: {_currentBbCommand}");
			});
		_settingsService.GetSettingObservable<string>(FentwumSNetlistViewerSettingsHelper.FpgaDeviceFamilyKey)
			.Subscribe(x =>
			{
				_currentDeviceFamily = x;
				UpdateBbCommand(_currentManufacturer, _currentDeviceFamily);

				_logger.Log($"DeviceFamily: {_currentDeviceFamily}");
				_logger.Log($"Command: {_currentBbCommand}");
			});
	}

	private void UpdateBbCommand(string manufacturer = "", string deviceFamily = "")
	{
		_currentBbCommand = getBbCommand();
	}

	public string getBbCommand(IProjectFile? file = null)
	{
		string? manufacturer = _currentManufacturer;
		string? deviceFamily = _currentDeviceFamily;

		if (file is null || file.Root is not UniversalFpgaProjectRoot root)
		{
			_logger.Warning($"{file?.Name} is not associated with an FPGA project. Falling back to global settings", null, false);
		}
		else
		{
			manufacturer = root.Properties.GetString(FentwumSNetlistViewerSettingsHelper.ProjectFpgaManufacturerKey);

			if (manufacturer is null)
			{
				manufacturer = _currentManufacturer;
				root.Properties.SetString(FentwumSNetlistViewerSettingsHelper.ProjectFpgaManufacturerKey, manufacturer);
			}

			deviceFamily = root.Properties.GetString(FentwumSNetlistViewerSettingsHelper.ProjectFpgaDeviceFamilyKey);

			if (deviceFamily is null)
			{
				deviceFamily = _currentDeviceFamily;
				root.Properties.SetString(FentwumSNetlistViewerSettingsHelper.ProjectFpgaDeviceFamilyKey, deviceFamily);
			}
		}

		switch (manufacturer)
		{
			case "achronix":
				return "read_verilog -sv -lib -overwrite +/achronix/speedster22i/cells_sim.v;";

			case "anlogic":
				return "read_verilog -lib -overwrite +/anlogic/cells_sim.v +/anlogic/eagle_bb.v;";

			case "coolrunner2":
				return "read_verilog -lib -overwrite +/coolrunner2/cells_sim.v;";

			case "ecp5":
				return "read_verilog -lib -specify -overwrite +/ecp5/cells_sim.v +/ecp5/cells_bb.v;";

			case "efinix":
				return "read_verilog -lib -overwrite +/efinix/cells_sim.v;";

			case "fabulous":
				return "read_verilog -lib -overwrite +/fabulous/prims.v;";

			case "gatemate":
				return "read_verilog -lib -specify -overwrite +/gatemate/cells_sim.v +/gatemate/cells_bb.v;";

			case "gowin":
				if (deviceFamily is "gw1n" or "gw2a" or "gw5a")
				{
					return
						"read_verilog -specify -lib -overwrite +/gowin/cells_sim.v; " +
						$"read_verilog -specify -lib -overwrite +/gowin/cells_xtra_{deviceFamily}.v;";
				}

				_logger.Error(
					"The current combination of device manufacturer and device family is not valid. Valid device family options are: gw1n, gw2a or gw5a", null, false);

				break;

			case "greenpak4":
				return "read_verilog -lib -overwrite +/greenpak4/cells_sim.v;";

			case "ice40":
				return "read_verilog -D ICE40_HX -lib -specify -overwrite +/ice40/cells_sim.v;";

			case "intel":
				return
					"read_verilog -sv -lib -overwrite +/intel/max10/cells_sim.v; " +
					"read_verilog -sv -lib -overwrite +/intel/common/m9k_bb.v; " +
					"read_verilog -sv -lib -overwrite +/intel/common/altpll_bb.v;";

			case "intel_alm":
				if (deviceFamily is "cyclonev")
				{
					return
						$"read_verilog -specify -lib -D {deviceFamily} -overwrite +/intel_alm/common/alm_sim.v; " +
						$"read_verilog -specify -lib -D {deviceFamily} -overwrite +/intel_alm/common/dff_sim.v; " +
						$"read_verilog -specify -lib -D {deviceFamily} -overwrite +/intel_alm/common/dsp_sim.v; " +
						$"read_verilog -specify -lib -D {deviceFamily} -overwrite +/intel_alm/common/mem_sim.v; " +
						$"read_verilog -specify -lib -D {deviceFamily} -overwrite +/intel_alm/common/misc_sim.v; " +
						$"read_verilog -specify -lib -D {deviceFamily} -overwrite -icells +/intel_alm/common/abc9_model.v; " +
						"read_verilog -lib -overwrite +/intel/common/altpll_bb.v; " +
						"read_verilog -lib -overwrite +/intel_alm/common/megafunction_bb.v;";
				}

				_logger.Error(
					"The current combination of device manufacturer and device family is not valid. Valid device family options are: cyclonev");

				break;

			case "lattice":
				return "read_verilog -lib -specify -overwrite +/lattice/cells_sim.v +/lattice/cells_bb.v;";

			case "microchip":
				return "read_verilog -lib -specify -overwrite +/microchip/cells_sim.v;";

			case "nanoxplore":
				return
					"read_verilog -lib -specify -overwrite +/nanoxplore/cells_sim.v +/nanoxplore/cells_sim.v +/nanoxplore/cells_bb.v +/nanoxplore/cells_bb.v;";

			case "nexus":
				return "read_verilog -lib -specify -overwrite +/nexus/cells_sim.v +/nexus/cells_xtra.v;";

			case "quicklogic":
				if (deviceFamily is "pp3" or "qlf_k6n10f")
				{
					return
						$"read_verilog -lib -specify -overwrite +/quicklogic/common/cells_sim.v +/quicklogic/{deviceFamily}/cells_sim.v;";
				}

				_logger.Error(
					"The current combination of device manufacturer and device family is not valid. Valid device family options are: pp3 or qlf_k6n10f");
				break;

			case "sf2":
				return "read_verilog -lib -overwrite +/sf2/cells_sim.v;";

			case "xilinx":
				return "read_verilog -lib -overwrite -specify +/xilinx/cells_sim.v; " +
				       "read_verilog -lib -overwrite +/xilinx/cells_xtra.v;";

			default:
				_logger.Error("Unknown device manufacturer");
				break;
		}

		return _currentBbCommand;
	}
}