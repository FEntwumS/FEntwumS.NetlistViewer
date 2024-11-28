﻿namespace Oneware.NetlistReaderFrontend.Services;

public interface IToolExecuterService
{
    public Task<(bool success, string stdout, string stderr)> ExecuteToolAsync(string toolPath,
        IReadOnlyList<string> args, string workingDirectory);
}