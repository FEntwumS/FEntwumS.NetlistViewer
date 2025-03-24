// using System.IO.Hashing;
using System.Text;
using FEntwumS.NetlistViewer.Services;

namespace FEntwumS.WaveformInteractor.Services;

public class VcdService : IVcdService
{
    private VcdScope? _rootScope = new("ROOT", null);
    private List<string> _bodyLines = new();
    private List<string> _headerLines = new();
    private int bodyStartIndex = 0;
    public string BodyHash;

    public VcdService()
    {
        BodyHash = new("");
    }

    public void Reset()
    {
        _rootScope = new VcdScope("ROOT", null);
        _bodyLines.Clear();
        _headerLines.Clear();
        bodyStartIndex = 0;
        BodyHash = new("");
    }
    
    // TODO: use FileStreaming to avoid saving the whole .vcd in Memory
    public void LoadVcd(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        _rootScope = ParseVcdHeader(lines);
        bodyStartIndex = GetBodyStartIndex(lines);
        _bodyLines = lines.Skip(bodyStartIndex).ToList();
        BodyHash = HashVcdBody(lines, bodyStartIndex);
    }

    public void WriteVcd(string filePath)
    {
        using var writer = new StreamWriter(filePath);
        // Write header lines
        foreach (var line in _headerLines)
        {
            writer.WriteLine(line);
        }

        // Write VCD hierarchy from data structure
        // Omit the "ROOT" scope, which just holds all scopes.
        List<VcdScope?> scopes = _rootScope.SubScopes;
        foreach (var scope in scopes)
        {
            WriteScopeRecursive(writer, scope);
        }

        // Write enddefinitions block
        writer.WriteLine("$enddefinitions $end");
            
        // Write body lines
        foreach (var line in _bodyLines)
        {
            writer.WriteLine(line);
        }
    }

    private void WriteScopeRecursive(StreamWriter writer, VcdScope? scope)
    {
        writer.WriteLine($"$scope module {scope.Name} $end");

        foreach (var signal in scope.Signals)
        {
            writer.WriteLine($"$var {signal.Type} {signal.BitWidth} {signal.Id} {signal.Name} $end");
        }

        foreach (var subScope in scope.SubScopes)
        {
            WriteScopeRecursive(writer, subScope);
        }

        writer.WriteLine("$upscope $end");
    }

    public void RecreateVcdHierarchy()
    {
        // the scope of the signal can be derived from signalname: scope;signal where ';' is the delimiter.
        // dual nested signals (in 2 scopes) are saved as: scope1;scope2;signal
        VcdScope? rootScope = IterateThroughSignalsofScopeRecursive(_rootScope);
        _rootScope = rootScope;
    }

    public VcdScope? IterateThroughSignalsofScopeRecursive(VcdScope? currentRootScope)
    {
        for (int i = 0; i < currentRootScope.SubScopes.Count; i++)
        {
            currentRootScope.SubScopes[i] = IterateThroughSignalsofScopeRecursive(currentRootScope.SubScopes[i]);
        }
        
        foreach (var signal in currentRootScope.Signals.ToList())
        {
            var parts = signal.Name.Split(';');
            if (parts.Length > 1)
            {
                // list from all but last
                var scopePath = parts[..^1];
                // get last part
                var signalName = parts[^1];
                var targetScope = currentRootScope;
                
                foreach (var scopeName in scopePath)
                {
                    // search if scope already exists
                    var existingScope = targetScope.SubScopes.FirstOrDefault(s => s.Name == scopeName);
                    // if not add new scope
                    if (existingScope == null)
                    {
                        var newScope = new VcdScope(scopeName, targetScope);
                        targetScope.SubScopes.Add(newScope);
                        targetScope = newScope;
                    }
                    else
                    {
                        targetScope = existingScope;
                    }
                }
                // add new signal to the correct scope
                // omit scopenames in signalname by using last only part of original signalname (e.g. "txInst;clk" -> "clk")
                targetScope.Signals.Add(new Signal(signal.Type, signal.BitWidth, signal.Id, signalName));
                currentRootScope.Signals.Remove(signal);
            }
        }
        return currentRootScope;
    }
    
