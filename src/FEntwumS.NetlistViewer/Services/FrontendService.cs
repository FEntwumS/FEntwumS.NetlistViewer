using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Asmichi.ProcessManagement;
using FEntwumS.NetlistViewer.Types.HierarchyView;
using FEntwumS.NetlistViewer.Types;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using FEntwumS.NetlistViewer.ViewModels;
using FEntwumS.NetlistViewer.Views;
using OneWare.Essentials.Helpers;
using OneWare.Essentials.PackageManager;
using OneWare.ProjectSystem.Models;
using StreamContent = System.Net.Http.StreamContent;

namespace FEntwumS.NetlistViewer.Services;

public class FrontendService : IFrontendService
{
    private static readonly ICustomLogger _logger;
    private static readonly IApplicationStateService _applicationStateService;
    private static readonly IDockService _dockService;
    private static readonly ISettingsService _settingsService;
    private static readonly IPackageService _packageService;
    private static readonly IHierarchyJsonParser _hierarchyJsonParser;
    private static readonly INetlistGenerator _netlistGenerator;

    private static string _backendAddress = string.Empty;
    private static string _backendPort = string.Empty;
    private static bool _useLocalBackend = false;
    private static int _requestTimeout = 600;
    private static string _backendJarFolder = string.Empty;
    private static int _entityLabelFontSize = 25;
    private static int _cellLabelFontSize = 15;
    private static int _edgeLabelFontSize = 10;
    private static int _portLabelFontSize = 10;
    private static string _javaBinaryFolder = string.Empty;
    private static string _extraJarArgs = string.Empty;
    private static bool _continueOnBinaryInstallError = false;
    private static string _performanceTarget = string.Empty;

    private static bool _restartRequired = false;

    private UInt64 currentNetlist = 0;

    // Used to store the process handle of the backend process. If the handle is not stored, the process will be
    // automatically terminated
    private static IChildProcess? backendProcess;

    static FrontendService()
    {
        _logger = ServiceManager.GetCustomLogger();
        _applicationStateService = ServiceManager.GetService<IApplicationStateService>();
        _dockService = ServiceManager.GetService<IDockService>();
        _settingsService = ServiceManager.GetService<ISettingsService>();
        _packageService = ServiceManager.GetService<IPackageService>();
        _hierarchyJsonParser = ServiceManager.GetService<IHierarchyJsonParser>();
        _netlistGenerator = ServiceManager.GetService<INetlistGenerator>();
    }

