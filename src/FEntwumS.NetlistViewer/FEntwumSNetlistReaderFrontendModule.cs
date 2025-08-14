using System.Net.Http.Headers;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using CommunityToolkit.Mvvm.Input;
using FEntwumS.NetlistViewer.Helpers;
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

namespace FEntwumS.NetlistViewer;

public class FEntwumSNetlistReaderFrontendModule : IModule
{
	public static readonly Package NetlistViewerBackendPackage = new()
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
				Title = "README",
				ContentUrl = "https://raw.githubusercontent.com/FEntwumS/NetlistReaderBackend/refs/heads/master/README.md"
			},
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
				Version = "0.11.2",
				Targets =
				[
					new PackageTarget()
					{
						Target = "all",
						Url =
							"https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.11.2/fentwums-netlist-reader-server-v0.11.2.tar.gz",
						AutoSetting =
						[
							new PackageAutoSetting()
							{
								RelativePath = "fentwums-netlist-reader",
								SettingKey = FentwumSNetlistViewerSettingsHelper.NetlistPathSettingKey,
							}
						]
					}
				]
			},
			new PackageVersion()
			{
				Version = "0.11.3",
				Targets =
				[
					new PackageTarget()
					{
						Target = "all",
						Url =
							"https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.11.3/fentwums-netlist-reader-server-v0.11.3.tar.gz",
						AutoSetting =
						[
							new PackageAutoSetting()
							{
								RelativePath = "fentwums-netlist-reader",
								SettingKey = FentwumSNetlistViewerSettingsHelper.NetlistPathSettingKey,
							}
						]
					}
				]
			}
		]
	};

	public static readonly Package JREPackage = new()
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
								SettingKey = FentwumSNetlistViewerSettingsHelper.JavaPathSettingKey,
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
								SettingKey = FentwumSNetlistViewerSettingsHelper.JavaPathSettingKey,
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
								SettingKey = FentwumSNetlistViewerSettingsHelper.JavaPathSettingKey,
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
								SettingKey = FentwumSNetlistViewerSettingsHelper.JavaPathSettingKey,
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
								SettingKey = FentwumSNetlistViewerSettingsHelper.JavaPathSettingKey,
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
								SettingKey = FentwumSNetlistViewerSettingsHelper.JavaPathSettingKey,
							}
						]
					}
				]
			},
			new PackageVersion()
			{
				Version = "21.0.8",
				Targets =
				[
					new PackageTarget()
					{
						Target = "win-x64",
						Url =
							"https://github.com/adoptium/temurin21-binaries/releases/download/jdk-21.0.8%2B9/OpenJDK21U-jre_x64_windows_hotspot_21.0.8_9.zip",
						AutoSetting =
						[
							new PackageAutoSetting()
							{
								RelativePath = "",
								SettingKey = FentwumSNetlistViewerSettingsHelper.JavaPathSettingKey,
							}
						]
					},
					new PackageTarget()
					{
						Target = "win-arm64",
						Url =
							"https://github.com/adoptium/temurin21-binaries/releases/download/jdk-21.0.8%2B9/OpenJDK21U-jre_aarch64_windows_hotspot_21.0.8_9.zip",
						AutoSetting =
						[
							new PackageAutoSetting()
							{
								RelativePath = "",
								SettingKey = FentwumSNetlistViewerSettingsHelper.JavaPathSettingKey,
							}
						]
					},
					new PackageTarget()
					{
						Target = "linux-x64",
						Url =
							"https://github.com/adoptium/temurin21-binaries/releases/download/jdk-21.0.8%2B9/OpenJDK21U-jre_x64_linux_hotspot_21.0.8_9.tar.gz",
						AutoSetting =
						[
							new PackageAutoSetting()
							{
								RelativePath = "",
								SettingKey = FentwumSNetlistViewerSettingsHelper.JavaPathSettingKey,
							}
						]
					},
					new PackageTarget()
					{
						Target = "linux-arm64",
						Url =
							"https://github.com/adoptium/temurin21-binaries/releases/download/jdk-21.0.8%2B9/OpenJDK21U-jre_aarch64_linux_hotspot_21.0.8_9.tar.gz",
						AutoSetting =
						[
							new PackageAutoSetting()
							{
								RelativePath = "",
								SettingKey = FentwumSNetlistViewerSettingsHelper.JavaPathSettingKey,
							}
						]
					},
					new PackageTarget()
					{
						Target = "osx-x64",
						Url =
							"https://github.com/adoptium/temurin21-binaries/releases/download/jdk-21.0.8%2B9/OpenJDK21U-jre_x64_mac_hotspot_21.0.8_9.tar.gz",
						AutoSetting =
						[
							new PackageAutoSetting()
							{
								RelativePath = "",
								SettingKey = FentwumSNetlistViewerSettingsHelper.JavaPathSettingKey,
							}
						]
					},
					new PackageTarget()
					{
						Target = "osx-arm64",
						Url =
							"https://github.com/adoptium/temurin21-binaries/releases/download/jdk-21.0.8%2B9/OpenJDK21U-jre_aarch64_mac_hotspot_21.0.8_9.tar.gz",
						AutoSetting =
						[
							new PackageAutoSetting()
							{
								RelativePath = "",
								SettingKey = FentwumSNetlistViewerSettingsHelper.JavaPathSettingKey,
							}
						]
					}
				]
			}
		]
	};

	private static bool EnableHierarchyView = false;

	public void RegisterTypes(IContainerRegistry containerRegistry)
	{
		containerRegistry.RegisterSingleton<IViewportDimensionService, ViewportDimensionService>();
		containerRegistry.RegisterSingleton<IJsonLoader, JsonLoader>();
		containerRegistry.RegisterSingleton<ICustomLogger, CustomLogger>();
		containerRegistry.RegisterSingleton<IHashService, OAATHashService>();
		containerRegistry.RegisterSingleton<IYosysService, YosysService>();
		containerRegistry.RegisterSingleton<IToolExecuterService, ToolExecuterService>();
		containerRegistry.RegisterSingleton<IFpgaBbService, FpgaBbService>();
		containerRegistry.RegisterSingleton<ICcVhdlFileIndexService, CcVhdlFileIndexService>();
		containerRegistry.RegisterSingleton<IFrontendService, FrontendService>();
		containerRegistry.RegisterSingleton<INetlistGenerator, NetlistGenerator>();
		containerRegistry.Register<FrontendViewModel>();
		containerRegistry.RegisterSingleton<IHierarchyJsonParser, HierarchyJsonParser>();
		containerRegistry.RegisterSingleton<IHierarchyInformationService, HierarchyInformationService>();
		containerRegistry.RegisterSingleton<IStorageService, StorageService>();
	}

	public void OnInitialized(IContainerProvider? containerProvider)
	{
		ILogger logger = ServiceManager.GetService<ILogger>();

		// Log some debug information
		ServiceManager.GetCustomLogger().Log($"Platform: {PlatformHelper.Platform}");

		ServiceManager.GetService<IPackageService>().RegisterPackage(NetlistViewerBackendPackage);
		ServiceManager.GetService<IPackageService>().RegisterPackage(JREPackage);

		ServiceManager.GetCustomLogger().Log("Registered Packages");

		var resourceInclude = new ResourceInclude(new Uri("avares://FEntwumS.NetlistViewer/Styles/Icons.axaml"))
			{ Source = new Uri("avares://FEntwumS.NetlistViewer/Styles/Icons.axaml") };

		Application.Current?.Resources.MergedDictionaries.Add(resourceInclude);

		ISettingsService settingsService = ServiceManager.GetService<ISettingsService>();

		ServiceManager.GetService<IDockService>().RegisterLayoutExtension<FrontendViewModel>(DockShowLocation.Document);
		ServiceManager.GetService<IDockService>()
			.RegisterLayoutExtension<HierarchySidebarViewModel>(DockShowLocation.Left);

		ServiceManager.GetCustomLogger().Log("Registered FrontendViewModel as Document in dock system");
		
		RegisterContextMenus();
		RegisterSettings();
		RegisterProjectSettings();
		SubscribeToSettings();
		RegisterShutdownActions();
		
		// Subscribe to the setting that enables/disables the hierarchy viewer. The value is used to determine whether
		// the context menu option for the hierarchy viewer is to be shown to the user 
		
		settingsService.GetSettingObservable<bool>(FentwumSNetlistViewerSettingsHelper.EnableHierarchyViewKey)
			.Subscribe(x => EnableHierarchyView = x);

		// Upgrade settings, if necessary
		if (SettingsUpgrader.NeedsUpgrade())
		{
			ServiceManager.GetCustomLogger().Log("Upgrading settings");
			_ = SettingsUpgrader.UpgradeSettingsIfNecessaryAsync();
		}
	}

	private void RegisterContextMenus()
	{
		ServiceManager.GetService<IProjectExplorerService>().RegisterConstructContextMenu((selected, menuItems) =>
		{
			// JSON netlists can be directly passed to the ShowViewer() function. The backend determines whether it is a
			// flattened or a hierarchical netlist. For the supported HDLs, the netlist is first retrieved or generated 
			
			if (selected is [IProjectFile { Extension: ".json" } jsonFile])
			{
				menuItems.Add(new MenuItemViewModel("NetlistViewer")
				{
					Header = $"View netlist {jsonFile.Header}",
					Command = new AsyncRelayCommand(() => ServiceManager.GetService<FrontendService>().ShowViewerAsync(jsonFile))
				});
			}
			else if (selected is [IProjectFile { Extension: ".vhd" } vhdlFile])
			{
				menuItems.Add(new MenuItemViewModel("NetlistViewer_CreateNetlist")
				{
					Header = $"View netlist for {vhdlFile.Header}",
					Command = new AsyncRelayCommand(() => ServiceManager.GetService<FrontendService>().CreateVhdlNetlistAsync(vhdlFile))
				});
			}
			else if (selected is [IProjectFile { Extension: ".v" } verilogFile])
			{
				menuItems.Add(new MenuItemViewModel("NetlistViewer_CreateVerilogNetlist")
				{
					Header = $"View netlist for {verilogFile.Header}",
					Command = new AsyncRelayCommand(() => ServiceManager.GetService<FrontendService>().CreateVerilogNetlistAsync(verilogFile))
				});
			}
			else if (selected is [IProjectFile { Extension: ".sv" } systemVerilogFile])
			{
				menuItems.Add(new MenuItemViewModel("NetlistViewer_CreateSystemVerilogNetlist")
				{
					Header = $"View netlist for {systemVerilogFile.Header}",
					Command = new AsyncRelayCommand(() =>
						ServiceManager.GetService<FrontendService>().CreateSystemVerilogNetlistAsync(systemVerilogFile))
				});
			}
		});

		ServiceManager.GetService<IProjectExplorerService>().RegisterConstructContextMenu((selected, menuItems) =>
		{
			// Add no context menu entry for the hierarchy viewer if it has been disabled
			
			if (!EnableHierarchyView)
			{
				return;
			}

			if (selected is [IProjectFile { Extension: ".vhd" } vhdlFile])
			{
				menuItems.Add(new MenuItemViewModel("NetlistViewer_VHDLHierarchy")
				{
					Header = $"View design hierarchy for {vhdlFile.Header}",
					Command = new AsyncRelayCommand(() => ServiceManager.GetService<FrontendService>().CreateVhdlHierarchyAsync(vhdlFile))
				});
			}
			else if (selected is [IProjectFile { Extension: ".v" } verilogFile])
			{
				menuItems.Add(new MenuItemViewModel("NetlistViewer_VerilogHierarchy")
				{
					Header = $"View design hierarchy for {verilogFile.Header}",
					Command = new AsyncRelayCommand(() => ServiceManager.GetService<FrontendService>().CreateVerilogHierarchyAsync(verilogFile))
				});
			}
			else if (selected is [IProjectFile { Extension: ".sv" } systemVerilogFile])
			{
				menuItems.Add(new MenuItemViewModel("NetlistViewer_SystemVerilogHierarchy")
				{
					Header = $"View design hierarchy for {systemVerilogFile.Header}",
					Command = new AsyncRelayCommand(() =>
						ServiceManager.GetService<FrontendService>().CreateSystemVerilogHierarchyAsync(systemVerilogFile))
				});
			}
		});

		ServiceManager.GetCustomLogger().Log("Registered custom context menu entries");
	}
	
	private void RegisterSettings()
	{
		ServiceManager.GetService<ISettingsService>().RegisterSettingCategory("Netlist Viewer", 100, "netlistIcon");
		
		ServiceManager.GetService<ISettingsService>().RegisterSettingSubCategory("Netlist Viewer", "Backend");
		
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Backend",
			FentwumSNetlistViewerSettingsHelper.NetlistPathSettingKey,
			new FolderPathSetting("Path to folder containing server jar", "fentwums-netlist-reader", "",
				FentwumSNetlistViewerSettingsHelper.NetlistPathSettingKey, Path.Exists));

		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Backend",
			FentwumSNetlistViewerSettingsHelper.JavaPathSettingKey,
			new FolderPathSetting("Path to folder containing java binary", "", "",
				FentwumSNetlistViewerSettingsHelper.JavaPathSettingKey,
				Path.Exists));

		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Backend",
			FentwumSNetlistViewerSettingsHelper.JavaArgsKey,
			new TextBoxSetting("Extra arguments for the Java Virtual Machine", "-Xmx16G -XX:+UseZGC -XX:+ZGenerational",
				"null"));

		ServiceManager.GetService<ISettingsService>().RegisterSettingSubCategory("Netlist Viewer", "FPGA");

		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "FPGA",
			FentwumSNetlistViewerSettingsHelper.FpgaManufacturerKey,
			new ComboBoxSetting("FPGA manufacturer", "gatemate",
			[
				"achronix", "anlogic", "coolrunner2", "ecp5", "efinix", "fabulous", "gatemate", "gowin", "greenpak4",
				"ice40", "intel", "intel_alm", "lattice", "microchip", "nanoxplore", "nexus", "quicklogic", "sf2",
				"xilinx"
			]));

		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "FPGA",
			FentwumSNetlistViewerSettingsHelper.FpgaDeviceFamilyKey,
			new TextBoxSetting("Device family", "", null));

		ServiceManager.GetService<ISettingsService>().RegisterSettingSubCategory("Netlist Viewer", "Backend");

		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Backend",
			FentwumSNetlistViewerSettingsHelper.BackendAddressKey,
			new TextBoxSetting("Server address", "127.0.0.1", null));
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Backend", FentwumSNetlistViewerSettingsHelper.BackendPortKey,
			new TextBoxSetting("Port", "8080", null));
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Backend",
			FentwumSNetlistViewerSettingsHelper.BackendRequestTimeoutKey,
			new TextBoxSetting("Request Timeout (in seconds)", "8000", null));
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Backend",
			FentwumSNetlistViewerSettingsHelper.BackendUseLocalKey,
			new CheckBoxSetting("Use local backend server", true));

		ServiceManager.GetService<ISettingsService>().RegisterSettingSubCategory("Netlist Viewer", "Font sizes");

		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Font sizes",
			FentwumSNetlistViewerSettingsHelper.EntityFontSizeKey,
			new TextBoxSetting("Entity Label Font Size", "25", null));
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Font sizes",
			FentwumSNetlistViewerSettingsHelper.CellFontSizeKey,
			new TextBoxSetting("Cell Label Font Size", "15", null));
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Font sizes",
			FentwumSNetlistViewerSettingsHelper.EdgeFontSizeKey,
			new TextBoxSetting("Edge Label Font Size", "10", null));
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Font sizes",
			FentwumSNetlistViewerSettingsHelper.PortFontSizeKey,
			new TextBoxSetting("Port Font Size", "10", null));

		ServiceManager.GetService<ISettingsService>().RegisterSettingSubCategory("Netlist Viewer", "Experimental");

		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Experimental",
			FentwumSNetlistViewerSettingsHelper.ContinueOnBinaryInstallErrorKey,
			new CheckBoxSetting("Continue if errors occur during dependency installation", false));
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Experimental",
			FentwumSNetlistViewerSettingsHelper.UseHierarchicalBackendKey,
			new CheckBoxSetting("Use hierarchical backend", true));
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Experimental",
			FentwumSNetlistViewerSettingsHelper.PerformanceTargetKey,
			new ComboBoxSetting("Performance Target", "Intelligent Ahead Of Time",
				["Preloading", "Just In Time", "Intelligent Ahead Of Time"]));
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Experimental",
			FentwumSNetlistViewerSettingsHelper.AlwaysRegenerateNetlistsKey,
			new CheckBoxSetting("Always regenerate netlists", false));
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Experimental",
			FentwumSNetlistViewerSettingsHelper.EnableHierarchyViewKey,
			new CheckBoxSetting("Enable hierarchy view", true));
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Experimental",
			FentwumSNetlistViewerSettingsHelper.AutomaticNetlistGenerationKey,
			new ComboBoxSetting("Automatic netlist generation", "Never", ["Never", "Always", "Interval"]));
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Experimental",
			FentwumSNetlistViewerSettingsHelper.AutomaticNetlistGenerationIntervalKey,
			new SliderSetting("Automatic netlist generation interval (s)", 60.0d, 15.0d, 3600.0d, 5.0d));

		ServiceManager.GetCustomLogger().Log("Registered custom settings");
	}

	private void RegisterProjectSettings()
	{
		IProjectSettingsService projectSettingsService = ServiceManager.GetService<IProjectSettingsService>();

		projectSettingsService.AddProjectSetting(new ProjectSettingBuilder()
			.WithKey(FentwumSNetlistViewerSettingsHelper.ProjectFpgaManufacturerKey)
			.WithDisplayOrder(1000)
			.WithSetting(new ComboBoxSetting("FPGA Manufacturer",
				"gatemate",
				[
					"achronix", "anlogic", "coolrunner2", "ecp5", "efinix", "fabulous", "gatemate", "gowin",
					"greenpak4",
					"ice40", "intel", "intel_alm", "lattice", "microchip", "nanoxplore", "nexus", "quicklogic", "sf2",
					"xilinx"
				]))
			.Build());
		
		projectSettingsService.AddProjectSetting(new ProjectSettingBuilder()
			.WithKey(FentwumSNetlistViewerSettingsHelper.ProjectFpgaDeviceFamilyKey)
			.WithDisplayOrder(1001)
			.WithSetting(new TextBoxSetting("Device family", "", null))
		.Build());

		ServiceManager.GetCustomLogger().Log("Added project-specific settings");
	}

	private void SubscribeToSettings()
	{
		// Subscribe the FrontendService _AFTER_ the relevant settings have been registered
		ServiceManager.GetService<FrontendService>().SubscribeToSettings();
		ServiceManager.GetService<IFpgaBbService>().SubscribeToSettings();
		ServiceManager.GetService<IYosysService>().SubscribeToSettings();
		ServiceManager.GetService<INetlistGenerator>().SubscribeToSettings();

		ServiceManager.GetCustomLogger().Log("FEntwumS.NetlistViewer: Subscribed services to the settings relevant to them");
	}

	private void RegisterShutdownActions()
	{
		// Since the backend sometimes is not stopped when OneWare Studio is closed, the following shutdown action is
		// registered. It sends an explicit request to shut down the server. The result and any errors are deliberately
		// ignored. This request is only sent if a local backend is used to not inconvenience users of a shared
		// remote backend 
		
		ServiceManager.GetService<IApplicationStateService>().RegisterShutdownAction(() =>
		{
			try
			{
				if (ServiceManager.GetService<ISettingsService>()
				    .GetSettingValue<bool>(FentwumSNetlistViewerSettingsHelper.BackendUseLocalKey))
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
			}
			catch (Exception)
			{
				// ignored
			}
		});
	}
}