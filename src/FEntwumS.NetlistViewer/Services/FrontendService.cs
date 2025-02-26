using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Asmichi.ProcessManagement;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using FEntwumS.NetlistViewer.ViewModels;
using OneWare.Essentials.Helpers;
using OneWare.Essentials.PackageManager;
using OneWare.ProjectSystem.Models;
using StreamContent = System.Net.Http.StreamContent;

namespace FEntwumS.NetlistViewer.Services;

public class FrontendService
{
    private readonly ICustomLogger _logger;
    private readonly IApplicationStateService _applicationStateService;
    private readonly IDockService _dockService;
    private readonly ISettingsService _settingsService;
    private readonly IPackageService _packageService;

    private string _backendAddress = string.Empty;
    private string _backendPort = string.Empty;
    private bool _useLocalBackend = false;
    private int _requestTimeout = 600;
    private string _backendJarFolder = string.Empty;
    private int _entityLabelFontSize = 25;
    private int _cellLabelFontSize = 15;
    private int _edgeLabelFontSize = 10;
    private int _portLabelFontSize = 10;
    private string _javaBinaryFolder = string.Empty;
    private string extraJarArgs = string.Empty;
    private bool _continueOnBinaryInstallError = false;

    private UInt64 currentNetlist = 0;

    // Used to store the process handle of the backend process. If the handle is not stored, the process will be
    // automatically terminated
    private static IChildProcess? backendProcess;

