using System.Collections.ObjectModel;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using FEntwumS.Common;
using FEntwumS.Common.Services;
using FEntwumS.NetlistViewer.Services;
using FEntwumS.WaveformInteractor.Services;
using FEntwumS.WaveformInteractor.ViewModels;
using Newtonsoft.Json.Linq;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.UniversalFpgaProjectSystem.Models;
using OneWare.Vcd.Parser.Data;
using OneWare.Vcd.Viewer.Models;
using OneWare.Vcd.Viewer.ViewModels;
using Prism.Ioc;
using Prism.Modularity;
using ILogger = OneWare.Essentials.Services.ILogger;
using IYosysService = FEntwumS.Common.Services.IYosysService;

namespace FEntwumS.WaveformInteractor;

public class FEntwumSWaveformInteractorModule : IModule
{
    private ObservableCollection<ExtendedVcdScopeModel>? _fentwumsScopes;
    private HttpClient? _httpClient;
    private ILogger? _logger;
    private ISettingsService? _settingsService;
    
    private IContainerProvider _containerProvider; 

    private IProjectExplorerService? _projectExplorerService;
    private SignalBitIndexService? _signalBitIndexService;
    private IVerilatorService? _verilatorService;
    private IWaveformInteractorService _waveformInteractorService;
    private IWindowService? _windowService;
    private IYosysService? _yosysSimService;
    private NetlistService _netlistService;

    // holds all Signals
    // private VcdDefinition? rootScope;
    // private string _backendAddress = "http://127.0.0.1";
    // private string _backendPort = "8080";

    public FEntwumSWaveformInteractorModule(IWaveformInteractorService? waveformInteractorService = null)
    {
        _waveformInteractorService = waveformInteractorService;
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IYosysService, YosysSimService>();
        containerRegistry.RegisterSingleton<IVerilatorService, VerilatorService>();
        containerRegistry.RegisterSingleton<SignalBitIndexService>();
        containerRegistry.RegisterSingleton<INetlistService, NetlistService>();
        containerRegistry.Register<IWaveformInteractorService, WaveformInteractorService>();
        containerRegistry.Register<WaveformInteractorViewModel>();
        // containerRegistry.Register<IFrontendService, FrontendService>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        _containerProvider = containerProvider;
        _yosysSimService = containerProvider.Resolve<IYosysService>();
        _verilatorService = containerProvider.Resolve<IVerilatorService>();
        _signalBitIndexService = containerProvider.Resolve<SignalBitIndexService>();
        _waveformInteractorService = containerProvider.Resolve<IWaveformInteractorService>();
        _netlistService = containerProvider.Resolve<NetlistService>();
        // OneWare Services
        var dockService = containerProvider.Resolve<IDockService>();
        _projectExplorerService = containerProvider.Resolve<IProjectExplorerService>();
        _windowService = containerProvider.Resolve<IWindowService>();
        _logger = containerProvider.Resolve<ILogger>();
        _settingsService = containerProvider.Resolve<ISettingsService>();

        // for now register Menu which handles functionality
        _windowService.RegisterMenuItem("MainWindow_MainMenu/Verilator",
            new MenuItemViewModel("Create_Verilator_Binary")
            {
                Header = "Create Verilator Binary",
                Command = new AsyncRelayCommand(CreateVerilatorBinaryAllStepsAsync),
                IconObservable = Application.Current!.GetResourceObservable("CreateIcon")
            },
            new MenuItemViewModel("Run_Verilator_Binary")
            {
                Header = "Run Verilator Binary",
                Command = new AsyncRelayCommand(RunVerilatorExecutableFromToplevelAsync),
                IconObservable = Application.Current!.GetResourceObservable("CreateIcon")
            });

        _projectExplorerService.RegisterConstructContextMenu((selected, menuItems) =>
        {
            if (selected is [IProjectFile { Extension: ".cpp" } cppFile])
            {
                var filepath = cppFile.FullPath;
                var testbench = _verilatorService.Testbench;
                var testbenchpath = testbench != null ? testbench.FullPath : string.Empty;

                if (string.Equals(filepath, testbenchpath) == false)
                    menuItems.Add(new MenuItemViewModel("Set as Verilator testbench")
                    {
                        Header = "Set as Verilator testbench",
                        Command = new RelayCommand(() => _verilatorService.RegisterTestbench(cppFile)),
                        IconObservable = Application.Current!.GetResourceObservable("VSImageLib.AddTest_16x")
                    });
                else
                    // TODO: change icon, if testbench is set.
                    menuItems.Add(new MenuItemViewModel("Unset Verilator testbench")
                    {
                        Header = "Unset Verilator testbench",
                        Command = new RelayCommand(() => _verilatorService.UnregisterTestbench(cppFile)),
                        IconObservable =
                            Application.Current!.GetResourceObservable("VSImageLib.RemoveSingleDriverTest_16x")
                    });
            }
        });

        dockService.PropertyChanged += (o, args) =>
        {
            if (args.PropertyName != nameof(dockService.CurrentDocument)) return;
            var currentDocument = dockService.CurrentDocument;

            if (currentDocument is VcdViewModel vcdViewModel)
            {
                vcdViewModel.PropertyChanged += (o1, innerArgs) =>
                {
                    switch (innerArgs.PropertyName)
                    {
                        case nameof(vcdViewModel.IsLoading):
                            if(!vcdViewModel.IsLoading)
                                _ = HandleIsLoadingChangedAsync(vcdViewModel);
                            break;
                        // Subscribe to PropertyChanged for SelectedSignal in WaveformViewer
                        case nameof(vcdViewModel.WaveFormViewer.SelectedSignal):
                            var selectedSignal = vcdViewModel.SelectedSignal;

                            // TODO: How to map from WaveformViewer to Bit Index?
                            // currently use signalname as UID to get bit indices
                            // then map bit indices to VCD ID
                            // -> Signalname still have to be used initially, otherwise no way to map from netlist to vcd
                            
                            var bits = _signalBitIndexService.GetMapping(selectedSignal.Id);

                            // jump to selected Signal via bit index
                            _waveformInteractorService.GoToSignal(bits.BitIndexId);
                            break;
                    }
                };
            }
        };

        // wait until project launches and active project may be fetched
        _projectExplorerService.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName != nameof(_projectExplorerService.ActiveProject)) return;
            var currentProject = _projectExplorerService.ActiveProject;

