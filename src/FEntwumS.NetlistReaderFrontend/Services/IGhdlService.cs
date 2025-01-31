using OneWare.Essentials.Models;
using OneWare.Essentials.Services;

namespace FEntwumS.NetlistReaderFrontend.Services;

public interface IGhdlService
{
    Task<bool> ElaborateDesignAsync(IProjectFile file);
    Task<bool> CrossCompileDesignAsync(IProjectFile file);
}