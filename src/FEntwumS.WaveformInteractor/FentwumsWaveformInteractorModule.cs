using System.Collections.ObjectModel;
using System.Net.Mime;
using System.Text;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Markup.Xaml.Styling;
using CommunityToolkit.Mvvm.Input;
using FEntwumS.WaveformInteractor;
using FEntwumS.WaveformInteractor.Services;
using FEntwumS.Common;
using FEntwumS.Common.Services;
using ImTools;
using Newtonsoft.Json.Linq;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.ProjectSystem.Models;
using OneWare.UniversalFpgaProjectSystem.Models;
using OneWare.Vcd.Parser.Data;
using OneWare.Vcd.Viewer.Models;
using OneWare.Vcd.Viewer.ViewModels;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Fentwums.WaveformInteractor;

public class FentwumsWaveformInteractorModule : IModule
{
    // private HttpServer _httpServer;
    private HttpClient _httpClient;
    private readonly IWaveformInteractorService _waveformInteractorService;
    private IYosysService _yosysSimService;
    private IVerilatorService _verilatorService;
    
    private ObservableCollection<ExtendedVcdScopeModel> _fentwumsScopes;
    
    // holds all Signals
    private VcdDefinition rootScope;
    
    private Application _resourceDictionary;
    
    
    public FentwumsWaveformInteractorModule(IWaveformInteractorService waveformInteractorService)
    {
        _waveformInteractorService = waveformInteractorService;
    }
    
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // containerRegistry.Register<IWaveformService, WaveformService>();
        containerRegistry.RegisterSingleton<IYosysService, YosysSimService>();
        containerRegistry.RegisterSingleton<IVerilatorService, VerilatorService>();
        containerRegistry.Register<IWaveformInteractorService, WaveformInteractorService>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        _yosysSimService = containerProvider.Resolve<IYosysService>(); // Resolve with containerProvider
        _verilatorService = containerProvider.Resolve<IVerilatorService>();
        var dockService = containerProvider.Resolve<IDockService>();
        IProjectExplorerService projectExplorerService = containerProvider.Resolve<IProjectExplorerService>();

        projectExplorerService.RegisterConstructContextMenu((selected, menuItems) =>
            {
                if (selected is [IProjectFile { Extension: ".v" } verilogFile])
                {
                        menuItems.Add(new MenuItemViewModel("Create flattened verilog")
                        {
                            Header = $"Create flattened verilog file",
                            Command = new AsyncRelayCommand(() => _yosysSimService.LoadVerilogAsync(verilogFile))
                        });
                        
                        menuItems.Add(new MenuItemViewModel("Verilate")
                        {
                            Header = $"Verilate this file{verilogFile.Header}",
                            Command = new AsyncRelayCommand(() => _verilatorService.VerilateAsync(verilogFile))
                        });
                }

                if (selected is [IProjectFile { Extension: ".cpp" } cppFile])
                {
                    var filepath = cppFile.FullPath; 
                    var testbenchpath = _verilatorService.GetTestbench();
                    if (String.Equals(filepath, testbenchpath) == false)
                    {
                        menuItems.Add(new MenuItemViewModel("Set as Verilator testbench")
                        {
                            Header = "Set as Verilator testbench",
                            Command = new RelayCommand(() => _verilatorService.SetTestbench(cppFile.Header)),
                            IconObservable = Application.Current!.GetResourceObservable("VSImageLib.AddTest_16x")
                        });
                    }
                    else
                    {   
                        // TODO: change icon, if testbench is set.
                        menuItems.Add(new MenuItemViewModel("Unset Verilator testbench")
                        {
                            Header = "Unset Verilator testbench",
                            Command = new RelayCommand(() => _verilatorService.SetTestbench("")),
                            IconObservable = Application.Current!.GetResourceObservable("VSImageLib.AddTest_16x")
                        });
                    }
                    
                    UniversalFpgaProjectRoot root = cppFile.Root as UniversalFpgaProjectRoot;
                    // ProjectFile toplevel = root.TopEntity.;
                    menuItems.Add(new MenuItemViewModel("Build Verilator executable")
                    {
                        Header = $"Build Verilator executable from: {cppFile.Header}",
                        Command = new RelayCommand(() => _verilatorService.CompileVerilatedAsync(cppFile)), // TODO: Toplevel of project should be put in here!
                    });
                }
            });
        
