namespace FEntwumS.WaveformInteractor.Services;

public class SignalBitIndexService
{
    private readonly Dictionary<string, BitIndexEntry> _signalBitIndexMapping = new();

    public void AddMapping(string id, List<int> bitIndices)
    {
        if (bitIndices.Count == 0) return;
        _signalBitIndexMapping[id] = new BitIndexEntry { BitIndexId = bitIndices[0], BitIndices = bitIndices };
    }

    public BitIndexEntry? GetMapping(string id) =>
        _signalBitIndexMapping.TryGetValue(id, out var entry) ? entry : null;

    public bool RemoveMapping(string id) => _signalBitIndexMapping.Remove(id);
}

public class BitIndexEntry
{
    public int BitIndexId { get; set; }
    public List<int> BitIndices { get; set; } = new();
}