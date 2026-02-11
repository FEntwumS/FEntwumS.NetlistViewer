using System.Net.Http.Headers;
using System.Reflection;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using CommunityToolkit.Mvvm.Input;
using FEntwumS.NetlistViewer.Helpers;
using FEntwumS.NetlistViewer.Helpers.Validators;
using FEntwumS.NetlistViewer.Services;
using FEntwumS.NetlistViewer.ViewModels;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Helpers;
using OneWare.Essentials.Models;
using OneWare.Essentials.PackageManager;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using OneWare.UniversalFpgaProjectSystem.Services;

namespace FEntwumS.NetlistViewer;

public class FEntwumSNetlistReaderFrontendModule : OneWareModuleBase
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
				ContentUrl =
					"https://raw.githubusercontent.com/FEntwumS/NetlistReaderBackend/refs/heads/master/README.md"
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
			},
			new PackageVersion()
			{
				Version = "0.11.4",
				Targets =
				[
					new PackageTarget()
					{
						Target = "all",
						Url =
							"https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.11.4/fentwums-netlist-reader-server-v0.11.4.tar.gz",
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
				Version = "0.11.5",
				Targets =
				[
					new PackageTarget()
					{
						Target = "all",
						Url =
							"https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.11.5/fentwums-netlist-reader-server-v0.11.5.tar.gz",
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
				Version = "0.11.6",
				Targets =
				[
					new PackageTarget()
					{
						Target = "all",
						Url =
							"https://github.com/FEntwumS/NetlistReaderBackend/releases/download/v0.11.6/fentwums-netlist-reader-server-v0.11.6.tar.gz",
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

	public override void RegisterServices(IServiceCollection containerRegistry)
	{
		containerRegistry.AddSingleton<IViewportDimensionService, ViewportDimensionService>();
		containerRegistry.AddSingleton<IJsonLoader, JsonLoader>();
		containerRegistry.AddSingleton<IHashService, OAATHashService>();
		containerRegistry.AddSingleton<IYosysService, YosysService>();
		containerRegistry.AddSingleton<IToolExecuterService, ToolExecuterService>();
		containerRegistry.AddSingleton<IFpgaBbService, FpgaBbService>();
		containerRegistry.AddSingleton<ICcVhdlFileIndexService, CcVhdlFileIndexService>();
		containerRegistry.AddSingleton<IFrontendService, FrontendService>();
		containerRegistry.AddSingleton<INetlistGenerator, NetlistGenerator>();
		containerRegistry.AddSingleton<IHierarchyJsonParser, HierarchyJsonParser>();
		containerRegistry.AddSingleton<IHierarchyInformationService, HierarchyInformationService>();
		containerRegistry.AddSingleton<IStorageService, StorageService>();
	}

	public override void Initialize(IServiceProvider containerProvider)
	{
		ILogger logger = ServiceManager.GetService<ILogger>();

		// Log some debug information
		ServiceManager.GetService<ILogger>().Log($"Platform: {PlatformHelper.Platform}");

		ServiceManager.GetService<IPackageService>().RegisterPackage(NetlistViewerBackendPackage);
		ServiceManager.GetService<IPackageService>().RegisterPackage(JREPackage);

		ServiceManager.GetService<ILogger>().Log("Registered Packages");

		var resourceInclude = new ResourceInclude(new Uri("avares://FEntwumS.NetlistViewer/Styles/Icons.axaml"))
			{ Source = new Uri("avares://FEntwumS.NetlistViewer/Styles/Icons.axaml") };

		Application.Current?.Resources.MergedDictionaries.Add(resourceInclude);

		ISettingsService settingsService = ServiceManager.GetService<ISettingsService>();

		ServiceManager.GetService<IMainDockService>()
			.RegisterLayoutExtension<FrontendViewModel>(DockShowLocation.Document);
		ServiceManager.GetService<IMainDockService>()
			.RegisterLayoutExtension<HierarchySidebarViewModel>(DockShowLocation.Left);

		ServiceManager.GetService<ILogger>().Log("Registered FrontendViewModel as Document in dock system");

		RegisterContextMenus();
		RegisterSettings();
		RegisterProjectSettings();
		RegisterMigrations();
		SubscribeToSettings();
		RegisterShutdownActions();

		// Subscribe to the setting that enables/disables the hierarchy viewer. The value is used to determine whether
		// the context menu option for the hierarchy viewer is to be shown to the user 

		settingsService.GetSettingObservable<bool>(FentwumSNetlistViewerSettingsHelper.EnableHierarchyViewKey)
			.Subscribe(x => EnableHierarchyView = x);

		// Upgrade settings, if necessary
		if (SettingsUpgrader.NeedsUpgrade())
		{
			ServiceManager.GetService<ILogger>().Log("Upgrading settings");
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
				menuItems.Add(new MenuItemModel("NetlistViewer")
				{
					Header = $"View netlist {jsonFile.Header}",
					Command = new AsyncRelayCommand(() =>
						ServiceManager.GetService<FrontendService>().ShowViewerAsync(jsonFile))
				});
			}
			else if (selected is [IProjectFile { Extension: ".vhd" } vhdlFile])
			{
				menuItems.Add(new MenuItemModel("NetlistViewer_CreateNetlist")
				{
					Header = $"View netlist for {vhdlFile.Header}",
					Command = new AsyncRelayCommand(() =>
						ServiceManager.GetService<FrontendService>().CreateVhdlNetlistAsync(vhdlFile))
				});
			}
			else if (selected is [IProjectFile { Extension: ".v" } verilogFile])
			{
				menuItems.Add(new MenuItemModel("NetlistViewer_CreateVerilogNetlist")
				{
					Header = $"View netlist for {verilogFile.Header}",
					Command = new AsyncRelayCommand(() =>
						ServiceManager.GetService<FrontendService>().CreateVerilogNetlistAsync(verilogFile))
				});
			}
			else if (selected is [IProjectFile { Extension: ".sv" } systemVerilogFile])
			{
				menuItems.Add(new MenuItemModel("NetlistViewer_CreateSystemVerilogNetlist")
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
				menuItems.Add(new MenuItemModel("NetlistViewer_VHDLHierarchy")
				{
					Header = $"View design hierarchy for {vhdlFile.Header}",
					Command = new AsyncRelayCommand(() =>
						ServiceManager.GetService<FrontendService>().CreateVhdlHierarchyAsync(vhdlFile))
				});
			}
			else if (selected is [IProjectFile { Extension: ".v" } verilogFile])
			{
				menuItems.Add(new MenuItemModel("NetlistViewer_VerilogHierarchy")
				{
					Header = $"View design hierarchy for {verilogFile.Header}",
					Command = new AsyncRelayCommand(() =>
						ServiceManager.GetService<FrontendService>().CreateVerilogHierarchyAsync(verilogFile))
				});
			}
			else if (selected is [IProjectFile { Extension: ".sv" } systemVerilogFile])
			{
				menuItems.Add(new MenuItemModel("NetlistViewer_SystemVerilogHierarchy")
				{
					Header = $"View design hierarchy for {systemVerilogFile.Header}",
					Command = new AsyncRelayCommand(() =>
						ServiceManager.GetService<FrontendService>()
							.CreateSystemVerilogHierarchyAsync(systemVerilogFile))
				});
			}
		});

		ServiceManager.GetService<ILogger>().Log("Registered custom context menu entries");
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
				FentwumSNetlistViewerSettingsHelper.FpgaManufacturers.ToArray<object>()));

		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "FPGA",
			FentwumSNetlistViewerSettingsHelper.FpgaDeviceFamilyKey,
			new TextBoxSetting("Device family", "", null)
			{
				Validator = new GlobalFpgaDeviceFamilyValidator()
			});

		ServiceManager.GetService<ISettingsService>().RegisterSettingSubCategory("Netlist Viewer", "Backend");

		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Backend",
			FentwumSNetlistViewerSettingsHelper.BackendAddressKey,
			new TextBoxSetting("Server address", "127.0.0.1", null)
			{
				Validator = new BackendAddressValidator()
			});

		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Backend",
			FentwumSNetlistViewerSettingsHelper.BackendPortKey,
			new TextBoxSetting("Port", "8080", null)
			{
				Validator = new BackendPortValidator()
			});
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Backend",
			FentwumSNetlistViewerSettingsHelper.BackendRequestTimeoutKey,
			new TextBoxSetting("Request Timeout (in seconds)", "8000", null)
			{
				Validator = new RequestTimeoutValidator()
			});
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Backend",
			FentwumSNetlistViewerSettingsHelper.BackendUseLocalKey,
			new CheckBoxSetting("Use local backend server", true));

		ServiceManager.GetService<ISettingsService>().RegisterSettingSubCategory("Netlist Viewer", "Font sizes");

		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Font sizes",
			FentwumSNetlistViewerSettingsHelper.EntityFontSizeKey,
			new TextBoxSetting("Entity Label Font Size", "25", null)
			{
				Validator = new FontSizeValidator()
			});
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Font sizes",
			FentwumSNetlistViewerSettingsHelper.CellFontSizeKey,
			new TextBoxSetting("Cell Label Font Size", "15", null)
			{
				Validator = new FontSizeValidator()
			});
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Font sizes",
			FentwumSNetlistViewerSettingsHelper.EdgeFontSizeKey,
			new TextBoxSetting("Edge Label Font Size", "10", null)
			{
				Validator = new FontSizeValidator()
			});
		ServiceManager.GetService<ISettingsService>().RegisterSetting("Netlist Viewer", "Font sizes",
			FentwumSNetlistViewerSettingsHelper.PortFontSizeKey,
			new TextBoxSetting("Port Font Size", "10", null)
			{
				Validator = new FontSizeValidator()
			});

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

		ServiceManager.GetService<ILogger>().Log("Registered custom settings");
	}

	private void RegisterProjectSettings()
	{
		IProjectSettingsService projectSettingsService = ServiceManager.GetService<IProjectSettingsService>();

		projectSettingsService.AddProjectSetting(new ProjectSettingBuilder()
			.WithKey(FentwumSNetlistViewerSettingsHelper.ProjectFpgaManufacturerKey)
			.WithDisplayOrder(1000)
			.WithSetting(new ComboBoxSetting("FPGA Manufacturer",
				"gatemate",
				FentwumSNetlistViewerSettingsHelper.FpgaManufacturers.ToArray<object>()))
			.Build());

		projectSettingsService.AddProjectSetting(new ProjectSettingBuilder()
			.WithKey(FentwumSNetlistViewerSettingsHelper.ProjectFpgaDeviceFamilyKey)
			.WithDisplayOrder(1001)
			.WithSetting(new TextBoxSetting("Device family", "", null)
			{
				Validator = new ProjectFpgaDeviceFamilyValidator()
			})
			.Build());

		ServiceManager.GetService<ILogger>().Log("Added project-specific settings");
	}

	/// <summary>
	/// This method automatically finds all interfaces inheriting from the ISettingsSubscriber interface. The
	/// ISettingsSubscriber.SubscribeToSettings() method is called on the implementations of these interfaces. Thereby
	/// all services that need to subscribe to some setting are automatically subscribed without them being needed to be
	/// manually subscribed 
	/// </summary>
	private void SubscribeToSettings()
	{
		// Inspired by: https://stackoverflow.com/a/26745

		Type settingsSubscriberType = typeof(ISettingsSubscriber);

		foreach (Assembly asmbly in AppDomain.CurrentDomain.GetAssemblies())
		{
			try
			{
				foreach (Type type in asmbly.GetTypes())
				{
					if (settingsSubscriberType.IsAssignableFrom(type) && type is { IsInterface: true } &&
					    settingsSubscriberType != type)
					{
						(ContainerLocator.Current.Resolve(type) as ISettingsSubscriber)?.SubscribeToSettings();

						ServiceManager.GetService<ILogger>().Log($"Subscribed {type.FullName} to settings");
					}
				}
			}
			catch (ReflectionTypeLoadException ex)
			{
				ServiceManager.GetService<ILogger>()
					.Error(
						"[FEntwumS.NetlistViewer]: An issue occured during settings subscription\n\nPlease file a bug report!",
						ex);
			}
		}

		ServiceManager.GetService<ILogger>()
			.Log("FEntwumS.NetlistViewer: Subscribed services to the settings relevant to them");
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

	private void RegisterMigrations()
	{
		ServiceManager.GetService<FpgaService>().RegisterProjectPropertyMigration(
			FentwumSNetlistViewerSettingsHelper.ProjectFpgaManufacturerKey, "FEntwumS_FPGA_Manufacturer");
		ServiceManager.GetService<FpgaService>().RegisterProjectPropertyMigration(
			FentwumSNetlistViewerSettingsHelper.ProjectFpgaDeviceFamilyKey, "FEntwumS_FPGA_DeviceFamily");
	}
}