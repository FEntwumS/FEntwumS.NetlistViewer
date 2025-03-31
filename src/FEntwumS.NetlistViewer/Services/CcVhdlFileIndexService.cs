using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using OneWare.Essentials.Helpers;

namespace FEntwumS.NetlistViewer.Services;

public class CcVhdlFileIndexService : ICcVhdlFileIndexService
{
    private static ConcurrentDictionary<UInt64, ConcurrentDictionary<long, long>> _index = new ();
    private static ConcurrentDictionary<UInt64, ConcurrentDictionary<long, string>> _indexToFile = new ();
    
    private ICustomLogger _logger;

    public CcVhdlFileIndexService()
    {
        _logger = ServiceManager.GetCustomLogger();
    }
    
    public async Task<bool> IndexFileAsync(string filePath, ulong netlistId)
    {
        if (!File.Exists(filePath))
        {
            _logger.Log($"File {filePath} does not exist...");
            
            return false;
        }

        ConcurrentDictionary<long, long> fileIndex = new();
        ConcurrentDictionary<long, string> fileIndexToSource = new();
        
        string[] linesToIndex = await File.ReadAllLinesAsync(filePath);

        long currentLine = 1, actualSrcLine = -1;
        string formattedLine = "";

        foreach (string line in linesToIndex)
        {
            if (Regex.IsMatch(line, @"\s+\/\*.+\*\/"))
            {
                actualSrcLine = currentLine;

                // Trim whitespace
                formattedLine = line.Trim();
                // Remove block comment (first and last three characters)
                formattedLine = formattedLine.Substring(3, formattedLine.Length - 6);

                string[] formattedLineSplit = formattedLine.Split(':');

                if (PlatformHelper.Platform is PlatformId.WinArm64 or PlatformId.WinX64)
                {
                    formattedLine = $"{formattedLineSplit[0]}:{formattedLineSplit[1]}";
                    actualSrcLine = long.Parse(formattedLineSplit[2]);
                } else if (PlatformHelper.Platform is not PlatformId.Wasm or PlatformId.Unknown)
                {
                    formattedLine = formattedLineSplit[0];

                    for (int i = 1; i < formattedLineSplit.Length - 2; i++)
                    {
                        formattedLine = $"{formattedLine}:{formattedLineSplit[i]}";
                    }
                    
                    // second to last element is the line number
                    // last element is column number (currently unused)
                    actualSrcLine = long.Parse(formattedLineSplit[^2]);
                }
                else
                {
                    return false;
                }
                
                fileIndexToSource[currentLine] = formattedLine;
            }
            else
            {
                if (actualSrcLine != -1)
                {
                    fileIndex[currentLine] = actualSrcLine;
                    fileIndexToSource[currentLine] = formattedLine;
                }
            }
            
            currentLine++;
        }
        
        _index[netlistId] = fileIndex;
        _indexToFile[netlistId] = fileIndexToSource;

        return true;
    }

    public (string srcfile, long actualSrcline, bool success) GetActualSource(long srcline, ulong netlistId)
    {
        if (!_index.TryGetValue(netlistId, out ConcurrentDictionary<long, long>? fileIndex))
        {
            return ("", 0, false);
        }

        if (!_indexToFile.TryGetValue(netlistId, out ConcurrentDictionary<long, string>? fileIndexToSource))
        {
            return ("", 0, false);
        }

        if (!fileIndex.TryGetValue(srcline, out long actualSrcline))
        {
            return ("", 0, false);
        }

        if (!fileIndexToSource.TryGetValue(srcline, out string? srcfile))
        {
            return ("", 0, false);
        }
        
        return (srcfile, actualSrcline, true);
    }
}