        dockService.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName != nameof(dockService.CurrentDocument)) return;
            var currentDocument = dockService.CurrentDocument;

            // Check if the current document is a VcdViewModel
            if (currentDocument is VcdViewModel vcdViewModel)
            {

                vcdViewModel.PropertyChanged += (innerSender, innerArgs) =>
                {
                    switch (innerArgs.PropertyName)
                    {
                        // Subscribe to PropertyChanged for IsLoading
                        case nameof(vcdViewModel.IsLoading):
                            // spin up http server
                            // string[] prefixes = { "http://localhost:6969/" }; // Specify the URL prefixes
                            // _httpServer = new HttpServer(prefixes);
                            _httpClient = new HttpClient();
                            _ = InitializeHttpAsync();

                            // Copy vcdScopeModel to extended implementation of vcdScopeModel
                            ObservableCollection<VcdScopeModel> oneWareScopes = vcdViewModel.Scopes;
                            _fentwumsScopes = new ObservableCollection<ExtendedVcdScopeModel>();
                            rootScope = new VcdDefinition();

                            foreach (var scopeModel in oneWareScopes)
                            {
                                VcdScope scope = CreateVcdScopeFromModel(scopeModel, rootScope);
                                ExtendedVcdScopeModel extendedScopeModel = new ExtendedVcdScopeModel(scope);
                                _fentwumsScopes.Add(extendedScopeModel);
                            }
                    
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
                            string selectedSignalName = vcdViewModel.SelectedSignal?.Name;
                            // TODO: How to map from WaveformViewer to Bit Index?
                            // currently use signalname to get bit index
                            int bitIndex = 0;                       
                            _waveformInteractorService.GoToSignal(bitIndex);
                            break;
                    }

                };

                    
            }
        };
        
    }
    
    private async Task InitializeHttpAsync()
    {
        // await _httpServer.StartAsync();
    }
    
    private VcdScope CreateVcdScopeFromModel(VcdScopeModel scopeModel, IScopeHolder parentScope)
    {
        // Create a new VcdScope based on information from VcdScopeModel
        VcdScope scope = new VcdScope(parentScope, scopeModel.Name); // Parent is null for simplicity, can be adjusted
        scope.Signals.AddRange(scopeModel.Signals);
        
        // Recursively create subscopes and add them to the current scope
        foreach (var subScopeModel in scopeModel.Scopes)
        {
            // Pass the current scope as the parent for the subscopes
            VcdScope subScope = CreateVcdScopeFromModel(subScopeModel, scope);
            scope.Scopes.Add(subScope); // Add the subscope to the current scope's list of subscopes
        }

        return scope;
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
            Console.WriteLine("File not found.");
            return null;
        }

        string fileContent = await File.ReadAllTextAsync(netlistPath);
        
        ReadOnlySpan<byte> contentByteSpan = new ReadOnlySpan<Byte>(Encoding.UTF8.GetBytes(fileContent));
        ReadOnlySpan<byte> pathByteSpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(netlistPath));
                    
        UInt32 hashPath = Util.ComputeOneAtATimeHash(pathByteSpan);
        UInt32 hashContent = Util.ComputeOneAtATimeHash(contentByteSpan);
                    
        UInt64 netHash = ((UInt64)hashPath << 32) | hashContent;
        
        Console.WriteLine("Requesting Backend to populate Signals with bitindeces and hdlname/scope");
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
            Console.WriteLine($"Request to {url} failed. Error: {ex.Message}");
            return null;
        }
    }
    
    public void ParseNetInformation(JObject netInfo)
    {
        // write to .json
        string jsonString = netInfo.ToString();
        File.WriteAllText("/home/jonas/tin/fentwums/uart-verilog/yosys_verilog/netinfo.json", jsonString);
        
        // Check if the "signals" key exists
        if (!netInfo.TryGetValue("signals", out JToken signalsToken) || signalsToken is not JObject signalsObject)
        {
            Console.WriteLine("The provided netInfo does not contain valid 'signals' data.");
            return;
        }
        // Iterate over signals and search corresponding signals from Json
        foreach (var scopeModel in _fentwumsScopes)
        {
            SearchSignalsInScope(scopeModel, signalsObject);
        }
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
                    Console.WriteLine($"Signal {signal.OriginalSignal.Name} does not have valid details.");
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
                    Console.WriteLine($"Signal {signal.OriginalSignal.Name} does not have valid 'bits' data.");
                }
            }
            else
            {
                Console.WriteLine($"Signal {signal.OriginalSignal.Name} not found in JObject.");
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

