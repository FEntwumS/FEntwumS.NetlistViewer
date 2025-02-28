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

        foreach (string line in linesToIndex)
        {
            if (Regex.IsMatch(line, @"\s+\/\*.+\*\/"))
            {
                actualSrcLine = currentLine;

                // Trim whitespace
                string formattedLine = line.Trim();
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
                }
            }
            
            currentLine++;
        }

        return true;
    }

    public async Task<(string srcfile, int srcline, bool success)> GetActualSourceAsync(int srcline, ulong netlistId)
    {
        throw new NotImplementedException();
    }
}