namespace FEntwumS.NetlistViewer.Services;

public interface ICcVhdlFileIndexService
{
    public Task<bool> IndexFileAsync(string filePath, UInt64 netlistId);
    public Task<(string srcfile, long actualSrcline, bool success)> GetActualSourceAsync(long srcline, UInt64 netlistId);
}