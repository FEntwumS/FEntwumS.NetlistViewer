using System.Collections.ObjectModel;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using FEntwumS.WaveformInteractor;
using FEntwumS.WaveformInteractor.Services;
using FEntwumS.Common;
using FEntwumS.Common.Services;
using Fentwums.WaveformInteractor.Services;
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

namespace Fentwums.WaveformInteractor;

public class FentwumsWaveformInteractorModule : IModule
{
    private HttpClient? _httpClient;
    private readonly IWaveformInteractorService _waveformInteractorService;
    private IYosysService? _yosysSimService;
    private IVerilatorService? _verilatorService;
    private IProjectExplorerService? _projectExplorerService;
    private IWindowService? _windowService;
    private ILogger? _logger;
    private SignalBitIndexService? _signalBitIndexService;

    private ObservableCollection<VcdScopeModel> _oneWareScopes;
    private ObservableCollection<ExtendedVcdScopeModel>? _fentwumsScopes;
    
    // holds all Signals
    private VcdDefinition? rootScope;
    
    public FentwumsWaveformInteractorModule(IWaveformInteractorService waveformInteractorService)
    {
        _waveformInteractorService = waveformInteractorService;
    }
    
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IYosysService, YosysSimService>();
        containerRegistry.RegisterSingleton<IVerilatorService, VerilatorService>();
        containerRegistry.RegisterSingleton<SignalBitIndexService>();
        containerRegistry.Register<IWaveformInteractorService, WaveformInteractorService>();
        containerRegistry.Register<WaveformInteractorViewModel>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        _yosysSimService = containerProvider.Resolve<IYosysService>(); // Resolve with containerProvider
        _verilatorService = containerProvider.Resolve<IVerilatorService>();
        _signalBitIndexService = containerProvider.Resolve<SignalBitIndexService>();
        
        // OneWare Services
        var dockService = containerProvider.Resolve<IDockService>();
        _projectExplorerService = containerProvider.Resolve<IProjectExplorerService>();
        _windowService = containerProvider.Resolve<IWindowService>();
        _logger = containerProvider.Resolve<ILogger>();
        
        /*
        // TODO: Extension does not show
        var resourceInclude = new ResourceInclude(new Uri("avares://FEntwumS.WaveformInteractor/Styles/Icons.axaml")) 
            {Source = new Uri("avares://FEntwumS.WaveformInteractor/Styles/Icons.axaml")};
        Application.Current?.Resources.MergedDictionaries.Add(resourceInclude);
        var resources = Application.Current!.Resources;
        var icon = Application.Current!.GetResourceObservable("VSImageLib.AddTest_16x");
        WaveformInteractorViewModel vm = containerProvider.Resolve<WaveformInteractorViewModel>();
        vm.InitializeContent();
        
        windowService.RegisterUiExtension("MainWindow_Fentwums",
            new UiExtension(x => new WaveformInteractorView
            {
                DataContext =  vm
            }));
        */
        
