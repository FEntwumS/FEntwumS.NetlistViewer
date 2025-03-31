using System.Text.Json;

namespace FEntwumS.WaveformInteractor.Services;

public class SignalBitIndexService
{
    private readonly Dictionary<string, Dictionary<string, BitIndexEntry>> _signalBitIndexMapping = new();
    
    public void AddMapping(string hash, string id, List<int> bitIndices)
    {
        if (bitIndices.Count == 0) return;
        
        if (!_signalBitIndexMapping.ContainsKey(hash))
        {
            _signalBitIndexMapping[hash] = new Dictionary<string, BitIndexEntry>();
        }
        
        _signalBitIndexMapping[hash][id] = new BitIndexEntry { BitIndexId = bitIndices[0], BitIndices = bitIndices };
    }

    public BitIndexEntry? GetMapping(string hash, string id) =>
        _signalBitIndexMapping.TryGetValue(hash, out var innerDict) && innerDict.TryGetValue(id, out var entry) ? entry : null;

    public Dictionary<string, BitIndexEntry>? GetMapping(string hash) =>
        _signalBitIndexMapping.TryGetValue(hash, out var innerDict) ? innerDict : null;
    
    public bool RemoveMapping(string hash, string id)
    {
        if (!_signalBitIndexMapping.TryGetValue(hash, out var innerDict))
            return false;

        bool removed = innerDict.Remove(id);
        
        if (innerDict.Count == 0)
        {
            _signalBitIndexMapping.Remove(hash);
        }
        
        return removed;
    }
    
    public void SaveToJsonFile(string filename)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filename)!);
        File.WriteAllText(filename, JsonSerializer.Serialize(_signalBitIndexMapping, new JsonSerializerOptions { WriteIndented = true }));
    }

    public void LoadFromJsonFile(string filename)
    {
        if (File.Exists(filename))
        {
            string json = File.ReadAllText(filename);
            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, BitIndexEntry>>>(json);
            if (data != null)
            {
                _signalBitIndexMapping.Clear();
                foreach (var kvp in data)
                {
                    _signalBitIndexMapping[kvp.Key] = kvp.Value;
                }
            }
        }
    }
}

public class BitIndexEntry
{
    public int BitIndexId { get; set; }
    public List<int> BitIndices { get; set; } = new();
}