    public void SubscribeToSettings()
    {
        _settingsService.GetSettingObservable<string>("NetlistViewer_Backend_Address").Subscribe(x =>
        {
            if (isAddressValid(x))
            {
                _logger.Log($"New address: {x}", _backendAddress != string.Empty);
                _backendAddress = x;
            }
            else
            {
                _logger.Error($"{x} is not a valid address");
            }
        });

        _settingsService.GetSettingObservable<string>("NetlistViewer_Backend_Port").Subscribe(x =>
        {
            if (isPortValid(x))
            {
                _logger.Log($"New port: {x}", _backendPort != string.Empty);
                _backendPort = x;
            }
            else
            {
                _logger.Error($"{x} is not a valid port");
            }
        });

        _settingsService.GetSettingObservable<bool>("NetlistViewer_Backend_UseLocal")
            .Subscribe(x =>
            {
                _useLocalBackend = x;
                if (_useLocalBackend)
                {
                    ServiceManager.GetService<ISettingsService>()
                        .SetSettingValue("NetlistViewer_Backend_Address", "127.0.0.1");
                    ServiceManager.GetService<ISettingsService>()
                        .SetSettingValue("NetlistViewer_Backend_Port", "8080");
                }
            });
        _settingsService.GetSettingObservable<string>("NetlistViewer_Backend_RequestTimeout").Subscribe(x =>
        {
            try
            {
                _requestTimeout = int.Parse(x);

                if (_requestTimeout <= 0)
                {
                    _logger.Error("Request timeout not valid. Please enter a positive integer");

                    _requestTimeout = 600;
                }
            }
            catch (Exception)
            {
                _logger.Error("Request timeout not valid. Please enter a positive integer");

                _requestTimeout = 600;
            }
        });

        _settingsService.GetSettingObservable<string>(FEntwumSNetlistReaderFrontendModule.NetlistPathSetting)
            .Subscribe(x => _backendJarFolder = x);

        _settingsService.GetSettingObservable<string>("NetlistViewer_EntityFontSize").Subscribe(x =>
        {
            try
            {
                _entityLabelFontSize = int.Parse(x);

                if (_entityLabelFontSize <= 0)
                {
                    _logger.Error("Entity label font size not valid. Please enter a positive integer");
                    _entityLabelFontSize = 25;
                }
            }
            catch (Exception)
            {
                _logger.Error("Entity label font size not valid. Please enter a positive integer");

                _entityLabelFontSize = 25;
            }
        });

        _settingsService.GetSettingObservable<string>("NetlistViewer_CellFontSize").Subscribe(x =>
        {
            try
            {
                _cellLabelFontSize = int.Parse(x);

                if (_cellLabelFontSize <= 0)
                {
                    _logger.Error("Cell label font size not valid. Please enter a positive integer");

                    _cellLabelFontSize = 15;
                }
            }
            catch (Exception)
            {
                _logger.Error("Cell label font size not valid. Please enter a positive integer");

                _cellLabelFontSize = 15;
            }
        });

        _settingsService.GetSettingObservable<string>("NetlistViewer_EdgeFontSize").Subscribe(x =>
        {
            try
            {
                _edgeLabelFontSize = int.Parse(x);

                if (_edgeLabelFontSize <= 0)
                {
                    _logger.Error("Edge label font size not valid. Please enter a positive integer");

                    _edgeLabelFontSize = 10;
                }
            }
            catch (Exception)
            {
                _logger.Error("Edge label font size not valid. Please enter a positive integer");

                _edgeLabelFontSize = 10;
            }
        });

        _settingsService.GetSettingObservable<string>("NetlistViewer_PortFontSize").Subscribe(x =>
        {
            try
            {
                _portLabelFontSize = int.Parse(x);

                if (_portLabelFontSize <= 0)
                {
                    _logger.Error("Port label font size not valid. Please enter a positive integer");

                    _portLabelFontSize = 10;
                }
            }
            catch (Exception)
            {
                _logger.Error("Port label font size not valid. Please enter a positive integer");

                _portLabelFontSize = 10;
            }
        });

        _settingsService.GetSettingObservable<string>(FEntwumSNetlistReaderFrontendModule.JavaPathSetting).Subscribe(
            x => _javaBinaryFolder = x);

        _settingsService.GetSettingObservable<string>("NetlistViewer_java_args").Subscribe(x => _extraJarArgs = x);

        _settingsService.GetSettingObservable<bool>("NetlistViewer_ContinueOnBinaryInstallError")
            .Subscribe(x => _continueOnBinaryInstallError = x);

        _settingsService.GetSettingObservable<string>("NetlistViewer_PerformanceTarget")
            .Subscribe(x => _performanceTarget = x switch
            {
                "Preloading" => "Preloading",
                "Just In Time" => "JustInTime",
                "Intelligent Ahead Of Time" => "IntelligentAheadOfTime",
                _ => "Preloading"
            });
    }

