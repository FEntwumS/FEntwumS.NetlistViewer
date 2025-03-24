using Newtonsoft.Json.Linq;
using OneWare.Vcd.Viewer.Models;

namespace FEntwumS.WaveformInteractor.Services;

public interface INetlistService
{
    Task PostNetlistToBackendAsync(string jsonpath);
    Task<JObject> GetNetInformationAsync(string netlistPath);
    void ParseNetInfoToBitMapping(JObject netInfo, string vcdBodyHash);

    void PopulateSignalBitMappingRecursive(JObject signalsObject, IEnumerable<VcdScopeModel> scopeModels, string vcdBodyHash);
}