using CommunityToolkit.Mvvm.Input;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Helpers;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using Oneware.NetlistReaderFrontend.Services;
using Oneware.NetlistReaderFrontend.ViewModels;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Services.Dialogs;
using ReactiveUI;

namespace Oneware.NetlistReaderFrontend;

public class OnewareNetlistReaderFrontendModule : IModule
{
    private ServiceManager _serviceManager;
    
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<FrontendService>();
        containerRegistry.RegisterSingleton<IFileOpener, FileOpener>();
        containerRegistry.RegisterSingleton<IJsonLoader, JsonLoader>();
        containerRegistry.RegisterSingleton<IViewportDimensionService, ViewportDimensionService>();
        containerRegistry.RegisterSingleton<ICustomLogger, CustomLogger>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        _serviceManager = new ServiceManager(containerProvider);
        
        var frontendService = containerProvider.Resolve<FrontendService>();
        
        containerProvider.Resolve<IDockService>().RegisterLayoutExtension<FrontendViewModel>(DockShowLocation.Document);
        
        containerProvider.Resolve<IProjectExplorerService>().RegisterConstructContextMenu((selected, menuItems) =>
        {
            if (selected is [IProjectFile {Extension: ".json"} jsonFile])
            {
                menuItems.Add(new MenuItemViewModel("NetlistViewer")
                {
                    Header = $"View netlist {jsonFile.Header}",
                    Command = new AsyncRelayCommand(() => frontendService.ShowViewer(jsonFile))
                });
            }
        });
    }
}