    private async Task<(bool success, bool needsRestart)> InstallDependenciesAsync()
    {
        ApplicationProcess checkProc = _applicationStateService.AddState("Checking dependencies", AppState.Loading);

        bool globalSuccess = true, needsRestart = false;

        (string id, Version minversion)[] dependencyIDs = new (string, Version)
            []
            {
                ("OneWare.GhdlExtension", new Version(0, 10, 7)),
                ("osscadsuite", new Version(2025, 01, 21)),
                ("ghdl", new Version(5, 0, 1)),
                (FEntwumSNetlistReaderFrontendModule.NetlistPackage.Id!, new Version(0, 8, 1)),
                (FEntwumSNetlistReaderFrontendModule.JDKPackage.Id!, new Version(21, 0, 6))
            };

        // Install osscadsuite binary between GHDL plugin and ghdl binary to allow for the addition of the ghdl binary to the store

        foreach ((string dependencyID, Version minVersion) in dependencyIDs)
        {
            PackageModel? dependencyModel = _packageService.Packages.GetValueOrDefault(dependencyID);
            Package? dependencyPackage = dependencyModel?.Package;

            if (dependencyPackage == null)
            {
                _logger.Error(
                    $"Dependency with ID {dependencyID} is not available in the package manager. Please file a bug report, if this issue persists");

                globalSuccess = false;
                continue;
            }

            if (_packageService.Packages!.GetValueOrDefault(dependencyID) is
                {
                    Status: PackageStatus.Available
                })
            {
                bool updatePerformed = true;

                if (_settingsService.GetSettingValue<bool>("Experimental_AutoDownloadBinaries"))
                {
                    _logger.Log($"Installing \"{dependencyPackage.Name}\"...", true);

                    bool localSuccess = false;

                    // Try to install the dependency, starting with the latest version
                    // If the version is not compatible or the download fails, try the previous version
                    foreach (PackageVersion packageVersion in dependencyPackage.Versions!.Reverse())
                    {
                        // Skip incompatible versions
                        if (!(await dependencyModel!.CheckCompatibilityAsync(packageVersion)).IsCompatible)
                        {
                            continue;
                        }

                        PackageVersion? installedVersion = dependencyModel.InstalledVersion;

                        if (installedVersion == packageVersion)
                        {
                            _logger.Log(
                                $"Failed to update {dependencyPackage.Name} from version {installedVersion.Version} to version {dependencyPackage.Versions!.Last()}",
                                true);

                            updatePerformed = false;
                            localSuccess = true;
                            break;
                        }

                        localSuccess = await dependencyModel!.DownloadAsync(packageVersion);

                        // Stop trying, if install has been successful
                        if (localSuccess)
                        {
                            break;
                        }
                    }

                    globalSuccess = globalSuccess && localSuccess;

                    if (updatePerformed)
                    {
                        if (localSuccess)
                        {
                            _logger.Log($"Successfully installed \"{dependencyPackage.Name}\".", true);
                        }
                        else
                        {
                            _logger.Error($"Failed to install \"{dependencyPackage.Name}\".");
                        }
                    }
                }
                else
                {
                    _logger.Error(
                        $"Extension \"{dependencyPackage.Name}\" is not installed. Please enable \"Automatically download Binaries\" under the \"Experimental\" settings or download the extension yourself");

                    globalSuccess = false;
                }

                if (globalSuccess && updatePerformed)
                {
                    needsRestart = true;
                    _restartRequired = true;
                }
            }

            if (dependencyModel!.Status is PackageStatus.Installed or PackageStatus.UpdateAvailable)
            {
                if (minVersion.CompareTo(Version.Parse(dependencyModel.InstalledVersion!.Version!)) <= 0)
                {
                    _logger.Log(
                        $"Dependency {dependencyPackage.Id} installed with version {dependencyModel.InstalledVersion.Version} greater than or equal to expected version {minVersion.ToString()}");
                }
                else
                {
                    _logger.Error(
                        $"Installed version {dependencyModel.InstalledVersion.Version} for {dependencyPackage.Name} is below the minimum version {minVersion.ToString()}. Please update {dependencyPackage.Name}!");

                    globalSuccess = false;
                }
            }
        }

        if (globalSuccess && (needsRestart || _restartRequired))
        {
            _logger.Log("Dependencies were successfully installed. Please restart OneWare Studio!", true);
        }

        _applicationStateService.RemoveState(checkProc);

        return (globalSuccess, needsRestart || _restartRequired);
    }

