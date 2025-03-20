namespace FEntwumS.WaveformInteractor.Services;

public class VcdService : IVcdService
{
    private VcdScope _rootScope = new("PARENT");
    private List<string> _bodyLines = new();
    private List<string> _headerLines = new();

    public VcdService(List<string> headerLines)
    {
        _headerLines = headerLines;
    }

    public void LoadVcd(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        _rootScope = ParseVcdHeader(lines, out int bodyStartIndex);
        _bodyLines = lines.Skip(bodyStartIndex).ToList();
    }

    public void WriteVcd(string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            // Write header lines
            foreach (var line in _headerLines)
            {
                writer.WriteLine(line);
            }

            // Write VCD hierarchy from data structure
            WriteScopeRecursive(writer, _rootScope);
            // Write enddefinitions block
            writer.WriteLine("$enddefinitions $end");
            
            // Write body lines
            foreach (var line in _bodyLines)
            {
                writer.WriteLine(line);
            }
        }
    }

    private void WriteScopeRecursive(StreamWriter writer, VcdScope scope)
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
        VcdScope rootScope = IterateThroughSignalsofScopeRecursive(_rootScope);
        _rootScope = rootScope;
    }

    public VcdScope IterateThroughSignalsofScopeRecursive(VcdScope currentRootScope)
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
                        var newScope = new VcdScope(scopeName) { parent = targetScope };
                        targetScope.SubScopes.Add(newScope);
                        targetScope = newScope;
                    }
                    else
                    {
                        targetScope = existingScope;
                    }
                }
                // add signal to the correct scope
                targetScope.Signals.Add(new Signal(signal));
                currentRootScope.Signals.Remove(signal);
            }
        }
        return currentRootScope;
    }
    
    private VcdScope ParseVcdHeader(string[] lines, out int bodyStartIndex)
    {
        VcdScope currentScope = _rootScope;
        bodyStartIndex = 0;
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
            // parse header and save in datastructure
            if (headerParsed)
            {
                if (line.StartsWith("$scope"))
                {
                    VcdScope newScope = new VcdScope(parts[2]);// get name of scope
                    newScope.parent = currentScope;
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
                    bodyStartIndex = i + 1;
                    break;
                }
            }
        }
        return currentScope;
    }
}

public class VcdScope
{
    public string Name { get; set; }
    public List<VcdScope> SubScopes { get; } = new();
    public List<Signal> Signals { get; } = new();
    public VcdScope parent;

    public VcdScope(string name)
    {
        Name = name;
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

    public Signal()
    {
        throw new NotImplementedException();
    }

    public Signal(Signal signal)
    {
        Type = signal.Type;
        BitWidth = signal.BitWidth;
        Id = signal.Id;
        Name = signal.Name;
        Value = signal.Value;
    }

    public void UpdateValue(string newValue)
    {
        Value = newValue;
    }
}