using System.IO.Hashing;
using System.Text;
using FEntwumS.Common.Services;
using FEntwumS.Common.Types;

namespace FEntwumS.WaveformInteractor.Services;

public class VcdService : IVcdService
{
    public VcdScope? RootScope { get; private set; } = new("ROOT", null);
    private int _definitionsStartIndex = 0;
    private int _bodyStartIndex = 0;

    public void Reset()
    {
        RootScope = new VcdScope("ROOT", null);
        _definitionsStartIndex = 0;
        _bodyStartIndex = 0;
    }


    public void LoadVcd(string filePath)
    {
        using var reader = new StreamReader(filePath);
        RootScope = ParseVcdDefinitions(reader);
    }

    public void WriteVcd(string inputFilePath, string outputFilePath)
    {
        using var reader = new StreamReader(inputFilePath);
        using var writer = new StreamWriter(outputFilePath){ NewLine = "\n" };
        
        // Write header lines up to _definitionsStartIndex
        for (int i = 0; i < _definitionsStartIndex; i++)
        {
            if (reader.ReadLine() is string line)
            {
                writer.WriteLine(line);
            }
            else
            {
                break;
            }
        }

        if (RootScope == null)
        {
            
        }

        // Write VCD hierarchy from data structure
        // Omit the "ROOT" scope, which just holds all scopes.
        List<VcdScope?> scopes = RootScope.SubScopes;
        foreach (var scope in scopes)
        {
            WriteScopeRecursive(writer, scope);
        }

        // Write enddefinitions block
        writer.WriteLine("$enddefinitions $end");
        
        // Skip lines up to bodyStartIndex
        for (int i = 0; i < _bodyStartIndex - _definitionsStartIndex; i++)
        {
            if (reader.ReadLine() == null) return; // Stop if end of file is reached
        }
        
        // Write body lines
        while (reader.ReadLine() is string line)
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
        VcdScope? rootScope = IterateThroughSignalsofScopeRecursive(RootScope);
        RootScope = rootScope;
    }

    public VcdScope? IterateThroughSignalsofScopeRecursive(VcdScope? currentRootScope)
    {
        for (int i = 0; i < currentRootScope.SubScopes.Count; i++)
        {
            currentRootScope.SubScopes[i] = IterateThroughSignalsofScopeRecursive(currentRootScope.SubScopes[i]);
        }
        
        foreach (var signal in currentRootScope.Signals.ToList())
        {
            // TODO: set delimiter through OneWare Settings? synchronize with NetlistViewer
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
    // increments definitionsStartIndex and bodyStartIndex for later usage
    private VcdScope? ParseVcdDefinitions(StreamReader reader)
    {
        VcdScope? currentScope = RootScope;
        bool headerParsed = false;
        string? line;

        while((line = reader.ReadLine()) !=null)
        {
            line = line.Trim();
            var parts = line.Split(' ');
            
            _bodyStartIndex++;
            
            if (!headerParsed)
            {
                if ((line.StartsWith("$scope") || line.StartsWith("$var")))
                {
                    headerParsed = true;
                }
                else
                {
                    _definitionsStartIndex++;
                }
            }
            if (headerParsed)
            {
                // parse definitions and save in datastructure
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
    
    public int GetBodyStartIndex(StreamReader reader)
    {
        int index = 0;
        while (reader.ReadLine() is string line)
        {
            if (line.StartsWith("$enddefinitions")) return index + 1;
            index++;
        }
        return -1;
    }
    
    // using System.IO.Hashing
    public string HashVcd(StreamReader reader)
    {
        var xxHash = new XxHash32();     
        char[] buffer = new char[8192];
        int charsRead;
        
        while ((charsRead = reader.Read(buffer, 0, buffer.Length)) > 0)
        {
            ReadOnlySpan<byte> byteSpan = Encoding.UTF8.GetBytes(buffer, 0, charsRead);
            xxHash.Append(byteSpan);
        }

        return BitConverter.ToString(xxHash.GetCurrentHash()).Replace("-", "").ToLower();
    }

    // public string HashVcdBody(StreamReader reader, int bodystartIndex)
    // {
    //     for (int i = 0; i < bodyStartIndex; i++)
    //     {
    //         reader.ReadLine();
    //     }
    //
    //     char[] buffer = new char[8192];
    //
    //     
    //     IHashService hashService = ServiceManager.GetHashService();
    //     // ReadOnlySpan<byte> bodyByteSpan = new ReadOnlySpan<Byte>(Encoding.UTF8.GetBytes(content));
    //     // uint bodyhash = hashService.ComputeHash(bodyByteSpan);
    //
    //     // return bodyhash.ToString("X8");
    //     return "x";
    // }

    public string LoadVcdAndHashBody(string vcdPath)
    {
        Reset();

        using var stream = File.OpenRead(vcdPath);
        using var reader = new StreamReader(stream);
        RootScope = ParseVcdDefinitions(reader);
        var hash = HashVcd(reader);
        return hash;
    }
}