using Asmichi.ProcessManagement;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Services;

namespace FEntwumS.NetlistViewer.Services;

public class ToolExecuterService : IToolExecuterService
{
    private readonly IChildProcessService _childProcessService;
    private readonly ICustomLogger _logger;

    public ToolExecuterService()
    {
        _childProcessService = ServiceManager.GetService<IChildProcessService>();
        _logger = ServiceManager.GetCustomLogger();
    }
    
    public async Task<(bool success, string stdout, string stderr)> ExecuteToolAsync(string toolPath, IReadOnlyList<string> args, string workingDirectory)
    {
        bool success = false;
        string stdout = string.Empty;
        string stderr = string.Empty;
        
        (success, _) = await _childProcessService.ExecuteShellAsync(toolPath, args, workingDirectory, "", AppState.Idle, false, x =>
        {
            if (x.StartsWith("ghdl:error:"))
            {
                _logger.Error(x);
                return false;
            }

            stdout += x + "\n";

            //_logger.Log(x);
            return true;
        }, x =>
        {
            if (x.StartsWith("ghdl:error:") || x.StartsWith("ERROR:"))
            {
                _logger.Error(x);
                return false;
            }
            
            stderr += x + "\n";
                
            _logger.Log(x);
            return true;
        });
        
        return (success, stdout, stderr);
    }

    public async Task<IChildProcess> ExecuteBackgroundProcessAsync(string path, IReadOnlyList<string> args, string workingDirectory)
    {
        ChildProcessStartInfo info = new ChildProcessStartInfo
        {
            WorkingDirectory = workingDirectory,
            FileName = path,
            Arguments = args
        };
        
        var process = ChildProcess.Start(info);
        
        return process;
    }
}