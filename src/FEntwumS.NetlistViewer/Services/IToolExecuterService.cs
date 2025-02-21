using Asmichi.ProcessManagement;

namespace FEntwumS.NetlistViewer.Services;

public interface IToolExecuterService
{
    public Task<(bool success, string stdout, string stderr)> ExecuteToolAsync(string toolPath,
        IReadOnlyList<string> args, string workingDirectory);
    
    public Task<IChildProcess> ExecuteBackgroundProcessAsync(string path, IReadOnlyList<string> args, string workingDirectory);
}