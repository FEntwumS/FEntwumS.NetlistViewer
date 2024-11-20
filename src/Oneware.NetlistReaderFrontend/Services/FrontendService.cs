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
        var vm = new FrontendViewModel();
        vm.InitializeContent();
        vm.Title = json.Name;
        vm.File = File.Open(json.FullPath, FileMode.Open, FileAccess.Read);
        _dockService.Show(vm, DockShowLocation.Document);
        _dockService.InitializeContent();
        vm.OpenFileImpl();
        
        return Task.CompletedTask;
    }
}