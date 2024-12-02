using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
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
        containerRegistry.RegisterSingleton<IFileOpener, FileOpener>();
        containerRegistry.RegisterSingleton<IJsonLoader, JsonLoader>();
        containerRegistry.RegisterSingleton<IViewportDimensionService, ViewportDimensionService>();
        containerRegistry.RegisterSingleton<ICustomLogger, CustomLogger>();
        containerRegistry.RegisterSingleton<IHashService, OAATHashService>();
        containerRegistry.RegisterSingleton<IGhdlService, GhdlService>();
        containerRegistry.RegisterSingleton<IYosysService, YosysService>();
        containerRegistry.RegisterSingleton<IToolExecuterService, ToolExecuterService>();
        containerRegistry.RegisterSingleton<FrontendService>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        var resourceInclude = new ResourceInclude(new Uri("avares://Oneware.NetlistReaderFrontend/Styles/Icons.axaml"))
            { Source = new Uri("avares://Oneware.NetlistReaderFrontend/Styles/Icons.axaml")};
        
        Application.Current?.Resources.MergedDictionaries.Add(resourceInclude);
        
        _serviceManager = new ServiceManager(containerProvider);

        ISettingsService settingsService = ServiceManager.GetService<ISettingsService>();

        var frontendService = containerProvider.Resolve<FrontendService>();

        containerProvider.Resolve<IDockService>().RegisterLayoutExtension<FrontendViewModel>(DockShowLocation.Document);

        containerProvider.Resolve<IProjectExplorerService>().RegisterConstructContextMenu((selected, menuItems) =>
        {
            if (selected is [IProjectFile { Extension: ".json" } jsonFile])
            {
                menuItems.Add(new MenuItemViewModel("NetlistViewer")
                {
                    Header = $"View netlist {jsonFile.Header}",
                    Command = new AsyncRelayCommand(() => frontendService.ShowViewer(jsonFile))
                });
            }
            else if (selected is [IProjectFile { Extension: ".vhd" } vhdlFile])
            {
                menuItems.Add(new MenuItemViewModel("NetlistViewer_CreateNetlist")
                {
                    Header = $"View netlist for {vhdlFile.Header}",
                    Command = new AsyncRelayCommand(() => frontendService.CreateVhdlNetlist(vhdlFile))
                });
            }
            else if (selected is [IProjectFile { Extension: ".v" } verilogFile])
            {
                menuItems.Add(new MenuItemViewModel("NetlistViewer_CreateVerilogNetlist")
                {
                    Header = $"View netlist for {verilogFile.Header}",
                    Command = new AsyncRelayCommand(() => frontendService.CreateVerilogNetlist(verilogFile))
                });
            }
            else if (selected is [IProjectFile { Extension: ".sv" } systemVerilogFile])
            {
                menuItems.Add(new MenuItemViewModel("NetlistViewer_CreateSystemVerilogNetlist")
                {
                    Header = $"View netlist for {systemVerilogFile.Header}",
                    Command = new AsyncRelayCommand(() => frontendService.CreateSystemVerilogNetlist(systemVerilogFile))
                });
            }
        });

        settingsService.RegisterSettingCategory("Netlist Viewer", 100, "netlistIcon");
        settingsService.RegisterSettingSubCategory("Netlist Viewer", "VHDL");

        settingsService.RegisterSetting("Netlist Viewer", "VHDL", "NetlistViewer_VHDL_Standard",
            new ComboBoxSetting("VHDL Standard", "93c", ["87", "93", "93c", "00", "02", "08", "19"]));

        settingsService.RegisterSettingSubCategory("Netlist Viewer", "Backend");

        settingsService.RegisterSetting("Netlist Viewer", "Backend", "NetlistViewer_Backend_Address",
            new TextBoxSetting("Server address", "127.0.0.1", null));
        settingsService.RegisterSetting("Netlist Viewer", "Backend", "NetlistViewer_Backend_Port",
            new TextBoxSetting("Port", "8080", null));
        settingsService.RegisterSetting("Netlist Viewer", "Backend", "NetlistViewer_Backend_RequestTimeout",
            new TextBoxSetting("Request Timeout (in seconds)", "1", null));
        settingsService.RegisterSetting("Netlist Viewer", "Backend", "NetlistViewer_Backend_UseRemote",
            new CheckBoxSetting("Use remote backend server", false));

        // Subscribe the FrontendService _AFTER_ the relevant settings have been registered
        ServiceManager.GetService<FrontendService>().SubscribeToSettings();
    }
}