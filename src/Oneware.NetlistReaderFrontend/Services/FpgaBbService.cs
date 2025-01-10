using OneWare.Essentials.Services;

namespace Oneware.NetlistReaderFrontend.Services;

public class FpgaBbService : IFpgaBbService
{
    private static string currentBbCommand = "";

    private string currentManufacturer;
    private string currentDeviceFamily;

    private ISettingsService settingsService;
    private ICustomLogger logger;

    public FpgaBbService()
    {
        settingsService = ServiceManager.GetService<ISettingsService>();
        logger = ServiceManager.GetService<ICustomLogger>();
    }


    public void SubscribeToSettings()
    {
        settingsService.GetSettingObservable<string>("NetlistViewer_FPGA_Manufacturer").Subscribe(x =>
        {
            currentManufacturer = x;
            UpdateBbCommand();
            
            logger.Log($"Manufacturer: {currentManufacturer}");
            logger.Log($"Command: {currentBbCommand}");
        });
        settingsService.GetSettingObservable<string>("NetlistViewer_FPGA_DeviceFamily").Subscribe(x =>
        {
            currentDeviceFamily = x;
            UpdateBbCommand();
            
            logger.Log($"DeviceFamily: {currentDeviceFamily}");
            logger.Log($"Command: {currentBbCommand}");
        });
    }

    private void UpdateBbCommand()
    {
        switch (currentManufacturer)
        {
            case "achronix":
                currentBbCommand = "read_verilog -sv -lib +/achronix/speedster22i/cells_sim.v;";
                break;

            case "anlogic":
                currentBbCommand = "read_verilog -lib +/anlogic/cells_sim.v +/anlogic/eagle_bb.v;";
                break;

            case "coolrunner2":
                currentBbCommand = "read_verilog -lib +/coolrunner2/cells_sim.v;";
                break;

            case "ecp5":
                currentBbCommand = "read_verilog -lib -specify +/ecp5/cells_sim.v +/ecp5/cells_bb.v;";
                break;

            case "efinix":
                currentBbCommand = "read_verilog -lib +/efinix/cells_sim.v;";
                break;

            case "fabulous":
                currentBbCommand = "read_verilog  -lib +/fabulous/prims.v;";
                break;

            case "gatemate":
                currentBbCommand = "read_verilog -lib -specify +/gatemate/cells_sim.v +/gatemate/cells_bb.v;";
                break;

            case "gowin":
                if (currentDeviceFamily == "gw1n" || currentDeviceFamily == "gw2a" || currentDeviceFamily == "gw5a")
                {
                    currentBbCommand =
                        "read_verilog -specify -lib +/gowin/cells_sim.v; " + 
                        $"read_verilog -specify -lib +/gowin/cells_xtra_{currentDeviceFamily}.v;";
                }
                else
                {
                    logger.Error(
                        "The current combination of device manufacturer and device family is not valid. Valid device family options are: gw1n, gw2a or gw5a");
                }

                break;

            case "greenpak4":
                currentBbCommand = "read_verilog -lib +/greenpak4/cells_sim.v;";
                break;

            case "ice40":
                currentBbCommand = "read_verilog -D ICE40_HX -lib -specify +/ice40/cells_sim.v;";
                break;

            case "intel":
                currentBbCommand =
                    "read_verilog -sv -lib +/intel/max10/cells_sim.v; " + 
                    "read_verilog -sv -lib +/intel/common/m9k_bb.v; " + 
                    "read_verilog -sv -lib +/intel/common/altpll_bb.v;";
                break;

            case "intel_alm":
                if (currentDeviceFamily == "cyclonev")
                {
                    currentBbCommand =
                        $"read_verilog -specify -lib -D {currentDeviceFamily} +/intel_alm/common/alm_sim.v; " +
                        $"read_verilog -specify -lib -D {currentDeviceFamily} +/intel_alm/common/dff_sim.v; " +
                        $"read_verilog -specify -lib -D {currentDeviceFamily} +/intel_alm/common/dsp_sim.v; " +
                        $"read_verilog -specify -lib -D {currentDeviceFamily} +/intel_alm/common/mem_sim.v; " +
                        $"read_verilog -specify -lib -D {currentDeviceFamily} +/intel_alm/common/misc_sim.v; " +
                        $"read_verilog -specify -lib -D {currentDeviceFamily} -icells +/intel_alm/common/abc9_model.v; " +
                        "read_verilog -lib +/intel/common/altpll_bb.v; " +
                        "read_verilog -lib +/intel_alm/common/megafunction_bb.v;";
                }
                else
                {
                    logger.Error("The current combination of device manufacturer and device family is not valid. Valid device family options are: cyclonev");
                }

                break;
            
            case "lattice":
                currentBbCommand = "read_verilog -lib -specify +/lattice/cells_sim.v +/lattice/cells_bb.v;";
                break;
            
            case "microchip":
                currentBbCommand = "read_verilog -lib -specify +/microchip/cells_sim.v;";
                break;
            
            case "nanoxplore":
                currentBbCommand =
                    "read_verilog -lib -specify +/nanoxplore/cells_sim.v +/nanoxplore/cells_sim.v +/nanoxplore/cells_bb.v +/nanoxplore/cells_bb.v;";
                break;
            
            case "nexus":
                currentBbCommand = "read_verilog -lib -specify +/nexus/cells_sim.v +/nexus/cells_xtra.v;";
                break;
            
            case "quicklogic":
                if (currentDeviceFamily == "pp3" || currentDeviceFamily == "qlf_k6n10f")
                {
                    currentBbCommand =
                        $"read_verilog -lib -specify +/quicklogic/common/cells_sim.v +/quicklogic/{currentDeviceFamily}/cells_sim.v;";
                }
                else
                {
                    logger.Error("The current combination of device manufacturer and device family is not valid. Valid device family options are: pp3 or qlf_k6n10f");
                }
                break;
            
            case "sf2":
                currentBbCommand = "read_verilog -lib +/sf2/cells_sim.v;";
                break;
            
            case "xilinx":
                currentBbCommand = "read_verilog -lib -specify +/xilinx/cells_sim.v; " +
                                   "read_verilog -lib +/xilinx/cells_xtra.v;";
                break;

            default:
                logger.Error("Unknown device manufacturer");
                break;
        }
    }

    public string getBbCommand()
    {
        return currentBbCommand;
    }
}