    private async Task<IProjectFile?> GenerateNetlistAsync(IProjectFile projectFile, NetlistType netlistType)
    {
        string netlistTypeString  = netlistType switch
        {
            NetlistType.VHDL => "VHDL",
            NetlistType.Verilog => "Verilog",
            NetlistType.System_Verilog => "System Verilog",
            _ => ""
        };

        ApplicationProcess proc = _applicationStateService.AddState($"Visualizing {netlistTypeString} netlist", AppState.Loading);

        (bool success, bool needsRestart) = await InstallDependenciesAsync();

        if (needsRestart)
        {
            _applicationStateService.RemoveState(proc, "Please restart OneWare Studio!");

            return null;
        }
        else if (!(success || _continueOnBinaryInstallError))
        {
            _applicationStateService.RemoveState(proc, "An error occured during dependency installation/checking");

            return null;
        }

        success = await StartBackendIfNotStartedAsync();

        if (!success)
        {
            _applicationStateService.RemoveState(proc, "Error: The backend could not be started");

            return null;
        }
        
        (IProjectFile? netlistFile, success) = await _netlistGenerator.GenerateNetlistAsync(projectFile, netlistType);

        if (!success)
        {
            _applicationStateService.RemoveState(proc, "Error: The netlist could not be generated");

            return null;
        }

        success = await ServerStartedAsync();

        if (!success)
        {
            _applicationStateService.RemoveState(proc, "Error: The backend could not be reached");

            return null;
        }
        
        _applicationStateService.RemoveState(proc);

        return netlistFile;
    }

    public async Task CreateVhdlNetlistAsync(IProjectFile vhdl)
    {
        await ShowViewerAsync((await GenerateNetlistAsync(vhdl, NetlistType.VHDL))!);
    }

    public async Task CreateVerilogNetlistAsync(IProjectFile verilog)
    {
        await ShowViewerAsync((await GenerateNetlistAsync(verilog, NetlistType.Verilog))!);
    }

    public async Task CreateSystemVerilogNetlistAsync(IProjectFile sVerilog)
    {
        await ShowViewerAsync((await GenerateNetlistAsync(sVerilog, NetlistType.System_Verilog))!);
    }

    public async Task ShowViewerAsync(IProjectFile json)
    {
        ApplicationProcess proc = _applicationStateService.AddState("Starting viewer", AppState.Loading);

        HttpResponseMessage? resp = null;
        string top = Path.GetFileNameWithoutExtension(json.FullPath);

        if (!File.Exists(json.FullPath))
        {
            _applicationStateService.RemoveState(proc, "Error: No JSON netlist was found");

            _logger.Log(
                "No json netlist was found. Aborting...");
            return;
        }

        string content = await File.ReadAllTextAsync(json.FullPath);

        IHashService hashService = ServiceManager.GetHashService();
        ReadOnlySpan<byte> contentByteSpan = new ReadOnlySpan<Byte>(Encoding.UTF8.GetBytes(content));
        ReadOnlySpan<byte> pathByteSpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(json.FullPath));
        UInt32 pathHash = hashService.ComputeHash(pathByteSpan);
        UInt32 contenthash = hashService.ComputeHash(contentByteSpan);

        UInt64 combinedHash = ((UInt64)pathHash) << 32 | contenthash;

        currentNetlist = combinedHash;

        ServiceManager.GetCustomLogger().Log("Path hash: " + pathHash);
        ServiceManager.GetCustomLogger().Log("Full file hash is: " + contenthash);
        ServiceManager.GetCustomLogger().Log("Combined hash is: " + combinedHash);

        ServiceManager.GetViewportDimensionService()!.SetClickedElementPath(combinedHash, string.Empty);
        ServiceManager.GetViewportDimensionService()!.SetCurrentElementCount(combinedHash, 0);
        ServiceManager.GetViewportDimensionService()!.SetZoomElementDimensions(combinedHash, null);

        FrontendViewModel vm = new FrontendViewModel();
        vm.InitializeContent();
        vm.Title = $"Netlist: {top}";
        _logger.Log("Selected file: " + json.FullPath);

