namespace FEntwumS.Common.Services;

public interface ICcVhdlFileIndexService
{
    public Task<bool> IndexFileAsync(string filePath, UInt64 netlistId);
    public (string srcfile, long actualSrcline, bool success) GetActualSource(long srcline, UInt64 netlistId);
}