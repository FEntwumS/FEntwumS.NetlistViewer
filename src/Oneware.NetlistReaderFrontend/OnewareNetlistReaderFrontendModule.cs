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
        containerRegistry.RegisterSingleton<IHashService, OAATHashService>();
        containerRegistry.RegisterSingleton<IGhdlService, GhdlService>();
        containerRegistry.RegisterSingleton<IYosysService, YosysService>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        _serviceManager = new ServiceManager(containerProvider);
        
        ISettingsService settingsService = ServiceManager.GetService<ISettingsService>();
        
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
            } else if (selected is [IProjectFile { Extension: ".vhd" } vhdlFile])
            {
                menuItems.Add(new MenuItemViewModel("NetlistViewer_CreateNetlist")
                {
                    Header = $"View netlist for {vhdlFile.Header}",
                    Command = new AsyncRelayCommand(() => frontendService.CreateVhdlNetlist(vhdlFile))
                });
            } else if (selected is [IProjectFile { Extension: ".v" } verilogFile])
            {
                menuItems.Add(new MenuItemViewModel("NetlistViewer_CreateVerilogNetlist")
                {
                    Header = $"View netlist for {verilogFile.Header}",
                    Command = new AsyncRelayCommand(() => frontendService.CreateVerilogNetlist(verilogFile))
                });
            } else if (selected is [IProjectFile { Extension: ".sv" } systemVerilogFile])
            {
                menuItems.Add(new MenuItemViewModel("NetlistViewer_CreateSystemVerilogNetlist")
                {
                    Header = $"View netlist for {systemVerilogFile.Header}",
                    Command = new AsyncRelayCommand(() => frontendService.CreateSystemVerilogNetlist(systemVerilogFile))
                });
            }
        });
        
        settingsService.RegisterSettingCategory("Netlist Viewer");
        settingsService.RegisterSettingSubCategory("Netlist Viewer", "VHDL");
        
        settingsService.RegisterSetting("Netlist Viewer", "VHDL", "NetlistViewer_VHDL_Standard", new ComboBoxSetting("VHDL Standard", "93c", [ "87", "93", "93c", "00", "02", "08", "19"]));
    }
}