using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using Oneware.NetlistReaderFrontend.ViewModels;
using OneWare.ProjectSystem.Models;
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
    }

    public async Task CreateVhdlNetlist(IProjectFile vhdl)
    {
        bool success = false;
        
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

        string netlistPath = Path.Combine(vhdl.Root!.FullPath, "build", "netlist", "netlist.json");

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
        IYosysService yosysService = ServiceManager.GetService<IYosysService>();
        
        await yosysService.LoadVerilogAsync(verilog);
        
        IProjectFile test = new ProjectFile(Path.Combine(verilog.Root!.FullPath, "build", "netlist", "netlist.json"), verilog.TopFolder!);

        await ShowViewer(test);
    }
    
    public async Task CreateSystemVerilogNetlist(IProjectFile sVerilog)
    {
        IYosysService yosysService = ServiceManager.GetService<IYosysService>();
        
        await yosysService.LoadSystemVerilogAsync(sVerilog);

        IProjectFile test = new ProjectFile(Path.Combine(sVerilog.Root!.FullPath, "build", "netlist", "netlist.json"), sVerilog.TopFolder!);

        await ShowViewer(test);
    }
    
    public async Task ShowViewer(IProjectFile json)
    {
        HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.spring-boot.actuator.v3+json"));
        client.DefaultRequestHeaders.Add("User-Agent", "Oneware.NetlistReaderFrontend");
        client.BaseAddress = new Uri($"http://{_backendAddress}:{_backendPort}");
        client.Timeout = TimeSpan.FromSeconds(5000);    // TODO add setting

        var content = await File.ReadAllTextAsync(json.FullPath);

        IHashService hashService = ServiceManager.GetHashService();
        var contentByteSpan = new ReadOnlySpan<Byte>(Encoding.UTF8.GetBytes(content));
        
        ServiceManager.GetCustomLogger().Log("GetHashCode() hash: " + json.FullPath.GetHashCode(), true);
        ServiceManager.GetCustomLogger().Log("OAAT hash: " + ServiceManager.GetHashService().ComputeHash(new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(json.FullPath))), true);
        
        var watch = Stopwatch.StartNew();
        UInt32 computedHash = hashService.ComputeHash(contentByteSpan);
        watch.Stop();
        ServiceManager.GetCustomLogger().Log("Full file hash is: " + computedHash, true);
        ServiceManager.GetCustomLogger().Log("Took " + watch.ElapsedMilliseconds + " milliseconds", true);

        MultipartFormDataContent formDataContent = new MultipartFormDataContent()
        {
            {new StreamContent(File.Open(json.FullPath, FileMode.Open, FileAccess.Read)), "file", json.Name}
        };
    
        var resp = await client.PostAsync("/graphRemoteFile", formDataContent);
    
    
    
        // Task<String> t = client.GetStringAsync("/graphLocalFile?filename=" + json.FullPath);
        // t.Wait();
        //
        // Console.Write(t.Result);
    
        var vm = new FrontendViewModel();
        vm.InitializeContent();
        vm.Title = json.Name;
        _logger.Log("Selected file: " + json.FullPath);
        vm.File = await resp.Content.ReadAsStreamAsync();
        _dockService.Show(vm, DockShowLocation.Document);
        _dockService.InitializeContent();
        vm.OpenFileImpl();
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
}