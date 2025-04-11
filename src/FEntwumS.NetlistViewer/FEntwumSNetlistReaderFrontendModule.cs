using System.Net.Http.Headers;
using System.Reflection;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using CommunityToolkit.Mvvm.Input;
using FEntwumS.NetlistViewer.Services;
using FEntwumS.NetlistViewer.ViewModels;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Helpers;
using OneWare.Essentials.Models;
using OneWare.Essentials.PackageManager;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using Prism.Ioc;
using Prism.Modularity;
using FEntwumS.Common.Services;
using FEntwumS.Common.Types;

namespace FEntwumS.NetlistViewer;

public class FEntwumSNetlistReaderFrontendModule : IModule
{
    public static readonly Package NetlistPackage = new()
    {
        Category = "Binaries",
        Id = "NetlistReaderBackend",
        Type = "NativeTool",
        Name = "FEntwumS NetlistViewer Backend",
        Description = "Backend for the FEntwumS Netlist Viewer",
        License = "MIT License",
        IconUrl = "https://avatars.githubusercontent.com/u/184253110?s=200&v=4",
        Links =
        [
            new PackageLink()
            {
                Name = "GitHub",
                Url = "https://github.com/FEntwumS/NetlistReaderBackend",
            }
        ],
        Tabs =
        [
            new PackageTab()
            {
                Title = "License",
                ContentUrl =
                    "https://raw.githubusercontent.com/FEntwumS/NetlistReaderBackend/refs/heads/master/LICENSE.txt"
            }
        ],
        Versions =
        [
            new PackageVersion()
            {
                Version = "0.5.1",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "win-x64",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.5.1/fentwums-netlist-reader-server-v0.5.1.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    }
                ]
            },
            new PackageVersion()
            {
                Version = "0.5.2",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "win-x64",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.5.2/fentwums-netlist-reader-server-v0.5.2.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    }
                ]
            },
            new PackageVersion()
            {
                Version = "0.5.3",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "win-x64",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.5.3/fentwums-netlist-reader-server-v0.5.3.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    }
                ]
            },
            new PackageVersion()
            {
                Version = "0.5.4",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "win-x64",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.5.4/fentwums-netlist-reader-server-v0.5.4.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    }
                ]
            },
            new PackageVersion()
            {
                Version = "0.5.5",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "win-x64",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.5.5/fentwums-netlist-reader-server-v0.5.5.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    }
                ]
            },
            new PackageVersion()
            {
                Version = "0.5.6",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "win-x64",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.5.6/fentwums-netlist-reader-server-v0.5.6.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    },
                    new PackageTarget()
                    {
                        Target = "linux-x64",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.5.6/fentwums-netlist-reader-server-v0.5.6.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    },
                    new PackageTarget()
                    {
                        Target = "osx-x64",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.5.6/fentwums-netlist-reader-server-v0.5.6.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    }
                ]
            },
            new PackageVersion()
            {
                Version = "0.5.7",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "win-x64",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.5.7/fentwums-netlist-reader-server-v0.5.7.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    },
                    new PackageTarget()
                    {
                        Target = "linux-x64",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.5.7/fentwums-netlist-reader-server-v0.5.7.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    },
                    new PackageTarget()
                    {
                        Target = "osx-x64",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.5.7/fentwums-netlist-reader-server-v0.5.7.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    }
                ]
            },
            new PackageVersion()
            {
                Version = "0.5.8",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "all",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.5.8/fentwums-netlist-reader-server-v0.5.8.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    }
                ]
            },
            new PackageVersion()
            {
                Version = "0.6.0",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "all",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.6.0/fentwums-netlist-reader-server-v0.6.0.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    }
                ]
            },
            new PackageVersion()
            {
                Version = "0.6.1",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "all",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.6.1/fentwums-netlist-reader-server-v0.6.1.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    }
                ]
            },
            new PackageVersion()
            {
                Version = "0.6.2",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "all",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.6.2/fentwums-netlist-reader-server-v0.6.2.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    }
                ]
            },
            new PackageVersion()
            {
                Version = "0.6.3",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "all",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.6.3/fentwums-netlist-reader-server-v0.6.3.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    }
                ]
            },
            new PackageVersion()
            {
                Version = "0.7.0",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "all",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.7.0/fentwums-netlist-reader-server-v0.7.0.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    }
                ]
            },
            new PackageVersion()
            {
                Version = "0.7.1",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "all",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.7.1/fentwums-netlist-reader-server-v0.7.1.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    }
                ]
            },
            new PackageVersion()
            {
                Version = "0.8.0",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "all",
                        Url =
                            "https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.8.0/fentwums-netlist-reader-server-v0.8.0.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "fentwums-netlist-reader",
                                SettingKey = NetlistPathSetting,
                            }
                        ]
                    }
                ]
            }
        ]
    };

    public static readonly Package JDKPackage = new()
    {
        Category = "Binaries",
        Id = "OpenJDK",
        Type = "NativeTool",
        Name = "Eclipse Adoptium OpenJDK",
        Description = "Production-ready open-source builds of the Java Development Kit",
        License = "GPL 2.0 with Classpath Exception",
        Links =
        [
            new PackageLink()
            {
                Name = "adoptium.net",
                Url = "https://adoptium.net/en-GB/temurin/releases/"
            }
        ],
        Tabs =
        [
            new PackageTab()
            {
                Title = "License",
                ContentUrl = "https://openjdk.org/legal/gplv2+ce.html"
            }
        ],
        Versions =
        [
            new PackageVersion()
            {
                Version = "21.0.6",
                Targets =
                [
                    new PackageTarget()
                    {
                        Target = "win-x64",
                        Url =
                            "https://github.com/adoptium/temurin21-binaries/releases/download/jdk-21.0.6%2B7/OpenJDK21U-jre_x64_windows_hotspot_21.0.6_7.zip",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "",
                                SettingKey = JavaPathSetting,
                            }
                        ]
                    },
                    new PackageTarget()
                    {
                        Target = "win-arm64",
                        Url =
                            "https://github.com/adoptium/temurin21-binaries/releases/download/jdk-21.0.6%2B7/OpenJDK21U-jre_aarch64_windows_hotspot_21.0.6_7.zip",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "",
                                SettingKey = JavaPathSetting,
                            }
                        ]
                    },
                    new PackageTarget()
                    {
                        Target = "linux-x64",
                        Url =
                            "https://github.com/adoptium/temurin21-binaries/releases/download/jdk-21.0.6%2B7/OpenJDK21U-jre_x64_linux_hotspot_21.0.6_7.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "",
                                SettingKey = JavaPathSetting,
                            }
                        ]
                    },
                    new PackageTarget()
                    {
                        Target = "linux-arm64",
                        Url =
                            "https://github.com/adoptium/temurin21-binaries/releases/download/jdk-21.0.6%2B7/OpenJDK21U-jre_aarch64_linux_hotspot_21.0.6_7.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "",
                                SettingKey = JavaPathSetting,
                            }
                        ]
                    },
                    new PackageTarget()
                    {
                        Target = "osx-x64",
                        Url =
                            "https://github.com/adoptium/temurin21-binaries/releases/download/jdk-21.0.6%2B7/OpenJDK21U-jre_x64_mac_hotspot_21.0.6_7.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "",
                                SettingKey = JavaPathSetting,
                            }
                        ]
                    },
                    new PackageTarget()
                    {
                        Target = "osx-arm64",
                        Url =
                            "https://github.com/adoptium/temurin21-binaries/releases/download/jdk-21.0.6%2B7/OpenJDK21U-jre_aarch64_mac_hotspot_21.0.6_7.tar.gz",
                        AutoSetting =
                        [
                            new PackageAutoSetting()
                            {
                                RelativePath = "",
                                SettingKey = JavaPathSetting,
                            }
                        ]
                    }
                ]
            }
        ]
    };

    private ServiceManager? _serviceManager;

    public const string NetlistPathSetting = "FEntwumS_NetlistReaderBackend";
    public const string JavaPathSetting = "FEntwumS_JDKPath";

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IFileOpener, FileOpener>();
        containerRegistry.RegisterSingleton<IViewportDimensionService, ViewportDimensionService>();
        containerRegistry.RegisterSingleton<IJsonLoader, JsonLoader>();
        containerRegistry.RegisterSingleton<ICustomLogger, CustomLogger>();
        containerRegistry.RegisterSingleton<IHashService, OAATHashService>();
        containerRegistry.RegisterSingleton<IGhdlService, GhdlService>();
        containerRegistry.RegisterSingleton<IYosysService, YosysService>();
        containerRegistry.RegisterSingleton<IToolExecuterService, ToolExecuterService>();
        containerRegistry.RegisterSingleton<IFpgaBbService, FpgaBbService>();
        containerRegistry.RegisterSingleton<ICcVhdlFileIndexService, CcVhdlFileIndexService>();
        containerRegistry.RegisterSingleton<IFrontendService, FrontendService>();
        containerRegistry.Register<FrontendViewModel>();
    }

    public void OnInitialized(IContainerProvider? containerProvider)
    {
        _serviceManager = new ServiceManager(containerProvider);
        
        ICustomLogger logger = ServiceManager.GetService<ICustomLogger>();
        
        // Log some debug information
        logger.Log($" Platform: {PlatformHelper.Platform}");
        
        // Add binary packages to store
        ServiceManager.GetService<IPackageService>().RegisterPackage(NetlistPackage);
        ServiceManager.GetService<IPackageService>().RegisterPackage(JDKPackage);
        
        // Add WaveformInteractor plugin to store
        string applicationDir = "";
        
        // Get compilation directory as fallback
        string developmentDir = (Assembly.GetExecutingAssembly().CustomAttributes.FirstOrDefault(attr => (attr.AttributeType.FullName is "System.Reflection.AssemblyMetadataAttribute" && attr.ConstructorArguments[0].Value as string is "BuildDir"))!.ConstructorArguments[1].Value as string)!;
        string productionDir =
            Path.Combine(ServiceManager.GetService<IPaths>().PluginsDirectory, "FEntwumS.NetlistViewer/");

        if (Assembly.GetExecutingAssembly().CustomAttributes.FirstOrDefault(attr =>
                attr.AttributeType.FullName is "System.Reflection.AssemblyConfigurationAttribute" &&
                attr.ConstructorArguments[0].Value as string is "Debug") != null)
        {
            applicationDir = developmentDir;
        }
        else
        {
            applicationDir = productionDir;
        }
        
        ServiceManager.GetService<IPluginService>().AddPlugin(Path.Combine(applicationDir, "ecosystem/FEntwumS.WaveformInteractor"));
        
        logger.Log("Registered Packages");

        ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Backend", NetlistPathSetting,
            new FolderPathSetting("Path to folder containing server jar", "fentwums-netlist-reader", "",
                NetlistPathSetting, Path.Exists));

        ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Backend", JavaPathSetting,
            new FolderPathSetting("Path to folder containing java binary", "", "", JavaPathSetting,
                Path.Exists));

        ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Backend",
            "NetlistViewer_java_args",
            new TextBoxSetting("Extra arguments for the Java Virtual Machine", "-Xmx16G -XX:+UseZGC -XX:+ZGenerational",
                "null"));

        var resourceInclude = new ResourceInclude(new Uri("avares://FEntwumS.NetlistViewer/Styles/Icons.axaml"))
            { Source = new Uri("avares://FEntwumS.NetlistViewer/Styles/Icons.axaml") };

        Application.Current?.Resources.MergedDictionaries.Add(resourceInclude);

        ISettingsService settingsService = ServiceManager.GetService<ISettingsService>();

        var frontendService = ServiceManager.GetService<IFrontendService>();

        ServiceManager.GetService<IDockService>().RegisterLayoutExtension<FrontendViewModel>(DockShowLocation.Document);
        
        logger.Log("Registered FrontendViewModel as Document in dock system");

        ServiceManager.GetService<IProjectExplorerService>().RegisterConstructContextMenu((selected, menuItems) =>
        {
            if (selected is [IProjectFile { Extension: ".json" } jsonFile])
            {
                menuItems.Add(new MenuItemViewModel("NetlistViewer")
                {
                    Header = $"View netlist {jsonFile.Header}",
                    Command = new AsyncRelayCommand(() => frontendService.ShowViewerAsync(jsonFile))
                });
            }
            else if (selected is [IProjectFile { Extension: ".vhd" } vhdlFile])
            {
                menuItems.Add(new MenuItemViewModel("NetlistViewer_CreateNetlist")
                {
                    Header = $"View netlist for {vhdlFile.Header}",
                    Command = new AsyncRelayCommand(() => frontendService.CreateVhdlNetlistAsync(vhdlFile))
                });
            }
            else if (selected is [IProjectFile { Extension: ".v" } verilogFile])
            {
                menuItems.Add(new MenuItemViewModel("NetlistViewer_CreateVerilogNetlist")
                {
                    Header = $"View netlist for {verilogFile.Header}",
                    Command = new AsyncRelayCommand(() => frontendService.CreateVerilogNetlistAsync(verilogFile))
                });
            }
            else if (selected is [IProjectFile { Extension: ".sv" } systemVerilogFile])
            {
                menuItems.Add(new MenuItemViewModel("NetlistViewer_CreateSystemVerilogNetlist")
                {
                    Header = $"View netlist for {systemVerilogFile.Header}",
                    Command = new AsyncRelayCommand(() => frontendService.CreateSystemVerilogNetlistAsync(systemVerilogFile))
                });
            }
        });
        
        logger.Log("Registered custom context menu entries");

        settingsService.RegisterSettingCategory("Netlist Viewer", 100, "netlistIcon");
        settingsService.RegisterSettingSubCategory("Netlist Viewer", "VHDL");

        settingsService.RegisterSetting("Netlist Viewer", "VHDL", "NetlistViewer_VHDL_Standard",
            new ComboBoxSetting("VHDL Standard", "93c", ["87", "93", "93c", "00", "02", "08", "19"]));

        settingsService.RegisterSettingSubCategory("Netlist Viewer", "FPGA");

        settingsService.RegisterSetting("Netlist Viewer", "FPGA", "NetlistViewer_FPGA_Manufacturer",
            new ComboBoxSetting("FPGA manufacturer", "gatemate",
            [
                "achronix", "anlogic", "coolrunner2", "ecp5", "efinix", "fabulous", "gatemate", "gowin", "greenpak4",
                "ice40", "intel", "intel_alm", "lattice", "microchip", "nanoxplore", "nexus", "quicklogic", "sf2",
                "xilinx"
            ]));

        settingsService.RegisterSetting("Netlist Viewer", "FPGA", "NetlistViewer_FPGA_DeviceFamily",
            new TextBoxSetting("Device family", "", null));

        settingsService.RegisterSettingSubCategory("Netlist Viewer", "Backend");

        settingsService.RegisterSetting("Netlist Viewer", "Backend", "NetlistViewer_Backend_Address",
            new TextBoxSetting("Server address", "127.0.0.1", null));
        settingsService.RegisterSetting("Netlist Viewer", "Backend", "NetlistViewer_Backend_Port",
            new TextBoxSetting("Port", "8080", null));
        settingsService.RegisterSetting("Netlist Viewer", "Backend", "NetlistViewer_Backend_RequestTimeout",
            new TextBoxSetting("Request Timeout (in seconds)", "8000", null));
        settingsService.RegisterSetting("Netlist Viewer", "Backend", "NetlistViewer_Backend_UseLocal",
            new CheckBoxSetting("Use local backend server", true));

        settingsService.RegisterSettingSubCategory("Netlist Viewer", "Font sizes");

        settingsService.RegisterSetting("Netlist Viewer", "Font sizes", "NetlistViewer_EntityFontSize",
            new TextBoxSetting("Entity Label Font Size", "25", null));
        settingsService.RegisterSetting("Netlist Viewer", "Font sizes", "NetlistViewer_CellFontSize",
            new TextBoxSetting("Cell Label Font Size", "15", null));
        settingsService.RegisterSetting("Netlist Viewer", "Font sizes", "NetlistViewer_EdgeFontSize",
            new TextBoxSetting("Edge Label Font Size", "10", null));
        settingsService.RegisterSetting("Netlist Viewer", "Font sizes", "NetlistViewer_PortFontSize",
            new TextBoxSetting("Port Font Size", "10", null));

        settingsService.RegisterSettingSubCategory("Netlist Viewer", "Experimental");

        settingsService.RegisterSetting("Netlist Viewer", "Experimental", "NetlistViewer_ContinueOnBinaryInstallError",
            new CheckBoxSetting("Continue if errors occur during dependency installation", false));
        
        logger.Log("Registered custom settings");

        // Subscribe the FrontendService _AFTER_ the relevant settings have been registered
        ServiceManager.GetService<FrontendService>().SubscribeToSettings();
        ServiceManager.GetService<IFpgaBbService>().SubscribeToSettings();
        
        logger.Log("Subscribed relevant services to the settings relevant to them");
        
        ServiceManager.GetService<IApplicationStateService>().RegisterShutdownAction(() =>
        {
            try
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.spring-boot.actuator.v3+json"));
                client.DefaultRequestHeaders.Add("User-Agent", "FEntwumS.NetlistViewer");
                client.BaseAddress = new Uri("http://localhost:8080");
                client.Timeout = TimeSpan.FromSeconds(1);

                var res = client.GetAsync("/shutdown-backend");
            }
            catch (Exception)
            {
                
            }
        });
    }
}