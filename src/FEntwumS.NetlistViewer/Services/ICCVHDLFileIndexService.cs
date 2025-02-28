﻿namespace FEntwumS.NetlistViewer.Services;

public interface ICcVhdlFileIndexService
{
    public Task<bool> IndexFileAsync(string filePath, UInt64 netlistId);
    public Task<(string srcfile, int srcline, bool success)> GetActualSourceAsync(int srcline, UInt64 netlistId);
}