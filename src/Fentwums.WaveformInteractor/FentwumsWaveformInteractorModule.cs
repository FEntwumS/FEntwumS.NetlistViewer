using System.Collections.ObjectModel;
using FEntwumS.WfInteractor.Common;
using Newtonsoft.Json.Linq;
using OneWare.Essentials.Services;
using OneWare.Vcd.Parser.Data;
using OneWare.Vcd.Viewer.Models;
using OneWare.Vcd.Viewer.ViewModels;
using Prism.Ioc;
using Prism.Modularity;

namespace FEntwumS.WfInteractor;

public class FentwumsWaveformInteractorModule : IModule
{
    private HttpServer _httpServer;
    private HttpClient _httpClient;

    private ObservableCollection<ExtendedVcdScopeModel> _fentwumsScopes;
    
    // holds all Signals
    private VcdDefinition rootScope;
    
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Console.WriteLine("Register Service in from Plugin");
        // containerRegistry.Register<IWaveformService, WaveformService>();
        containerRegistry.Register<Common.IWaveformInteractorService, WaveformInteractorService>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        var dockService = containerProvider.Resolve<IDockService>();
        dockService.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName != nameof(dockService.CurrentDocument)) return;
            var currentDocument = dockService.CurrentDocument;

            // Check if the current document is a VcdViewModel
            if (currentDocument is VcdViewModel vcdViewModel)
            {
                // Subscribe to PropertyChanged for IsLoading
                vcdViewModel.PropertyChanged += (innerSender, innerArgs) =>
                {
                    if (innerArgs.PropertyName != nameof(vcdViewModel.IsLoading) || vcdViewModel.IsLoading) return;
                    // spin up http server
                    string[] prefixes = { "http://localhost:6969/" }; // Specify the URL prefixes
                    _httpServer = new HttpServer(prefixes);
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
                        
                    // get Bit indices and hdlname from backend
                    IProjectManagerService projectManagerService = containerProvider.Resolve<IProjectManagerService>();
                    IProjectExplorerService projectExplorerService = containerProvider.Resolve<IProjectExplorerService>();
                    
                    // netlist hash is uint64, generated from oneAtATime(absolutepathtonetlist) 32 MSBs OneAtATime(netlistcontent) 32 LSBs
                    // netlist path is always in ${ONEWAREPROJECTROOT}/build/netlist/<toplevelEntity>.json
                    // compute hash of netlist
                    // TODO: RootFolderPath is only first open Project, not the Root of open .vcd file
                    // string netlistPath = projectExplorerService.ActiveProject.RootFolderPath + "build/netlist/netlists.json";
                    // byte[] fileContent = File.ReadAllBytes(netlistPath);
                    // UInt32 hashPath = Util.ComputeOneAtATimeHash(netlistPath);
                    // UInt32 hashContent = Util.ComputeOneAtATimeHash(fileContent);
                    //
                    // UInt64 netHash = ((UInt64)hashPath << 32) | hashContent;
                    
                    // get netlist information
                    // Task<JObject> respObj = GetNetInformation(netHash.ToString());
                    // for now just use hardcoded "1"
                    Task<JObject> respObj = GetNetInformationAsync("1");
                    JObject netInfo = respObj.Result;
                    ParseNetInformation(netInfo);
                    
                    // TODO: parse response and add bitindices to vcdScopeModel Signals
                    Console.WriteLine($"Loaded {_fentwumsScopes.Count} scopes");
                };

                    
            }
        };
        
    }
    
    private async Task InitializeHttpAsync()
    {
        await _httpServer.StartAsync();
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
    private async Task<JObject> GetNetInformationAsync(string netHash)
    {
        Console.WriteLine("Requesting Backend to populate Signals with bitindeces and hdlname/scope");
        string url = "http://localhost:8080/get-net-information";
        
        try
        {
            // Create MultipartFormDataContent for form data
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(netHash), "hash");

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
        // Check if the "signals" key exists
        if (!netInfo.TryGetValue("signals", out JToken signalsToken) || signalsToken is not JObject signalsObject)
        {
            Console.WriteLine("The provided netInfo does not contain valid 'signals' data.");
            return;
        }
        // Iterate over signals
        foreach (var signal in signalsObject)
        {
            string signalName = signal.Key;
            JObject signalDetails = signal.Value as JObject;

            if (signalDetails == null)
            {
                Console.WriteLine($"Signal {signalName} does not have valid details.");
                continue;
            }
            // Extract "scope"
            string scope = signalDetails.GetValue("scope")?.ToString() ?? "Unknown";

            // Extract "bits"
            JToken bitsToken = signalDetails.GetValue("bits");
            if (bitsToken is JArray bitsArray)
            {
                List<int> bits = bitsArray.Select(bit => bit.ToObject<int>()).ToList();

                // Output parsed data
                Console.WriteLine($"Signal: {signalName}");
                Console.WriteLine($"  Scope: {scope}");
                Console.WriteLine($"  Bits: {string.Join(", ", bits)}");
            }
            else
            {
                Console.WriteLine($"Signal {signalName} does not have valid 'bits' data.");
            }
        }
    }

}

