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
    
    private IContainerProvider _containerProvider; 

    private ObservableCollection<VcdScopeModel> _oneWareScopes;
    private IProjectExplorerService? _projectExplorerService;
    private SignalBitIndexService? _signalBitIndexService;
    private IVerilatorService? _verilatorService;
    private IWaveformInteractorService _waveformInteractorService;
    private IWindowService? _windowService;
    private IYosysService? _yosysSimService;

    // holds all Signals
    private VcdDefinition? rootScope;

    public FEntwumSWaveformInteractorModule(IWaveformInteractorService? waveformInteractorService = null)
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
        // containerRegistry.Register<IFrontendService, FrontendService>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        _containerProvider = containerProvider;
        _yosysSimService = containerProvider.Resolve<IYosysService>();
        _verilatorService = containerProvider.Resolve<IVerilatorService>();
        _signalBitIndexService = containerProvider.Resolve<SignalBitIndexService>();
        _waveformInteractorService = containerProvider.Resolve<IWaveformInteractorService>();
        // OneWare Services
        var dockService = containerProvider.Resolve<IDockService>();
        _projectExplorerService = containerProvider.Resolve<IProjectExplorerService>();
        _windowService = containerProvider.Resolve<IWindowService>();
        _logger = containerProvider.Resolve<ILogger>();

        // for now register Menu which handles functionality
        _windowService.RegisterMenuItem("MainWindow_MainMenu/FEntwumS",
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
    }
    
    private async Task HandleIsLoadingChangedAsync(VcdViewModel vcdViewModel)
    {
        try
        {
            // ensure that backend is running
            var frontendService = _containerProvider.Resolve<IFrontendService>();
            _ = frontendService.StartBackendIfNotStartedAsync();
            
            _httpClient = new HttpClient();

            // Get current scopes
            _oneWareScopes = vcdViewModel.Scopes;
            
            var projectRoot = _projectExplorerService.ActiveProject.Root as UniversalFpgaProjectRoot;
            var topEntity = Path.GetFileNameWithoutExtension(projectRoot.TopEntity.FullPath);
            var netlistPath = _projectExplorerService.ActiveProject.RootFolderPath + $"/build/netlist/{topEntity}.json";
            
            // post netlist to backend
            // TODO: dont post if netlist already present in backend?
            await PostNetlistToBackendAsync(netlistPath);
            
            var netInfo = await GetNetInformationAsync(netlistPath);
            ParseNetInformation(netInfo);
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

    private void PopulateSignalBitMappingRecursive(JObject signalsObject, IEnumerable<VcdScopeModel> scopeModels)
    {
        foreach (var scope in scopeModels)
        {
            PopulateSignalBitMappingRecursive(signalsObject, scope.Scopes);
            foreach (var signal in scope.Signals)
                if (signalsObject.TryGetValue(signal.Name, out var signalToken) &&
                    signalToken is JObject signalDetails)
                {
                    var scopeName = signalDetails.GetValue("scope")?.ToString() ?? "Unknown";
                    var bitsToken = signalDetails.GetValue("bits");

                    if (bitsToken is JArray bitsArray)
                    {
                        var bits = bitsArray.Select(bit => bit.ToObject<int>()).ToList();
                        var vcdId = signal.Id;

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

    public async Task PostNetlistToBackendAsync(string jsonpath)
    {
        HttpResponseMessage? resp = null;
        string top = Path.GetFileNameWithoutExtension(jsonpath);

        if (!File.Exists(jsonpath))
        {
            _logger.Error(
                "No json netlist found. Please generate netlist via rightclick on HDL file->Generate Json Netlist");
            return;
        }

        string content = await File.ReadAllTextAsync(jsonpath);

        IHashService hashService = ServiceManager.GetHashService();
        ReadOnlySpan<byte> contentByteSpan = new ReadOnlySpan<Byte>(Encoding.UTF8.GetBytes(content));
        ReadOnlySpan<byte> pathByteSpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(jsonpath));
        UInt32 pathHash = hashService.ComputeHash(pathByteSpan);
        UInt32 contenthash = hashService.ComputeHash(contentByteSpan);

        UInt64 combinedHash = ((UInt64)pathHash) << 32 | contenthash;

        ServiceManager.GetCustomLogger().Log("Path hash: " + pathHash);
        ServiceManager.GetCustomLogger().Log("Full file hash is: " + contenthash);
        ServiceManager.GetCustomLogger().Log("Combined hash is: " + combinedHash);

        await using FileStream jsonFileStream = File.Open(jsonpath, FileMode.Open, FileAccess.Read);

        MultipartFormDataContent formDataContent = new MultipartFormDataContent
        {
            { new StreamContent(jsonFileStream), "file", Path.GetFileName(jsonpath) }
        };

        HttpResponseMessage response = null;

        // backend healthcheck
        // timeout after 2 seconds. TODO: Adjust timeout values appropriately
        for (int i = 0; i < 10; i++)
        {
            try
            {
                response = await _httpClient.GetAsync("http://localhost:8080/actuator/health");
                if (response.IsSuccessStatusCode)
                {
                    _logger?.Log("Server is healthy and ready.", ConsoleColor.White);
                    break; // Exit the loop if the server is healthy
                }
                else 
                {
                    _logger?.Log($"Received non-success status code: {response.StatusCode}. Retrying...", ConsoleColor.White);
                }       
            }
            catch (Exception ex)
            {
                await Task.Delay(200);
                _logger?.Log($"Failed {i+1} time to reach server. Retrying... Exception: {ex.Message}", ConsoleColor.White);
                if(i == 9) return;
            }
        }

        // if healthcheck was successful
        string url = $"http://localhost:8080/graphRemoteFile?hash={combinedHash}";
        response = await _httpClient.PostAsync(url, formDataContent);
        
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Request failed: {response.StatusCode} {await response.Content.ReadAsStringAsync()}");
        }
    }
    
    // TODO: get URL and Port from settings
    // retrieves information about the netlist from the backend
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

        var fileContent = await File.ReadAllTextAsync(netlistPath);

        var contentByteSpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(fileContent));
        var pathByteSpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(netlistPath));

        var hashPath = Util.ComputeOneAtATimeHash(pathByteSpan);
        var hashContent = Util.ComputeOneAtATimeHash(contentByteSpan);

        var netHash = ((ulong)hashPath << 32) | hashContent;

        // _logger.Warning("Requesting Backend to populate Signals with bitindeces and hdlname/scope");
        var url = "http://localhost:8080/get-net-information";
        try
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(netHash.ToString()), "hash");

            var response = await _httpClient.PostAsync(url, formData).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var jsonResponse = JObject.Parse(responseString);

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

        // Write to .json file for debugging purposes
        // var jsonString = netInfo.ToString();
        // TODO: use /simulation directory for this
        // File.WriteAllText("/home/jonas/tin/fentwums/uart-verilog/yosys_verilog/netinfo.json", jsonString);

        // Check if the "signals" key exists
        if (!netInfo.TryGetValue("signals", out var signalsToken) || signalsToken is not JObject signalsObject)
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
            // Search for the signal in the JObject
            if (signalsObject.ContainsKey(signal.OriginalSignal.Name))
            {
                var signalDetails = signalsObject[signal.OriginalSignal.Name] as JObject;

                if (signalDetails == null)
                {
                    _logger.Error($"Signal {signal.OriginalSignal.Name} does not have valid details.");
                    continue;
                }

                // at first only print out scopeName associated with signal, without reconstructing the former scope hierarchy
                var scopeName = signalDetails.GetValue("scope")?.ToString() ?? "Unknown";

                // write bit indices to signal as property
                var bitsToken = signalDetails.GetValue("bits");
                if (bitsToken is JArray bitsArray)
                {
                    var bits = bitsArray.Select(bit => bit.ToObject<int>()).ToList();
                    // use first bit index to identify the signal
                    var signalId = bits.FirstOrDefault();
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

        // Recursively search through subscopes - unnecessary for flattened netlists, since only one layer exists.
        foreach (var vcdScopeModel in scope.Scopes)
        {
            var subScope = (ExtendedVcdScopeModel)vcdScopeModel;
            SearchSignalsInScope(subScope, signalsObject); // Recurse into subscopes
        }
    }
}