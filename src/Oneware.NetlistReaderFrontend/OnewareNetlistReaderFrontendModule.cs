using CommunityToolkit.Mvvm.Input;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using Oneware.NetlistReaderFrontend.Services;
using Prism.Ioc;
using Prism.Modularity;
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
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        _serviceManager = new ServiceManager(containerProvider);
        
        var frontendService = containerProvider.Resolve<FrontendService>();
        
        containerProvider.Resolve<IProjectExplorerService>().RegisterConstructContextMenu((selected, menuItems) =>
        {
            if (selected is [IProjectFile {Extension: ".vhd"} jsonFile])
            {
                menuItems.Add(new MenuItemViewModel("Hello World")
                {
                    Header = $"Hello World {jsonFile.Header}",
                    Command = new AsyncRelayCommand(() => frontendService.ShowViewer(jsonFile))
                });
            }
        });
    }
}