    // parses .vcd header and definitions section
    // returns root VcdScope, which holds all subscopes and signals
    private VcdScope? ParseVcdHeader(string[] lines)
    {
        VcdScope? currentScope = _rootScope;
        bool headerParsed = false;
        
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            var parts = line.Split(' ');
            // add header lines, which are added later without alteration later on
            if (!headerParsed && (line.StartsWith("$scope") || line.StartsWith("$var")))
            {
                headerParsed = true;
            }
            else if(!headerParsed)
            {
                _headerLines.Add(line);
            }
            // parse definitions and save in datastructure
            if (headerParsed)
            {
                if (line.StartsWith("$scope"))
                {
                    VcdScope? newScope = new VcdScope(parts[2], currentScope); // pass name and parent
                    currentScope.SubScopes.Add(newScope);
                    currentScope = newScope; // traverse into new scope
                }
                else if (line.StartsWith("$var"))
                {
                    var newSignal = new Signal(
                        parts[1],  // Type (e.g., "wire")
                        int.Parse(parts[2]), // BitWidth (e.g., 12)
                        parts[3],  // IdentifierCode (e.g., "4")
                        parts[4]   // Name (e.g., "generatorInst;tx_counter")
                    ); 
                    currentScope.Signals.Add(newSignal);
                }
                else if (line.StartsWith("$upscope"))
                {
                    currentScope = currentScope.parent;
                }
                else if (line.StartsWith("$enddefinitions"))
                {
                    break;
                }
            }
        }
        return currentScope;
    }
    
    public int GetBodyStartIndex(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (line.StartsWith("$enddefinitions"))
            {
                return i + 1;
            }
        }
        return 0; // Default if $enddefinitions is not found
    }
    
    // using System.IO.Hashing
    // public byte[] HashVcdBody(string vcdPath, int bodyStartIndex)
    // {
    //     using var stream = File.OpenRead(vcdPath);
    //     using var reader = new StreamReader(stream);
    //
    //     // Skip lines up to bodyStartIndex
    //     for (int i = 0; i < bodyStartIndex; i++)
    //     {
    //         if (reader.ReadLine() == null) return []; // Stop if end of file is reached
    //     }
    //     
    //     var xxHash = new XxHash32();     
    //     byte[] buffer = new byte[8192];
    //     int bytesRead;
    //     while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
    //     {
    //         xxHash.Append(buffer.AsSpan(0, bytesRead));
    //     }
    //
    //     return xxHash.GetCurrentHash();
    // }

    public string HashVcdBody(string[] lines, int bodystartIndex)
    {
        string content = string.Join("\n", lines.Skip(bodystartIndex));        
        IHashService hashService = ServiceManager.GetHashService();
        ReadOnlySpan<byte> bodyByteSpan = new ReadOnlySpan<Byte>(Encoding.UTF8.GetBytes(content));
        uint bodyhash = hashService.ComputeHash(bodyByteSpan);

        return bodyhash.ToString("X8");
    }

    public string LoadVcdAndHashBody(string vcdPath)
    {
        var lines = File.ReadAllLines(vcdPath);
        int bodyStartIndex = GetBodyStartIndex(lines);
        var hash = HashVcdBody(lines, bodyStartIndex);
        return hash;
    }
}

// data structures for VCD parsing
public class VcdScope
{
    public string Name { get; set; }
    public List<VcdScope?> SubScopes { get; } = new();
    public List<Signal> Signals { get; } = new();
    public VcdScope? parent;

    public VcdScope(string name, VcdScope? parent)
    {
        Name = name;
        this.parent = parent;
    }
}

public class Signal
{
    public string Type { get; }
    public int BitWidth { get; }
    public string Id { get; }
    public string Name { get; }
    public string Value { get; private set; } = "0";

    public Signal(string type, int bitWidth, string id, string name)
    {
        Type = type;
        BitWidth = bitWidth;
        Id = id;
        Name = name;
    }
}