        FileStream jsonFileStream = File.Open(json.FullPath, FileMode.Open, FileAccess.Read);

        MultipartFormDataContent formDataContent = new MultipartFormDataContent()
        {
            { new StreamContent(jsonFileStream), "file", json.Name }
        };

        ApplicationProcess waitForBackendProc =
            _applicationStateService.AddState("Layouting in progress", AppState.Loading);

        resp = await PostAsync(
            "/graphRemoteFile" + $"?hash={combinedHash}" + $"&entityLabelFontSize={_entityLabelFontSize}" +
            $"&cellLabelFontSize={_cellLabelFontSize}" + $"&edgeLabelFontSize={_edgeLabelFontSize}" +
            $"&portLabelFontSize={_portLabelFontSize}" + $"&performance-target={_performanceTarget}",
            formDataContent);

        _applicationStateService.RemoveState(waitForBackendProc);

        jsonFileStream.Close();

        if (resp == null)
        {
            _applicationStateService.RemoveState(proc, "Error: No response from backend");

            return;
        }

        vm.File = await resp.Content.ReadAsStreamAsync();

        if (!resp.IsSuccessStatusCode)
        {
            _applicationStateService.RemoveState(proc, "Error: The backend returned an error");

            return;
        }

        ApplicationProcess indexProc = _applicationStateService.AddState("Indexing", AppState.Loading);

        // create code index for cross-compiled VHDL
        string ccFile = Path.Combine(json.Root.FullPath, "build", "netlist", "design.v");

        if (File.Exists(ccFile))
        {
            bool success = await ServiceManager.GetService<ICcVhdlFileIndexService>()
                .IndexFileAsync(ccFile, combinedHash);

            if (success)
            {
                _logger.Log($"Successfully indexed {top}");
            }
            else
            {
                _logger.Log($"Failed to index {top}");
            }
        }

        _applicationStateService.RemoveState(indexProc);

        _dockService.Show(vm, DockShowLocation.Document);
        _dockService.InitializeContent();
        vm.NetlistId = currentNetlist;
        await vm.OpenFileImplAsync();

