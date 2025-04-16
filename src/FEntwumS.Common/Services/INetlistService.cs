using FEntwumS.Common.Types;
using Newtonsoft.Json.Linq;

namespace FEntwumS.Common.Services;

public interface INetlistService
{
    Task PostNetlistToBackendAsync(string jsonpath);
    Task<JObject> GetNetInformationAsync(string netlistPath);
    void ParseNetInfoToBitMapping(JObject netInfo, string vcdBodyHash);
    void PopulateSignalBitMappingRecursive(JObject signalsObject, List<VcdScope?> scopes, string vcdBodyHash);
}