        // for now register Menu which handles functionality
        _windowService.RegisterMenuItem("MainWindow_MainMenu/FEntwumS",
            new MenuItemViewModel("Create_Verilator_Binary")
            {
                Header = "Create Verilator Binary",
                Command = new AsyncRelayCommand(CreateVerilatorBinaryAllSteps),
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
                    
                    if (String.Equals(filepath, testbenchpath) == false)
                    {
                        menuItems.Add(new MenuItemViewModel("Set as Verilator testbench")
                        {
                            Header = "Set as Verilator testbench",
                            Command = new RelayCommand(() => _verilatorService.RegisterTestbench(cppFile)),
                            IconObservable = Application.Current!.GetResourceObservable("VSImageLib.AddTest_16x")
                        });
                    }
                    else
                    {   
                        // TODO: change icon, if testbench is set.
                        menuItems.Add(new MenuItemViewModel("Unset Verilator testbench")
                        {
                            Header = "Unset Verilator testbench",
                            Command = new RelayCommand(() => _verilatorService.UnregisterTestbench(cppFile)),
                            IconObservable = Application.Current!.GetResourceObservable("VSImageLib.RemoveSingleDriverTest_16x"),
                        });
                    }
                }
            });
        
        dockService.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName != nameof(dockService.CurrentDocument)) return;
            var currentDocument = dockService.CurrentDocument;

            if (currentDocument is VcdViewModel vcdViewModel)
            {
                vcdViewModel.PropertyChanged += (_, innerArgs) =>
                {
                    switch (innerArgs.PropertyName)
                    {
                        case nameof(vcdViewModel.IsLoading):
                            _httpClient = new HttpClient();
                            
                            // get current scopes
                            _oneWareScopes = vcdViewModel.Scopes;
                            
                            
                            //
                            //
                            // _fentwumsScopes = new ObservableCollection<ExtendedVcdScopeModel>();
                            // rootScope = new VcdDefinition();
                            //
                            // foreach (var scopeModel in oneWareScopes)
                            // {
                            //     VcdScope? scope = CreateVcdScopeFromModel(scopeModel, rootScope);
                            //     ExtendedVcdScopeModel extendedScopeModel = new ExtendedVcdScopeModel(scope);
                            //     _fentwumsScopes.Add(extendedScopeModel);
                            // }
                            
                            // Get Netlist information from backend and populate bitindeces dictionary
                            IProjectExplorerService projectExplorerService = containerProvider.Resolve<IProjectExplorerService>();
                            string netlistPath =  projectExplorerService.ActiveProject.RootFolderPath + "/build/netlist/netlist.json";
                    
                            Task.Run(async () =>
                            {
                                JObject netInfo = await GetNetInformationAsync(netlistPath);
                                ParseNetInformation(netInfo);
                            });
                            
                            break;
                        // Subscribe to PropertyChanged for SelectedSignal in WaveformViewer
                        case nameof(vcdViewModel.WaveFormViewer.SelectedSignal):
                            var selectedSignal = vcdViewModel.SelectedSignal;
                            // TODO: How to map from WaveformViewer to Bit Index?
                            // currently use signalname to get bit index
                            // OneWare assigns an Id to signal. This can be used to map from fentwums Signal to OneWare Signal without using the signalname
                            // -> Signalname still have to be used initially, because otherwise no way to map from netlist to vcd

                            // ExtendedVcdScopeModel.ExtendedSignal s = _fentwumsScopes
                            //     .SelectMany(scope => scope.Signals)
                            //     .FirstOrDefault(signal => signal.Id == selectedSignal.Id));

                            
                            // jump to selected Signal via bit index
                            BitIndexEntry bits = _signalBitIndexService.GetMapping(selectedSignal.Id);
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
                UniversalFpgaProjectRoot? project = _projectExplorerService.ActiveProject?.Root as UniversalFpgaProjectRoot;
                _verilatorService.RegisterTestbench(project?.TestBenches.FirstOrDefault());
            }
        };
        

    }
    
    // executes compiled verilator binary  
    private async Task RunVerilatorExecutableFromToplevelAsync()
    {
        UniversalFpgaProjectRoot projectRoot = _projectExplorerService.ActiveProject.Root as UniversalFpgaProjectRoot;
        var path = projectRoot.TopEntity.FullPath;
        var topFile = projectRoot.Files.FirstOrDefault(file => file.FullPath == path);
        
        await _verilatorService.RunExecutableAsync(topFile);
    }

    // requires verilator testbench, and toplevel entity to be set.
    private async Task CreateVerilatorBinaryAllSteps()
    {
        UniversalFpgaProjectRoot projectRoot = _projectExplorerService.ActiveProject.Root as UniversalFpgaProjectRoot;
        var path = projectRoot.TopEntity.FullPath;
        var topFile = projectRoot.Files.FirstOrDefault( file => file.FullPath == path);
        var verilatorServiceTestbench = _verilatorService.Testbench;

        if (topFile != null && verilatorServiceTestbench != null)
        {
            await _yosysSimService.LoadVerilogAsync(topFile);
            await _verilatorService.VerilateAsync(topFile);
            await _verilatorService.CompileVerilatedAsync(topFile);
        }
        else
        {
            if(topFile == null && verilatorServiceTestbench != null)
                _logger.Error("Toplevel Entity must be set!", null, true, true);
            if(topFile != null && verilatorServiceTestbench == null)
                _logger.Error("Verilator Testbench must be set!", null, true, true);
            if(topFile == null && verilatorServiceTestbench == null)
                _logger.Error("Toplevel Entity and Verilator Testbench must be set!", null, true, true);
        }
    }

    private VcdScope? CreateVcdScopeFromModel(VcdScopeModel scopeModel, IScopeHolder? parentScope)
    {
        VcdScope? scope = new VcdScope(parentScope, scopeModel.Name);
        scope.Signals.AddRange(scopeModel.Signals);
        
        // Recursively create subscopes and add them to the current scope
        foreach (var subScopeModel in scopeModel.Scopes)
        {
            VcdScope? subScope = CreateVcdScopeFromModel(subScopeModel, scope);
            scope.Scopes.Add(subScope);
        }

        return scope;
    }
    
    private VcdScope? CreateExtendedVcdScopeFromModel(VcdScopeModel scopeModel, IScopeHolder? parentScope)
    {
        VcdScope? scope = new VcdScope(parentScope, scopeModel.Name);
        scope.Signals.AddRange(scopeModel.Signals);
        
        // Recursively create subscopes and add them to the current scope
        foreach (var subScopeModel in scopeModel.Scopes)
        {
            VcdScope? subScope = CreateVcdScopeFromModel(subScopeModel, scope);
            scope.Scopes.Add(subScope);
        }

        return scope;
    }

    private void PopulateSignalBitMappingRecursive(JObject signalsObject, IEnumerable<VcdScopeModel> scopeModels)
    {
        foreach (var scope in scopeModels)
        {
            PopulateSignalBitMappingRecursive(signalsObject, scope.Scopes);
            foreach (var signal in scope.Signals)
            {
                if (signalsObject.TryGetValue(signal.Name, out var signalToken) &&
                    signalToken is JObject signalDetails)
                {
                    string scopeName = signalDetails.GetValue("scope")?.ToString() ?? "Unknown";
                    JToken bitsToken = signalDetails.GetValue("bits");
                
                    if (bitsToken is JArray bitsArray)
                    {
                        List<int> bits = bitsArray.Select(bit => bit.ToObject<int>()).ToList();
                        string vcdId = signal.Id;
                    
                        _signalBitIndexService?.AddMapping(vcdId, bits);
                    }
                    else
                    {
                        _logger.Error($"Signal {signal.Name} does not have valid 'bits' data.");
                    }
                }
                else
                {
                    _logger.Error($"Signal {signal.Name} not found in JObject.");
                }
            }
        }
    }
    
    // retrieves information about the netlist form the backend
    private async Task<JObject> GetNetInformationAsync(string netlistPath)
    {
        // get Bit indices and hdlname from backend

        // netlist hash is uint64, generated from oneAtATime(absolutepathtonetlist) 32 MSBs OneAtATime(netlistcontent) 32 LSBs
        // netlist path is always in ${ONEWAREPROJECTROOT}/build/netlist/<toplevelEntity>.json
        // compute hash of netlist
        if (!File.Exists(netlistPath))
        {
            _logger.Error("File not found.");
            return null;
        }

        string fileContent = await File.ReadAllTextAsync(netlistPath);
        
        ReadOnlySpan<byte> contentByteSpan = new ReadOnlySpan<Byte>(Encoding.UTF8.GetBytes(fileContent));
        ReadOnlySpan<byte> pathByteSpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(netlistPath));
                    
        UInt32 hashPath = Util.ComputeOneAtATimeHash(pathByteSpan);
        UInt32 hashContent = Util.ComputeOneAtATimeHash(contentByteSpan);
                    
        UInt64 netHash = ((UInt64)hashPath << 32) | hashContent;
        
        _logger.Warning("Requesting Backend to populate Signals with bitindeces and hdlname/scope");
        string url = "http://localhost:8080/get-net-information";
        
        try
        {
            // Create MultipartFormDataContent for form data
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(netHash.ToString()), "hash");
            
            // Send POST request
            var response = await _httpClient.PostAsync(url, formData).ConfigureAwait(false);            
            response.EnsureSuccessStatusCode();

            // Read the response content as a string
            string responseString = await response.Content.ReadAsStringAsync();

            // Parse the string content into a JObject
            JObject jsonResponse = JObject.Parse(responseString);

            return jsonResponse;
        }
        catch (Exception ex)
        {
            _logger.Error($"Request to {url} failed. Error: {ex.Message}");
            return null;
        }
    }

    private void ParseNetInformation(JObject netInfo)
    {
        // write to .json
        string jsonString = netInfo.ToString();
        File.WriteAllText("/home/jonas/tin/fentwums/uart-verilog/yosys_verilog/netinfo.json", jsonString);
        
        // Check if the "signals" key exists
        if (!netInfo.TryGetValue("signals", out JToken signalsToken) || signalsToken is not JObject signalsObject)
        {
            _logger.Error("The provided netInfo does not contain valid 'signals' data.");
            return;
        }
        PopulateSignalBitMappingRecursive(signalsObject, _oneWareScopes.OfType<VcdScopeModel>());
    }

    private void SearchSignalsInScope(ExtendedVcdScopeModel scope, JObject signalsObject)
    {
        // Iterate over all the signals in the scope
        foreach (var signal in scope.ExtendedSignals)
        {
            // Search for the signal in the JObject
            if (signalsObject.ContainsKey(signal.OriginalSignal.Name))
            {
                JObject? signalDetails = signalsObject[signal.OriginalSignal.Name] as JObject;

                if (signalDetails == null)
                {
                    _logger.Error($"Signal {signal.OriginalSignal.Name} does not have valid details.");
                    continue;
                }
                
                // at first only print out scopeName associated with signal, without reconstructing the former scope hierarchy
                string scopeName = signalDetails.GetValue("scope")?.ToString() ?? "Unknown";

                // write bit indices to signal as property
                JToken bitsToken = signalDetails.GetValue("bits");
                if (bitsToken is JArray bitsArray)
                {
                    List<int> bits = bitsArray.Select(bit => bit.ToObject<int>()).ToList();
                    // use first bit index to identify the signal
                    int signalId = bits.FirstOrDefault(); 
                    signal.BitIndexId = signalId;
                    signal.BitIndices = bits;
                }
                else
                {
                    _logger.Error($"Signal {signal.OriginalSignal.Name} does not have valid 'bits' data.");
                }
            }
            else
            {
                _logger.Error($"Signal {signal.OriginalSignal.Name} not found in JObject.");
            }
        }

        // Recursively search through subscopes - unnecessary for flattened netlists, since only one layer exists.
        foreach (var vcdScopeModel in scope.Scopes)
        {
            var subScope = (ExtendedVcdScopeModel)vcdScopeModel;
            SearchSignalsInScope(subScope, signalsObject); // Recurse into subscopes
        }
    }
    
    private void SearchSignalsInScope(ExtendedVcdScopeModel scope, String signalName)
    {
        // Iterate over all the signals in the scope and check its names.
        foreach (var signal in scope.ExtendedSignals)
        {
            //TODO: implement searching
        }

        // Recursively search through subscopes - unnecessary for flattened netlists, since only one layer exists.
        foreach (var vcdScopeModel in scope.Scopes)
        {
            var subScope = (ExtendedVcdScopeModel)vcdScopeModel;
            SearchSignalsInScope(subScope, signalName); // Recurse into subscopes
        }
    }
    
    public ExtendedVcdScopeModel.ExtendedSignal GetSignalByName(String signalName)
    {
        // ExtendedVcdScopeModel scope = SearchSignalsInScope(rootScope, signalName);
        return null;
    }
}