        _applicationStateService.RemoveState(proc);
    }

    public async Task ExpandNodeAsync(string? nodePath, FrontendViewModel vm)
    {
        ApplicationProcess expandProc = _applicationStateService.AddState("Layouting in progress", AppState.Loading);

        _logger.Log("Sending request to ExpandNode");

        var resp = await PostAsync("/expandNode?hash=" + vm.NetlistId + "&nodePath=" + nodePath, null);

        if (resp is not { IsSuccessStatusCode: true })
        {
            _applicationStateService.RemoveState(expandProc);

            return;
        }

        vm.File = await resp.Content.ReadAsStreamAsync();

        _logger.Log("Answer received");

        await vm.OpenFileImplAsync();

        _logger.Log("Done");

        _applicationStateService.RemoveState(expandProc);
    }

    // Source:
    // https://stackoverflow.com/a/36760050
    private static string IpV4AddressPattern = "^((25[0-5]|(2[0-4]|1[0-9]|[1-9]|)[0-9])(\\.(?!$)|$)){4}$";

    private bool isAddressValid(string address)
    {
        Match match;

        try
        {
            match = Regex.Match(address, IpV4AddressPattern);
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);

            return false;
        }

        return match.Success;
    }

    private bool isPortValid(string port)
    {
        try
        {
            int portInt = Convert.ToInt32(port);

            return portInt is >= 1024 and <= 65535;
        }
        catch (IOException e)
        {
            _logger.Error(e.Message, false);

            return false;
        }
    }

    private async Task<HttpResponseMessage?> PostAsync(string URI, HttpContent? content, bool printErrors = true)
    {
        HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.spring-boot.actuator.v3+json"));
        client.DefaultRequestHeaders.Add("User-Agent", "FEntwumS.NetlistViewer");
        client.BaseAddress = new Uri($"http://{_backendAddress}:{_backendPort}");
        client.Timeout = TimeSpan.FromSeconds(_requestTimeout);

        try
        {
            var resp = await client.PostAsync(URI, content);

            if (!resp.IsSuccessStatusCode)
            {
                if (resp.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.Error(
                        "The requested resource could not be found on the server. This could be due to a server restart. Please Re-Open your netlist.",
                        printErrors);
                }
                else
                {
                    _logger.Error(
                        "An internal server error occured. Please file a bug report if this problem persists.",
                        printErrors);
                }
            }

            return resp;
        }
        catch (InvalidOperationException)
        {
            _logger.Error(
                $"The server at {_backendAddress} could not be reached. Make sure the server is started and reachable under this address",
                printErrors);
            return null;
        }
        catch (HttpRequestException e)
        {
            switch (e.HttpRequestError)
            {
                case HttpRequestError.NameResolutionError:
                    _logger.Error(
                        $"The address {_backendAddress} could not be resolved. Make sure the server is started and reachable under this address",
                        printErrors);
                    break;

                case HttpRequestError.ConnectionError:
                    _logger.Error(
                        $"The address {_backendAddress} could not be reached. Make sure the server is started and reachable under this address",
                        printErrors);
                    break;

                default:
                    _logger.Error(
                        "Due to an internal error, the server could not complete the request. Please file a bug report",
                        printErrors);
                    break;
            }

            return null;
        }
        catch (TaskCanceledException)
        {
            _logger.Error(
                "The request has timed out. Please increase the request timeout time in the settings menu and try again",
                printErrors);
            return null;
        }
        catch (UriFormatException)
        {
            _logger.Error(
                $"The provided server address ${_backendAddress} is not a valid address. Please enter a correct IP address",
                printErrors);
            return null;
        }
    }

    public async Task<bool> StartBackendIfNotStartedAsync()
    {
        if (backendProcess != null)
        {
            return true;
        }

        HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.spring-boot.actuator.v3+json"));
        client.DefaultRequestHeaders.Add("User-Agent", "FEntwumS.NetlistViewer");
        client.BaseAddress = new Uri($"http://{_backendAddress}:{_backendPort}");
        client.Timeout = TimeSpan.FromSeconds(_requestTimeout);

        // First check if server is already started
        try
        {
            var res = await client.GetAsync("/server-active");
            _logger.Log("Server already started", true);

            return true;
        }
        catch (Exception)
        {
            // ignored
        }

        if (!_useLocalBackend)
        {
            _logger.Error(
                "The remote server could not be reached. Make sure the server is started and reachable or switch to the local server");
            return false;
        }

        _logger.Log("Server not started. Looking for server jar...", true);

        if (!Directory.Exists(_backendJarFolder))
        {
            _logger.Error(
                "The directory containing the server jar could not be found. Please make sure that you have installed the \"FEntwumS NetlistViewer Backend\" binary using the extension manager and set the correct path to the server jar");

            return false;
        }

        var serverJar = Directory.GetFiles(_backendJarFolder).Where(x =>
            Regex.Match(x, @".*fentwums-netlist-reader-server-(\d+\.)(\d+\.)(\d+)-exec\.jar").Success);

        var enumeratedResults = serverJar.ToList();
        if (enumeratedResults.Count == 0)
        {
            _logger.Error(
                "No jar found. Please make sure that you have installed the \"FEntwumS NetlistViewer Backend\" binary using the extension manager");

            return false;
        }

        _logger.Log("Found server jar. Looking for java binary...", true);

        if (!Directory.Exists(_javaBinaryFolder))
        {
            _logger.Error(
                "The directory containing the java executable could not be found. Please make sure you have installed the \"OpenJDK JDK\" binary using the extension manager");

            return false;
        }

        string prefix = string.Empty;
        string suffix = string.Empty;

        if (PlatformHelper.Platform is PlatformId.Unknown or PlatformId.Wasm)
        {
            _logger.Error("Your platform is currently not supported");

            return false;
        }
        else
        {
            var dir = Directory.GetDirectories(_javaBinaryFolder)
                .Where(x => Regex.Match(x, @"jdk-(\d+)\.(\d+)\.(\d+)\+(\d+)-jre").Success);

            if (dir.Count() == 0)
            {
                _logger.Error(
                    "No directory found. Please make sure that you have installed the \"Eclipse Adoptium OpenJDK\" binary using the extension manager");

                return false;
            }

            prefix = dir.First();
        }

        if (PlatformHelper.Platform is PlatformId.WinX64 or PlatformId.WinArm64)
        {
            suffix = ".exe";
        }
        else if (PlatformHelper.Platform is PlatformId.OsxX64 or PlatformId.OsxArm64)
        {
            prefix += "/Content/Home";
        }

        prefix += "/bin";

        string javaBinaryFile = Path.Combine(_javaBinaryFolder, $"{prefix}/java{suffix}");

        var serverJarFile = enumeratedResults.First();

        // Start server to run independently
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        backendProcess = ServiceManager.GetService<IToolExecuterService>()
            .ExecuteBackgroundProcess(javaBinaryFile,
                _extraJarArgs.Split(' ').Concat(["-jar", serverJarFile]).ToArray(),
                Path.GetDirectoryName(serverJarFile));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        _logger.Log("Server started", true);

        return true;
    }

    public async Task<bool> ServerStartedAsync()
    {
        const int timeBetweenRetriesMS = 100;
        const int retriesPerSecond = 1000 / timeBetweenRetriesMS;

        ApplicationProcess liveProc = _applicationStateService.AddState("Connecting to backend", AppState.Loading);

        HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.spring-boot.actuator.v3+json"));
        client.DefaultRequestHeaders.Add("User-Agent", "FEntwumS.NetlistViewer");
        client.BaseAddress = new Uri($"http://{_backendAddress}:{_backendPort}");
        client.Timeout = TimeSpan.FromMilliseconds(timeBetweenRetriesMS);
        bool done = false;

        int failures = 0;

        while (!done)
        {
            try
            {
                var res = await client.GetAsync("/server-active");
                _logger.Log("Server is awaiting requests");
                done = true;
            }
            catch (Exception)
            {
                if (!_useLocalBackend)
                {
                    _logger.Error(
                        "The remote server could not be reached. Make sure the server is started and reachable or switch to the local server");


                    _applicationStateService.RemoveState(liveProc, "Error: The backend could not be reached");

                    return false;
                }

                _logger.Log($"No response. Trying again in {timeBetweenRetriesMS} ms");
                failures++;
                await Task.Delay(timeBetweenRetriesMS);

                if (failures % retriesPerSecond == 0)
                {
                    _logger.Log("The backend could not be reached. Retrying...", true);
                }
                else if (failures > 10 * retriesPerSecond)
                {
                    _logger.Log("The backend could not be reached. Aborting...", true);

                    _applicationStateService.RemoveState(liveProc, "Error: The backend could not be reached");

                    return false;
                }
            }
        }

        _applicationStateService.RemoveState(liveProc);

        return true;
    }

    public async Task CloseNetlistOnServerAsync(UInt64 netlistId)
    {
        await PostAsync("/close-netlist?hash=" + netlistId, null, false);
    }

    public async Task CreateVhdlHierarchyAsync(IProjectFile vhdlFile)
    {
        await ShowHierarchyAsync((await GenerateNetlistAsync(vhdlFile, NetlistType.VHDL))!);
    }

    public async Task CreateVerilogHierarchyAsync(IProjectFile verilogFile)
    {
        await ShowHierarchyAsync((await GenerateNetlistAsync(verilogFile, NetlistType.Verilog))!);
    }

    public async Task CreateSystemVerilogHierarchyAsync(IProjectFile systemVerilogFile)
    {
        await ShowHierarchyAsync((await GenerateNetlistAsync(systemVerilogFile, NetlistType.System_Verilog))!);
    }

    private async Task<bool> ShowHierarchyAsync(IProjectFile netlistFile)
    {
        ApplicationProcess proc = _applicationStateService.AddState("Starting viewer", AppState.Loading);

        HttpResponseMessage? resp = null;
        string top = Path.GetFileNameWithoutExtension(netlistFile.FullPath);

        if (!File.Exists(netlistFile.FullPath))
        {
            _applicationStateService.RemoveState(proc, "Error: No JSON netlist was found");

            _logger.Log(
                "No json netlist was found. Aborting...");
            return false;
        }

        string content = await File.ReadAllTextAsync(netlistFile.FullPath);

        IHashService hashService = ServiceManager.GetHashService();
        ReadOnlySpan<byte> contentByteSpan = new ReadOnlySpan<Byte>(Encoding.UTF8.GetBytes(content));
        ReadOnlySpan<byte> pathByteSpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(netlistFile.FullPath));
        UInt32 pathHash = hashService.ComputeHash(pathByteSpan);
        UInt32 contenthash = hashService.ComputeHash(contentByteSpan);

        UInt64 combinedHash = ((UInt64)pathHash) << 32 | contenthash;

        currentNetlist = combinedHash;

        ServiceManager.GetCustomLogger().Log("Path hash: " + pathHash);
        ServiceManager.GetCustomLogger().Log("Full file hash is: " + contenthash);
        ServiceManager.GetCustomLogger().Log("Combined hash is: " + combinedHash);

        ServiceManager.GetViewportDimensionService()!.SetClickedElementPath(combinedHash, string.Empty);
        ServiceManager.GetViewportDimensionService()!.SetCurrentElementCount(combinedHash, 0);
        ServiceManager.GetViewportDimensionService()!.SetZoomElementDimensions(combinedHash, null);

        FileStream jsonFileStream = File.Open(netlistFile.FullPath, FileMode.Open, FileAccess.Read);

        MultipartFormDataContent formDataContent = new MultipartFormDataContent()
        {
            { new StreamContent(jsonFileStream), "file", netlistFile.Name }
        };

        ApplicationProcess waitForBackendProc =
            _applicationStateService.AddState("Layouting in progress", AppState.Loading);

        resp = await PostAsync(
            "/extractHierarchy" + $"?hash={combinedHash}",
            formDataContent);

        _applicationStateService.RemoveState(waitForBackendProc);

        jsonFileStream.Close();

        if (resp == null)
        {
            _applicationStateService.RemoveState(proc, "Error: No response from backend");

            return false;
        }

        if (!resp.IsSuccessStatusCode)
        {
            _applicationStateService.RemoveState(proc, "Error: The backend returned an error");

            return false;
        }
        
        (HierarchySideBarElement? elem, List<HierarchyViewElement>? elements) = await _hierarchyJsonParser.LoadHierarchyAsync(await resp.Content.ReadAsStreamAsync(), combinedHash);


        _applicationStateService.RemoveState(proc);
        
        HierarchySidebarViewModel sidebarVM = new HierarchySidebarViewModel();
        sidebarVM.InitializeContent();
        sidebarVM.Title = "Design hierarchy";
        ObservableCollection<HierarchySideBarElement> sidebarelements = new ObservableCollection<HierarchySideBarElement>();
        sidebarelements.Add(elem!.Children[0]);
        sidebarVM.Elements = sidebarelements;
        
        _dockService.Show(sidebarVM, DockShowLocation.Left);
        _dockService.InitializeContent();
        
        HierarchyViewModel hierarchyVM = new HierarchyViewModel();
        hierarchyVM.InitializeContent();
        hierarchyVM.Title = "Design hierarchy";
        hierarchyVM.NetlistId = combinedHash;
        ObservableCollection<HierarchyViewElement> obsElements = new ObservableCollection<HierarchyViewElement>();
        obsElements.AddRange(elements);
        hierarchyVM.Items = obsElements;
        
        _dockService.Show(hierarchyVM, DockShowLocation.Document);
        _dockService.InitializeContent();
        
        return true;
    }
}