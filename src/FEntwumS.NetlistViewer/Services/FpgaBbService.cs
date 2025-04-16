using OneWare.Essentials.Services;
using FEntwumS.Common.Services;

namespace FEntwumS.NetlistViewer.Services;

public class FpgaBbService : IFpgaBbService
{
    private static string _currentBbCommand = "";

    private string _currentManufacturer = "";
    private string _currentDeviceFamily = "";

    private readonly ISettingsService _settingsService;
    private readonly ICustomLogger _logger;

    public FpgaBbService()
    {
        _settingsService = ServiceManager.GetService<ISettingsService>();
        _logger = ServiceManager.GetService<ICustomLogger>();
    }


    public void SubscribeToSettings()
    {
        _settingsService.GetSettingObservable<string>("NetlistViewer_FPGA_Manufacturer").Subscribe(x =>
        {
            _currentManufacturer = x;
            UpdateBbCommand();
            
            _logger.Log($"Manufacturer: {_currentManufacturer}");
            _logger.Log($"Command: {_currentBbCommand}");
        });
        _settingsService.GetSettingObservable<string>("NetlistViewer_FPGA_DeviceFamily").Subscribe(x =>
        {
            _currentDeviceFamily = x;
            UpdateBbCommand();
            
            _logger.Log($"DeviceFamily: {_currentDeviceFamily}");
            _logger.Log($"Command: {_currentBbCommand}");
        });
    }

    private void UpdateBbCommand()
    {
        switch (_currentManufacturer)
        {
            case "achronix":
                _currentBbCommand = "read_verilog -sv -lib +/achronix/speedster22i/cells_sim.v;";
                break;

            case "anlogic":
                _currentBbCommand = "read_verilog -lib +/anlogic/cells_sim.v +/anlogic/eagle_bb.v;";
                break;

            case "coolrunner2":
                _currentBbCommand = "read_verilog -lib +/coolrunner2/cells_sim.v;";
                break;

            case "ecp5":
                _currentBbCommand = "read_verilog -lib -specify +/ecp5/cells_sim.v +/ecp5/cells_bb.v;";
                break;

            case "efinix":
                _currentBbCommand = "read_verilog -lib +/efinix/cells_sim.v;";
                break;

            case "fabulous":
                _currentBbCommand = "read_verilog  -lib +/fabulous/prims.v;";
                break;

            case "gatemate":
                _currentBbCommand = "read_verilog -lib -specify +/gatemate/cells_sim.v +/gatemate/cells_bb.v;";
                break;

            case "gowin":
                if (_currentDeviceFamily == "gw1n" || _currentDeviceFamily == "gw2a" || _currentDeviceFamily == "gw5a")
                {
                    _currentBbCommand =
                        "read_verilog -specify -lib +/gowin/cells_sim.v; " + 
                        $"read_verilog -specify -lib +/gowin/cells_xtra_{_currentDeviceFamily}.v;";
                }
                else
                {
                    _logger.Error(
                        "The current combination of device manufacturer and device family is not valid. Valid device family options are: gw1n, gw2a or gw5a");
                }

                break;

            case "greenpak4":
                _currentBbCommand = "read_verilog -lib +/greenpak4/cells_sim.v;";
                break;

            case "ice40":
                _currentBbCommand = "read_verilog -D ICE40_HX -lib -specify +/ice40/cells_sim.v;";
                break;

            case "intel":
                _currentBbCommand =
                    "read_verilog -sv -lib +/intel/max10/cells_sim.v; " + 
                    "read_verilog -sv -lib +/intel/common/m9k_bb.v; " + 
                    "read_verilog -sv -lib +/intel/common/altpll_bb.v;";
                break;

            case "intel_alm":
                if (_currentDeviceFamily == "cyclonev")
                {
                    _currentBbCommand =
                        $"read_verilog -specify -lib -D {_currentDeviceFamily} +/intel_alm/common/alm_sim.v; " +
                        $"read_verilog -specify -lib -D {_currentDeviceFamily} +/intel_alm/common/dff_sim.v; " +
                        $"read_verilog -specify -lib -D {_currentDeviceFamily} +/intel_alm/common/dsp_sim.v; " +
                        $"read_verilog -specify -lib -D {_currentDeviceFamily} +/intel_alm/common/mem_sim.v; " +
                        $"read_verilog -specify -lib -D {_currentDeviceFamily} +/intel_alm/common/misc_sim.v; " +
                        $"read_verilog -specify -lib -D {_currentDeviceFamily} -icells +/intel_alm/common/abc9_model.v; " +
                        "read_verilog -lib +/intel/common/altpll_bb.v; " +
                        "read_verilog -lib +/intel_alm/common/megafunction_bb.v;";
                }
                else
                {
                    _logger.Error("The current combination of device manufacturer and device family is not valid. Valid device family options are: cyclonev");
                }

                break;
            
            case "lattice":
                _currentBbCommand = "read_verilog -lib -specify +/lattice/cells_sim.v +/lattice/cells_bb.v;";
                break;
            
            case "microchip":
                _currentBbCommand = "read_verilog -lib -specify +/microchip/cells_sim.v;";
                break;
            
            case "nanoxplore":
                _currentBbCommand =
                    "read_verilog -lib -specify +/nanoxplore/cells_sim.v +/nanoxplore/cells_sim.v +/nanoxplore/cells_bb.v +/nanoxplore/cells_bb.v;";
                break;
            
            case "nexus":
                _currentBbCommand = "read_verilog -lib -specify +/nexus/cells_sim.v +/nexus/cells_xtra.v;";
                break;
            
            case "quicklogic":
                if (_currentDeviceFamily == "pp3" || _currentDeviceFamily == "qlf_k6n10f")
                {
                    _currentBbCommand =
                        $"read_verilog -lib -specify +/quicklogic/common/cells_sim.v +/quicklogic/{_currentDeviceFamily}/cells_sim.v;";
                }
                else
                {
                    _logger.Error("The current combination of device manufacturer and device family is not valid. Valid device family options are: pp3 or qlf_k6n10f");
                }
                break;
            
            case "sf2":
                _currentBbCommand = "read_verilog -lib +/sf2/cells_sim.v;";
                break;
            
            case "xilinx":
                _currentBbCommand = "read_verilog -lib -specify +/xilinx/cells_sim.v; " +
                                   "read_verilog -lib +/xilinx/cells_xtra.v;";
                break;

            default:
                _logger.Error("Unknown device manufacturer");
                break;
        }
    }

    public string getBbCommand()
    {
        return _currentBbCommand;
    }
}