    public FrontendService()
    {
        _logger = ServiceManager.GetCustomLogger();
        _applicationStateService = ServiceManager.GetService<IApplicationStateService>();
        _dockService = ServiceManager.GetService<IDockService>();
        _settingsService = ServiceManager.GetService<ISettingsService>();
        _packageService = ServiceManager.GetService<IPackageService>();
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
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                _logger.Error("Port label font size not valid. Please enter a positive integer");

                _portLabelFontSize = 10;
            }
        });

        _settingsService.GetSettingObservable<string>(FEntwumSNetlistReaderFrontendModule.JavaPathSetting).Subscribe(
            x => _javaBinaryFolder = x);
        
        _settingsService.GetSettingObservable<string>("NetlistViewer_java_args").Subscribe(x => extraJarArgs = x);
        
        _settingsService.GetSettingObservable<bool>("NetlistViewer_ContinueOnBinaryInstallError").Subscribe(x => _continueOnBinaryInstallError = x);
    }

    private async Task<(bool success, bool needsRestart)> InstallDependenciesAsync()
    {
        bool globalSuccess = true, needsRestart = false;

        // Get packages for external dependencies
        PackageModel? ghdlPackageModel = _packageService.Packages.GetValueOrDefault("OneWare.GhdlExtension");
        Package? ghdlPackage = ghdlPackageModel?.Package;
        
        PackageModel? ghdlBinaryPackageModel = _packageService.Packages.GetValueOrDefault("ghdl");
        Package? ghdlBinaryPackage = ghdlBinaryPackageModel?.Package;
        
        PackageModel? osscadsuiteBinaryPackageModel = _packageService.Packages.GetValueOrDefault("osscadsuite");
        Package? osscadsuiteBinaryPackage = osscadsuiteBinaryPackageModel?.Package;

        List<Package?> dependencyList = new();
        
        dependencyList.Add(ghdlPackage);
        dependencyList.Add(ghdlBinaryPackage);
        dependencyList.Add(osscadsuiteBinaryPackage);
        dependencyList.Add(FEntwumSNetlistReaderFrontendModule.NetlistPackage);
        dependencyList.Add(FEntwumSNetlistReaderFrontendModule.JDKPackage);

        foreach (Package? dependency in dependencyList)
        {
            if (dependency == null)
            {
                _logger.Error("A dependency could not be found in the extension manager. Please file a bug report including the following information:");
                _logger.Error("Requested and available dependencies:");

                foreach (Package? dep in dependencyList)
                {
                    if (dep == null)
                    {
                        continue;
                    }
                    
                    _logger.Error($"Name: {dep.Name} - ID: {dep.Id}");
                }
                
                globalSuccess = false;
                continue;
            }
            
            if (_packageService.Packages!.GetValueOrDefault(dependency!.Id) is
                {
                    Status: PackageStatus.Available or PackageStatus.Installing or PackageStatus.UpdateAvailable
                })
            {
                if (_settingsService.GetSettingValue<bool>("Experimental_AutoDownloadBinaries"))
                {
                    _logger.Log($"Installing \"{dependency.Name}\"...", true);
                    
                    bool localSuccess = await _packageService.InstallAsync(dependency);
                    
                    globalSuccess = globalSuccess && localSuccess;

                    if (localSuccess)
                    {
                        _logger.Log($"Successfully installed \"{dependency.Name}\".", true);
                    }
                    else
                    {
                        _logger.Error($"Failed to install \"{dependency.Name}\".");
                    }
                }
                else
                {
                    _logger.Error(
                        $"Extension \"{dependency.Name}\" is not installed. Please enable \"Automatically download Binaries\" under the \"Experimental\" settings or download the extension yourself");

                    globalSuccess = false;
                }

                if (globalSuccess)
                {
                    needsRestart = true;
                }
            }
        }

        if (globalSuccess && needsRestart)
        {
            _logger.Log("Dependencies were successfully installed. Please restart OneWare Studio!", true);
        }

        return (globalSuccess, needsRestart);
    }

    public async Task CreateVhdlNetlist(IProjectFile vhdl)
    {
        (bool success, bool needsRestart) = await InstallDependenciesAsync();

        if (needsRestart)
        {
            return;
        } else if (!(success || _continueOnBinaryInstallError))
        {
            return;
        }
        
        success = await StartBackendIfNotStartedAsync();

        if (!success)
        {
            return;
        }

        string top = Path.GetFileNameWithoutExtension(vhdl.FullPath);

        IGhdlService ghdlService = ServiceManager.GetService<IGhdlService>();
        IYosysService yosysService = ServiceManager.GetService<IYosysService>();

        success = await ghdlService.ElaborateDesignAsync(vhdl);

        if (!success)
        {
            return;
        }

        success = await ghdlService.CrossCompileDesignAsync(vhdl);


        if (!success)
        {
            return;
        }

        success = await yosysService.LoadVerilogAsync(vhdl);

        if (!success)
        {
            return;
        }

        string netlistPath = Path.Combine(vhdl.Root!.FullPath, "build", "netlist", $"{top}.json");

        if (!File.Exists(netlistPath))
        {
            _logger.Error($"Netlist file not found: {netlistPath}");
            return;
        }

        IProjectFile test = new ProjectFile(netlistPath, vhdl.TopFolder!);

        success = await ServerStartedAsync();

        if (!success)
        {
            return;
        }

        await ShowViewer(test);
    }

    public async Task CreateVerilogNetlist(IProjectFile verilog)
    {
        (bool success, bool needsRestart) = await InstallDependenciesAsync();

        if (needsRestart)
        {
            return;
        } else if (!(success || _continueOnBinaryInstallError))
        {
            return;
        }
        
        success = await StartBackendIfNotStartedAsync();

        if (!success)
        {
            return;
        }

        string top = Path.GetFileNameWithoutExtension(verilog.FullPath);

        IYosysService yosysService = ServiceManager.GetService<IYosysService>();

        await yosysService.LoadVerilogAsync(verilog);

        string netlistPath = Path.Combine(verilog.Root!.FullPath, "build", "netlist", $"{top}.json");

        if (!File.Exists(netlistPath))
        {
            _logger.Error($"Netlist file not found: {netlistPath}");
            return;
        }

        IProjectFile test = new ProjectFile(netlistPath, verilog.TopFolder!);

        success = await ServerStartedAsync();

        if (!success)
        {
            return;
        }

        await ShowViewer(test);
    }

    public async Task CreateSystemVerilogNetlist(IProjectFile sVerilog)
    {
        (bool success, bool needsRestart) = await InstallDependenciesAsync();

        if (needsRestart)
        {
            return;
        } else if (!(success || _continueOnBinaryInstallError))
        {
            return;
        }
        
        success = await StartBackendIfNotStartedAsync();

        if (!success)
        {
            return;
        }

        string top = Path.GetFileNameWithoutExtension(sVerilog.FullPath);

        IYosysService yosysService = ServiceManager.GetService<IYosysService>();

        await yosysService.LoadSystemVerilogAsync(sVerilog);

        IProjectFile test = new ProjectFile(Path.Combine(sVerilog.Root!.FullPath, "build", "netlist", $"{top}.json"),
            sVerilog.TopFolder!);

        success = await ServerStartedAsync();

        if (!success)
        {
            return;
        }

        await ShowViewer(test);
    }

    public async Task ShowViewer(IProjectFile json)
    {
        HttpResponseMessage? resp = null;
        string top = Path.GetFileNameWithoutExtension(json.FullPath);

        if (!File.Exists(json.FullPath))
        {
            _logger.Error(
                "No json netlist was generated. Please ensure you are using the yosys from the OSS CAD Suite");
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

        ServiceManager.GetViewportDimensionService().SetClickedElementPath(combinedHash, string.Empty);
        ServiceManager.GetViewportDimensionService().SetCurrentElementCount(combinedHash, 0);
        ServiceManager.GetViewportDimensionService().SetZoomElementDimensions(combinedHash, null);

        FrontendViewModel vm = new FrontendViewModel();
        vm.InitializeContent();
        vm.Title = $"Netlist: {top}";
        _logger.Log("Selected file: " + json.FullPath);

        FileStream jsonFileStream = File.Open(json.FullPath, FileMode.Open, FileAccess.Read);

        MultipartFormDataContent formDataContent = new MultipartFormDataContent()
        {
            { new StreamContent(jsonFileStream), "file", json.Name }
        };

        resp = await PostAsync(
            "/graphRemoteFile" + $"?hash={combinedHash}" + $"&entityLabelFontSize={_entityLabelFontSize}" +
            $"&cellLabelFontSize={_cellLabelFontSize}" + $"&edgeLabelFontSize={_edgeLabelFontSize}" +
            $"&portLabelFontSize={_portLabelFontSize}",
            formDataContent);

        jsonFileStream.Close();

        if (resp == null)
        {
            return;
        }

        vm.File = await resp.Content.ReadAsStreamAsync();

        if (!resp.IsSuccessStatusCode)
        {
            return;
        }

        _dockService.Show(vm, DockShowLocation.Document);
        _dockService.InitializeContent();
        vm.NetlistId = currentNetlist;
        await vm.OpenFileImpl();
    }

    public async Task ExpandNode(string nodePath, FrontendViewModel vm)
    {
        _logger.Log("Sending request to ExpandNode", true);

        var resp = await PostAsync("/expandNode?hash=" + vm.NetlistId + "&nodePath=" + nodePath, null);

        if (resp == null)
        {
            return;
        }

        vm.File = await resp.Content.ReadAsStreamAsync();

        _logger.Log("Answer received", true);

        await vm.OpenFileImpl();

        _logger.Log("Done", true);
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
        catch (InvalidOperationException e)
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
        catch (TaskCanceledException e)
        {
            _logger.Error(
                "The request has timed out. Please increase the request timeout time in the settings menu and try again",
                printErrors);
            return null;
        }
        catch (UriFormatException e)
        {
            _logger.Error(
                $"The provided server address ${_backendAddress} is not a valid address. Please enter a correct IP address",
                printErrors);
            return null;
        }
    }

    private async Task<bool> StartBackendIfNotStartedAsync()
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
        catch (Exception e)
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
            var dir = Directory.GetDirectories(_javaBinaryFolder).Where(x => Regex.Match(x, @"jdk-(\d+)\.(\d+)\.(\d+)\+(\d+)-jre").Success);

            if (dir.Count() == 0)
            {
                _logger.Error("No directory found. Please make sure that you have installed the \"Eclipse Adoptium OpenJDK\" binary using the extension manager");
                
                return false;
            }
            
            prefix = dir.First();
        }

        if (PlatformHelper.Platform is PlatformId.WinX64 or PlatformId.WinArm64)
        {
            suffix = ".exe";
        } else if (PlatformHelper.Platform is PlatformId.OsxX64 or PlatformId.OsxArm64)
        {
            prefix += "/Content/Home";
        }

        prefix += "/bin";
        
        string javaBinaryFile = Path.Combine(_javaBinaryFolder, $"{prefix}/java{suffix}");

        var serverJarFile = enumeratedResults.First();

        // Start server to run independently
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        backendProcess = await ServiceManager.GetService<IToolExecuterService>()
            .ExecuteBackgroundProcessAsync(javaBinaryFile,
                extraJarArgs.Split(' ').Concat( ["-jar", serverJarFile]).ToArray(),
                Path.GetDirectoryName(serverJarFile));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        _logger.Log("Server started", true);

        return true;
    }

    private async Task<bool> ServerStartedAsync()
    {
        HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.spring-boot.actuator.v3+json"));
        client.DefaultRequestHeaders.Add("User-Agent", "FEntwumS.NetlistViewer");
        client.BaseAddress = new Uri($"http://{_backendAddress}:{_backendPort}");
        client.Timeout = TimeSpan.FromSeconds(_requestTimeout);
        bool done = false;

        while (!done)
        {
            try
            {
                var res = await client.GetAsync("/server-active");
                _logger.Log("Server is awaiting requests", true);
                done = true;
            }
            catch (Exception e)
            {
                if (_useLocalBackend)
                {
                    _logger.Error(
                        "The remote server could not be reached. Make sure the server is started and reachable or switch to the local server");

                    return false;
                }

                _logger.Log("No response. Trying again in 10 ms", true);
                await Task.Delay(10);
            }
        }

        return true;
    }

    public async Task CloseNetlistOnServerAsync(UInt64 netlistId)
    {
        await PostAsync("/close-netlist?hash=" + netlistId, null, false);
    }
}