using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using Oneware.NetlistReaderFrontend.ViewModels;

namespace Oneware.NetlistReaderFrontend.Services;

public class FrontendService(ILogger logger, IApplicationStateService applicationStateService, IDockService dockService)
{
    private readonly ILogger _logger = logger;
    private readonly IApplicationStateService _applicationStateService = applicationStateService;
    private readonly IDockService _dockService = dockService;

    public Task ShowViewer(IProjectFile json)
    {
        _dockService.Show(new FrontendViewModel());
        _dockService.InitializeContent();
        Console.WriteLine(_dockService.Layout);
        
        return Task.CompletedTask;
    }
}