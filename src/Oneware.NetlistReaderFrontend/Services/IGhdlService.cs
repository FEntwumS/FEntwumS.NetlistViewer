using OneWare.Essentials.Models;
using OneWare.Essentials.Services;

namespace Oneware.NetlistReaderFrontend.Services;

public interface IGhdlService
{
    Task<bool> ElaborateDesignAsync(IProjectFile file);
    Task<bool> CrossCompileDesignAsync(IProjectFile file);
}