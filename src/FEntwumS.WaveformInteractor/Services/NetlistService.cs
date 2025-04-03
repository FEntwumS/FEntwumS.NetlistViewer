using System.Text;
using FEntwumS.Common.Services;
using Newtonsoft.Json.Linq;
using OneWare.Essentials.Services;
using Prism.Ioc;
using ILogger = OneWare.Essentials.Services.ILogger;


namespace FEntwumS.WaveformInteractor.Services;

public class NetlistService : INetlistService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger? _logger;
    
    public string BackendAddress { get; set; } = "http://localhost";
    public string BackendPort { get; set; } = ":8080";

    private SignalBitIndexService? _signalBitIndexService;
    private IProjectExplorerService _projectExplorerService;
    private IVcdService _vcdService;

    public NetlistService(IContainerProvider containerProvider)
    {
        _httpClient = new HttpClient();
        _logger = containerProvider.Resolve<ILogger>();
        _signalBitIndexService = containerProvider.Resolve<SignalBitIndexService>();
        _projectExplorerService = containerProvider.Resolve<IProjectExplorerService>();
        _vcdService = containerProvider.Resolve<IVcdService>();
    }
    
     public async Task PostNetlistToBackendAsync(string jsonpath)
    {
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

        HttpResponseMessage response;

        // backend healthcheck
        // timeout after 2 seconds. TODO: Adjust timeout values appropriately
        for (int i = 0; i < 10; i++)
        {
            try
            {
                response = await _httpClient.GetAsync($"{BackendAddress}{BackendPort}/actuator/health");
                if (response.IsSuccessStatusCode)
                {
                    _logger?.Log("Server is healthy and ready.", ConsoleColor.White, true);
                    break; // Exit loop if server is healthy
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
        string url = $"{BackendAddress}{BackendPort}/graphRemoteFile?hash={combinedHash}";
        response = await _httpClient.PostAsync(url, formDataContent);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.Log($"Request failed: {response.StatusCode} {await response.Content.ReadAsStringAsync()}", showOutput:true);
        }
    }
     
    // retrieves information about the netlist from the backend
    // returns empty json if netlist not present locally and on backend.
    public async Task<JObject> GetNetInformationAsync(string netlistPath)
    {
        // get Bit indices and hdlname from backend
        // netlist hash is uint64, generated from oneAtATime(absolutepathtonetlist) 32 MSBs OneAtATime(netlistcontent) 32 LSBs
        // netlist path is always in ${ONEWAREPROJECTROOT}/build/netlist/<toplevelEntity>.json
        // compute hash of netlist
        // TODO: prompt to create json netlist first!
        if (!File.Exists(netlistPath))
        {
            _logger.Error("Netlist file not found. Please Generate netlist via rightclick on HDL file->Generate Json Netlist, then reopen .vcd file");
        }

        var fileContent = await File.ReadAllTextAsync(netlistPath);

        var contentByteSpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(fileContent));
        var pathByteSpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(netlistPath));

        var hashPath = Util.ComputeOneAtATimeHash(pathByteSpan);
        var hashContent = Util.ComputeOneAtATimeHash(contentByteSpan);

        var netHash = ((ulong)hashPath << 32) | hashContent;

        _logger.Log("Get netlist information from backend to populate Signals with bit-indices and hdlname/scope", ConsoleColor.White, true);
        var url = $"{BackendAddress}{BackendPort}/get-net-information";
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
        }
        return new JObject();
    }
    
    public void ParseNetInfoToBitMapping(JObject netInfo, string vcdBodyHash)
    {
        // Check if the "signals" key exists
        if (!netInfo.TryGetValue("signals", out var signalsToken) || signalsToken is not JObject signalsObject)
        {
            _logger.Error("The provided netInfo does not contain valid 'signals' data.");
            return;
        }
        
        var scopes = new List<VcdScope>();
        scopes.Add(_vcdService.RootScope);
        PopulateSignalBitMappingRecursive(signalsObject, scopes, vcdBodyHash);
        // PopulateSignalBitMappingRecursive(signalsObject, OneWareScopes.OfType<VcdScopeModel>(), vcdBodyHash);
        
        var projectPath = _projectExplorerService.ActiveProject?.FullPath;
        var jsonPath = Path.Combine(projectPath, "build", "simulation", "bitmapping.json");
        _signalBitIndexService.SaveToJsonFile(jsonPath);
        _logger.Log("Successfully mapped bit indices to vcd identifiers.", ConsoleColor.White);
    }
    public void PopulateSignalBitMappingRecursive(JObject signalsObject, List <VcdScope>? scopes, string vcdBodyHash)
    {
        foreach (var scope in scopes)
        {
            PopulateSignalBitMappingRecursive(signalsObject, scope.SubScopes, vcdBodyHash);
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

                        _signalBitIndexService?.AddMapping(vcdBodyHash, vcdId, bits);
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