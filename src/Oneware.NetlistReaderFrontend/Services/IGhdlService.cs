using OneWare.Essentials.Models;
using OneWare.Essentials.Services;

namespace Oneware.NetlistReaderFrontend.Services;

public interface IGhdlService
{
    Task<bool> ElaborateDesignAsync(IProjectFile file);
    Task<bool> CrossCompileDesignAsync(IProjectFile file);
    Task<(bool success, string stdout, string stderr)> RunGhdlAsync(string ghdlPath, IReadOnlyCollection<string> ghdlArgs, string workingDirectory);
}