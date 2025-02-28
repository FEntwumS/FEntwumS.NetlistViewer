using System.Collections.Concurrent;
using System.Text.RegularExpressions;

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
                // Remove block comment (first and last three cahracters)
                formattedLine = formattedLine.Substring(3, formattedLine.Length - 6);

                string[] formattedLineSplit = formattedLine.Split(':');

                // Extract filename
                formattedLine = $"{formattedLineSplit[0]}:{formattedLineSplit[1]}";
                actualSrcLine = long.Parse(formattedLineSplit[2]);
                
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

    public async Task<(string srcfile, long actualSrcline, bool success)> GetActualSourceAsync(long srcline, ulong netlistId)
    {
        string srcfile = "";
        long actualSrcline = 0;
        
        ConcurrentDictionary<long, long> fileIndex = new();
        ConcurrentDictionary<long, string> fileIndexToSource = new();

        if (!_index.TryGetValue(netlistId, out fileIndex))
        {
            return ("", 0, false);
        }

        if (!_indexToFile.TryGetValue(netlistId, out fileIndexToSource))
        {
            return ("", 0, false);
        }

        if (!fileIndex.TryGetValue(srcline, out actualSrcline))
        {
            return ("", 0, false);
        }

        if (!fileIndexToSource.TryGetValue(srcline, out srcfile))
        {
            return ("", 0, false);
        }
        
        return (srcfile, actualSrcline, true);
    }
}