            if (currentProject != null)
            {
                // set first testbench from OneWare Project as _verilatorTestbench
                var project = _projectExplorerService.ActiveProject?.Root as UniversalFpgaProjectRoot;
                _verilatorService.RegisterTestbench(project?.TestBenches.FirstOrDefault());
            }
        };
        
        // Subscribe to Settings from Frontend. Expects valid address and port
        // TODO: Signaling required, to indicate that settings have been Registered from Frontend. 
        // _settingsService.GetSettingObservable<string>("NetlistViewer_Backend_Address").Subscribe(x =>
        // {
        //     _backendAddress = x;
        // });
        //
        // _settingsService.GetSettingObservable<string>("NetlistViewer_Backend_Port").Subscribe(x =>
        // {
        //     _backendPort = x;
        // });
    }
    
    private async Task HandleIsLoadingChangedAsync(VcdViewModel vcdViewModel)
    {
        try
        {
            // ensure that backend is running
            var frontendService = _containerProvider.Resolve<IFrontendService>();
            _ = frontendService.StartBackendIfNotStartedAsync();

            // Get current scopes
            _netlistService.OneWareScopes = vcdViewModel.Scopes;
            
            var projectRoot = _projectExplorerService.ActiveProject.Root as UniversalFpgaProjectRoot;
            var topEntity = Path.GetFileNameWithoutExtension(projectRoot.TopEntity.FullPath);
            var netlistPath = _projectExplorerService.ActiveProject.RootFolderPath + $"/build/netlist/{topEntity}.json";
            
            // post netlist to backend
            // dont post if netlist already present in backend?
            var netInfo = await _netlistService.GetNetInformationAsync(netlistPath);
            if (!netInfo.HasValues)
            {
                await _netlistService.PostNetlistToBackendAsync(netlistPath);
                netInfo = await _netlistService.GetNetInformationAsync(netlistPath);
            }
            _netlistService.ParseNetInformation(netInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in HandleIsLoadingChangedAsync: {ex}");
        }
    }

    // executes compiled verilator binary  
    private async Task RunVerilatorExecutableFromToplevelAsync()
    {
        var projectRoot = _projectExplorerService.ActiveProject.Root as UniversalFpgaProjectRoot;
        var path = projectRoot.TopEntity.FullPath;
        var topFile = projectRoot.Files.FirstOrDefault(file => file.FullPath == path);

        await _verilatorService.RunExecutableAsync(topFile);
    }
    
    // TODO: refactor to VerilatorService
    // requires verilator testbench, and toplevel entity to be set.
    private async Task CreateVerilatorBinaryAllStepsAsync()
    {
        var projectRoot = _projectExplorerService.ActiveProject.Root as UniversalFpgaProjectRoot;
        var path = projectRoot.TopEntity.FullPath;
        var topFile = projectRoot.Files.FirstOrDefault(file => file.FullPath == path);
        var verilatorServiceTestbench = _verilatorService.Testbench;

        if (topFile != null && verilatorServiceTestbench != null)
        {
            await _yosysSimService.LoadVerilogAsync(topFile);
            await _verilatorService.VerilateAsync(topFile);
            await _verilatorService.CompileVerilatedAsync(topFile);
        }
        else
        {
            if (topFile == null && verilatorServiceTestbench != null)
                _logger.Error("Toplevel Entity must be set!", null, true, true);
            if (topFile != null && verilatorServiceTestbench == null)
                _logger.Error("Verilator Testbench must be set!", null, true, true);
            if (topFile == null && verilatorServiceTestbench == null)
                _logger.Error("Toplevel Entity and Verilator Testbench must be set!", null, true, true);
        }
    }
}