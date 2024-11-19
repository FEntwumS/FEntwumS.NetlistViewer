using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using Oneware.NetlistReaderFrontend.ViewModels;

namespace Oneware.NetlistReaderFrontend.Services;

public class FrontendService
{
    private readonly ILogger _logger;
    private readonly IApplicationStateService _applicationStateService;
    private readonly IDockService _dockService;

    public FrontendService(ILogger logger, IApplicationStateService applicationStateService, IDockService dockService)
    {
        _logger = logger;
        _applicationStateService = applicationStateService;
        _dockService = dockService;
    }

    public async Task ShowViewer(IProjectFile json)
    {
        _dockService.Show(new FrontendViewModel(json));
    }
}