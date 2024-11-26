using OneWare.Essentials.Models;
using OneWare.Essentials.Services;

namespace Oneware.NetlistReaderFrontend.Services;

public interface IGhdlService
{
    Task<bool> AnalyseDesignAsync(IProjectFile file);
    Task<bool> CrossCompileDesignAsync();
}