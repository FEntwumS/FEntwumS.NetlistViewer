﻿using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using Oneware.NetlistReaderFrontend.Controls;
using Oneware.NetlistReaderFrontend.ViewModels;
using Oneware.NetlistReaderFrontend.Views;
using OneWare.ProjectSystem.Models;
using Prism.Services.Dialogs;
using StreamContent = System.Net.Http.StreamContent;

namespace Oneware.NetlistReaderFrontend.Services;

public class FrontendService
{
    private readonly ICustomLogger _logger;
    private readonly IApplicationStateService _applicationStateService;
    private readonly IDockService _dockService;
    private readonly ISettingsService _settingsService;

    private string _backendAddress = string.Empty;
    private string _backendPort = string.Empty;
    private bool _useRemoteBackend = false;
    private int _requestTimeout = 600;

    private UInt64 currentNetlist = 0;

    public FrontendService()
    {
        _logger = ServiceManager.GetCustomLogger();
        _applicationStateService = ServiceManager.GetService<IApplicationStateService>();
        _dockService = ServiceManager.GetService<IDockService>();
        _settingsService = ServiceManager.GetService<ISettingsService>();
    }

    public void SubscribeToSettings()
    {
        _settingsService.GetSettingObservable<string>("NetlistViewer_Backend_Address").Subscribe(x =>
        {
            if (isAddressValid(x))
            {
                _backendAddress = x;

                _logger.Log($"New address: {_backendAddress}", true);
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
                _backendPort = x;
                _logger.Log($"New port: {_backendPort}", true);
            }
            else
            {
                _logger.Error($"{x} is not a valid port");
            }
        });

        _settingsService.GetSettingObservable<bool>("NetlistViewer_Backend_UseRemote")
            .Subscribe(x => _useRemoteBackend = x);
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
    }

    public async Task CreateVhdlNetlist(IProjectFile vhdl)
    {
        bool success = false;
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

        await ShowViewer(test);
    }

    public async Task CreateVerilogNetlist(IProjectFile verilog)
    {
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

        await ShowViewer(test);
    }

    public async Task CreateSystemVerilogNetlist(IProjectFile sVerilog)
    {
        string top = Path.GetFileNameWithoutExtension(sVerilog.FullPath);

        IYosysService yosysService = ServiceManager.GetService<IYosysService>();

        await yosysService.LoadSystemVerilogAsync(sVerilog);

        IProjectFile test = new ProjectFile(Path.Combine(sVerilog.Root!.FullPath, "build", "netlist", $"{top}.json"),
            sVerilog.TopFolder!);

        await ShowViewer(test);
    }

    public async Task ShowViewer(IProjectFile json)
    {
        string top = Path.GetFileNameWithoutExtension(json.FullPath);

        string content = await File.ReadAllTextAsync(json.FullPath);

        IHashService hashService = ServiceManager.GetHashService();
        ReadOnlySpan<byte> contentByteSpan = new ReadOnlySpan<Byte>(Encoding.UTF8.GetBytes(content));
        ReadOnlySpan<byte> pathByteSpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(json.FullPath));
        UInt32 pathHash = hashService.ComputeHash(pathByteSpan);
        UInt32 contenthash = hashService.ComputeHash(contentByteSpan);

        UInt64 combinedHash = ((UInt64)pathHash) << 32 | contenthash;

        currentNetlist = combinedHash;

        ServiceManager.GetCustomLogger().Log("Path hash: " + pathHash, true);
        ServiceManager.GetCustomLogger().Log("Full file hash is: " + contenthash, true);
        ServiceManager.GetCustomLogger().Log("Combined hash is: " + combinedHash, true);

        ServiceManager.GetViewportDimensionService().SetClickedElementPath(combinedHash, string.Empty);
        ServiceManager.GetViewportDimensionService().SetCurrentElementCount(combinedHash, 0);
        ServiceManager.GetViewportDimensionService().SetZoomElementDimensions(combinedHash, null);

        FrontendViewModel vm = new FrontendViewModel();
        vm.InitializeContent();
        vm.Title = $"Netlist: {top}";
        _logger.Log("Selected file: " + json.FullPath);

        if (_useRemoteBackend)
        {
            MultipartFormDataContent formDataContent = new MultipartFormDataContent()
            {
                { new StreamContent(File.Open(json.FullPath, FileMode.Open, FileAccess.Read)), "file", json.Name }
            };

            var resp = await PostAsync("/graphRemoteFile?hash=" + combinedHash, formDataContent);

            vm.File = await resp.Content.ReadAsStreamAsync();
        }
        else
        {
            var resp = await PostAsync("/graphLocalFile?filename=" + json.FullPath + "&hash=" +
                                       combinedHash, null);

            vm.File = await resp.Content.ReadAsStreamAsync();
        }

        _dockService.Show(vm, DockShowLocation.Document);
        _dockService.InitializeContent();
        vm.OpenFileImpl();
        vm.NetlistId = currentNetlist;
    }

    public async Task ExpandNode(string nodePath, NetlistControl control, FrontendViewModel vm)
    {
        _logger.Log("Sending request to ExpandNode", true);

        var resp = await PostAsync("/expandNode?hash=" + vm.NetlistId + "&nodePath=" + nodePath, null);

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
        var match = Regex.Match(address, IpV4AddressPattern);

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

    private async Task<HttpResponseMessage> PostAsync(string URI, HttpContent? content)
    {
        HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.spring-boot.actuator.v3+json"));
        client.DefaultRequestHeaders.Add("User-Agent", "Oneware.NetlistReaderFrontend");
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
                        "The requested resource could not be found on the server. This could be due to a server restart. Please Re-Open your netlist.");
                }
                else
                {
                    _logger.Error("An internal server error occured. Please file a bug report if this problem persists.");
                }
            }
            
            return resp;
        }
        catch (InvalidOperationException e)
        {
            _logger.Error(
                $"The server at {_backendAddress} could not be reached. Make sure the server is started and reachable under this address");
            return null;
        }
        catch (HttpRequestException e)
        {
            switch (e.HttpRequestError)
            {
                case HttpRequestError.NameResolutionError:
                    _logger.Error(
                        $"The address {_backendAddress} could not be resolved. Make sure the server is started and reachable under this address");
                    break;

                case HttpRequestError.ConnectionError:
                    _logger.Error(
                        $"The address {_backendAddress} could not be reached. Make sure the server is started and reachable under this address");
                    break;

                default:
                    _logger.Error(
                        "Due to an internal error, the server could not complete the request. Please file a bug report");
                    break;
            }

            return null;
        }
        catch (TaskCanceledException e)
        {
            _logger.Error(
                "The request has timed out. Please increase the request timeout time in the settings menu and try again");
            return null;
        }
        catch (UriFormatException e)
        {
            _logger.Error(
                $"The provided server address ${_backendAddress} is not a valid address. Please enter a correct IP address");
            return null;
        }
    }
}