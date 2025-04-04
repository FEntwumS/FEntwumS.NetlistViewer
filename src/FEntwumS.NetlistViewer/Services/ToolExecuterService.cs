using Asmichi.ProcessManagement;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Services;
using FEntwumS.Common.Services;

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
        bool noErrors = true;
        string stdout = string.Empty;
        string stderr = string.Empty;
        
        (bool success, _) = await _childProcessService.ExecuteShellAsync(toolPath, args, workingDirectory, $"Executing {Path.GetFileNameWithoutExtension(toolPath)}", AppState.Loading, false, x =>
        {
            if (x.StartsWith("ghdl:error:"))
            {
                _logger.Error(x);
                noErrors = false;
            }

            stdout += x + "\n";

            _logger.Log(x);
            return true;
        }, x =>
        {
            if (x.StartsWith("ghdl:error:") || x.StartsWith("ERROR:") || x.Contains("error:"))
            {
                noErrors = false;
            }
            
            stderr += x + "\n";
                
            _logger.Error(x);
            return true;
        });
        
        return (success && noErrors, stdout, stderr);
    }

    public IChildProcess ExecuteBackgroundProcess(string path, IReadOnlyList<string> args, string? workingDirectory)
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