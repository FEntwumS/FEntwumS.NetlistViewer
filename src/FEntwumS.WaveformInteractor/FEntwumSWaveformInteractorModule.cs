using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using FEntwumS.Common;
using FEntwumS.Common.Services;
using FEntwumS.NetlistViewer.Services;
using FEntwumS.WaveformInteractor.Services;
using FEntwumS.WaveformInteractor.ViewModels;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.UniversalFpgaProjectSystem.Models;
using OneWare.Vcd.Viewer.Models;
using OneWare.Vcd.Viewer.ViewModels;
using OneWare.WaveFormViewer.ViewModels;
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
    private NetlistService _netlistService;
    private VcdService? _vcdService;

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
        containerRegistry.RegisterSingleton<IVcdService, VcdService>();
        containerRegistry.Register<IWaveformInteractorService, WaveformInteractorService>();
        containerRegistry.Register<WaveformInteractorViewModel>();
        // containerRegistry.Register<IFrontendService, FrontendService>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        _containerProvider = containerProvider;
        _verilatorService = containerProvider.Resolve<IVerilatorService>();
        _signalBitIndexService = containerProvider.Resolve<SignalBitIndexService>();
        _waveformInteractorService = containerProvider.Resolve<IWaveformInteractorService>();
        _netlistService = containerProvider.Resolve<NetlistService>();
        _vcdService = containerProvider.Resolve<VcdService>();
        
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
                Command = new AsyncRelayCommand(_verilatorService.CreateVerilatorBinaryAllStepsAsync),
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
                    menuItems.Add(new MenuItemViewModel("Unset Verilator testbench")
                    {
                        Header = "Unset Verilator testbench",
                        Command = new RelayCommand(() => _verilatorService.UnregisterTestbench(cppFile)),
                        IconObservable =
                            Application.Current!.GetResourceObservable("VSImageLib.RemoveSingleDriverTest_16x")
                    });
            }

            if (selected is [IProjectFile { Extension: ".vcd" } vcdFile])
            {
                menuItems.Add(new MenuItemViewModel("Recreate Hierarchy from Signalnames")
                {
                    Header = "Recreate Hierarchy from Signalnames",
                    Command = new RelayCommand(() => recreateHirAndWriteVcd(vcdFile))
                });
            }
        });

        dockService.PropertyChanged += (o, args) =>
        {
            if (args.PropertyName != nameof(dockService.CurrentDocument)) return;
            var currentDocument = dockService.CurrentDocument;
            
            if (currentDocument is VcdViewModel vcdViewModel)
            {
                vcdViewModel.PropertyChanged -= VcdViewModel_PropertyChanged;
                vcdViewModel.PropertyChanged += VcdViewModel_PropertyChanged;

                // vcdViewModel.WaveFormViewer.PropertyChanged -= WaveFormViewer_PropertyChanged;
                vcdViewModel.WaveFormViewer.PropertyChanged += (sender, args) => WaveFormViewer_PropertyChanged(sender, args, vcdViewModel);            }
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
        //      _netlistService.BackendAddress = x;
        // });
        //
        // _settingsService.GetSettingObservable<string>("NetlistViewer_Backend_Port").Subscribe(x =>
        // {
        //     _netlistService.BackendPort = x;
        // });
    }
    
    void VcdViewModel_PropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        var vcdViewModel = sender as VcdViewModel;
        if (vcdViewModel == null) return;

        if (args.PropertyName == nameof(vcdViewModel.IsLoading) && !vcdViewModel.IsLoading)
        {
            _ = HandleIsLoadingChangedAsync(vcdViewModel);
        }
    }
    
    void WaveFormViewer_PropertyChanged(object? sender, PropertyChangedEventArgs args, VcdViewModel vcdViewModel)    
    {
        var waveformViewModel = sender as WaveFormViewModel;
        if (waveformViewModel == null) return;

        if (args.PropertyName == nameof(waveformViewModel.SelectedSignal))
        {
            var hash = _vcdService.LoadVcdAndHashBody(vcdViewModel.FullPath);
            
            var selectedWaveform = waveformViewModel.SelectedSignal;
            if (selectedWaveform != null)
            {
                var bits = _signalBitIndexService.GetMapping(hash, selectedWaveform.Signal.Id);
                _waveformInteractorService.GoToSignal(bits.BitIndexId);
            }
        }
    }

    private void recreateHirAndWriteVcd(IProjectFile vcdFile)
    {
        var waveformpath = vcdFile.FullPath;
        string waveformpathRecreatedHir = Path.Combine(vcdFile.TopFolder.FullPath, Path.GetFileNameWithoutExtension(vcdFile.FullPath) + "_recreated.vcd" );
        _vcdService.Reset();
        _vcdService.LoadVcd(waveformpath);
        _vcdService.RecreateVcdHierarchy();
        _vcdService.WriteVcd(waveformpathRecreatedHir);    
    }

    private async Task HandleIsLoadingChangedAsync(VcdViewModel vcdViewModel)
    {
        try
        {   
            // first read in bitMapping.json
            var projectPath = _projectExplorerService.ActiveProject?.FullPath;
            var jsonPath = Path.Combine(projectPath, "build", "simulation", "bitmapping.json");
            _signalBitIndexService.LoadFromJsonFile(jsonPath);
                
            // first check if the vcd file has been read before, by hashing the body.
            var vcdBodyHash = _vcdService.LoadVcdAndHashBody(vcdViewModel.FullPath);
            
            // check if mapping was already done before
            if (_signalBitIndexService.GetMapping(vcdBodyHash) != null)
            {
                return;
            }

            _vcdService.Reset();
            
            //If .vcd has not been loaded before, load it via backend
            // ensure that backend is running
            var frontendService = _containerProvider.Resolve<IFrontendService>();
            _ = frontendService.StartBackendIfNotStartedAsync();

            // Get current scopes
            _netlistService.OneWareScopes = vcdViewModel.Scopes;
            
            var projectRoot = _projectExplorerService.ActiveProject.Root as UniversalFpgaProjectRoot;
            var topEntity = Path.GetFileNameWithoutExtension(projectRoot.TopEntity.FullPath);
            var netlistPath = Path.Combine(_projectExplorerService.ActiveProject.RootFolderPath, "build", "netlist", $"{topEntity}.json");
            
            // post netlist to backend
            // dont post if netlist already present in backend?
            var netInfo = await _netlistService.GetNetInformationAsync(netlistPath);
            if (!netInfo.HasValues)
            {
                await _netlistService.PostNetlistToBackendAsync(netlistPath);
                netInfo = await _netlistService.GetNetInformationAsync(netlistPath);
            }
            
            _netlistService.ParseNetInfoToBitMapping(netInfo, vcdBodyHash);
        }
        catch (Exception ex)
        {
            _logger.Error($"Error in HandleIsLoadingChangedAsync: {ex}");
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
}