using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using Oneware.NetlistReaderFrontend.ViewModels;
using StreamContent = System.Net.Http.StreamContent;

namespace Oneware.NetlistReaderFrontend.Services;

public class FrontendService(ILogger logger, IApplicationStateService applicationStateService, IDockService dockService)
{
    private readonly ILogger _logger = logger;
    private readonly IApplicationStateService _applicationStateService = applicationStateService;
    private readonly IDockService _dockService = dockService;

    public async Task CreateVhdlNetlist(IProjectFile vhdl)
    {
        IGhdlService ghdlService = ServiceManager.GetService<IGhdlService>();
        IYosysService yosysService = ServiceManager.GetService<IYosysService>();
        
        await ghdlService.ElaborateDesignAsync(vhdl);
        await ghdlService.CrossCompileDesignAsync(vhdl);
        await yosysService.LoadVerilogAsync(vhdl);
        
        // ---------------
        // Big duplicate
        // ---------------
        // Quick and dirty
        // ---------------
        
        HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.spring-boot.actuator.v3+json"));
        client.DefaultRequestHeaders.Add("User-Agent", "Oneware.NetlistReaderFrontend");
        client.BaseAddress = new Uri("http://localhost:8080");
        
        string workingDirectory = Path.Combine(vhdl.Root!.FullPath, "build", "netlist");

        var content = await File.ReadAllTextAsync(Path.Combine(workingDirectory, "design.v"));

        IHashService hashService = ServiceManager.GetHashService();
        var contentByteSpan = new ReadOnlySpan<Byte>(Encoding.UTF8.GetBytes(content));
        
        ServiceManager.GetCustomLogger().Log("GetHashCode() hash: " + Path.Combine(workingDirectory, "design.v").GetHashCode(), true);
        ServiceManager.GetCustomLogger().Log("OAAT hash: " + ServiceManager.GetHashService().ComputeHash(new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(Path.Combine(workingDirectory, "design.v")))), true);
        
        var watch = Stopwatch.StartNew();
        UInt32 computedHash = hashService.ComputeHash(contentByteSpan);
        watch.Stop();
        ServiceManager.GetCustomLogger().Log("Full file hash is: " + computedHash, true);
        ServiceManager.GetCustomLogger().Log("Took " + watch.ElapsedMilliseconds + " milliseconds", true);

        MultipartFormDataContent formDataContent = new MultipartFormDataContent()
        {
            {new StreamContent(File.Open(Path.Combine(workingDirectory, "netlist.json"), FileMode.Open, FileAccess.Read)), "file", vhdl.Name}
        };
    
        var resp = await client.PostAsync("/graphRemoteFile", formDataContent);
    
    
    
        // Task<String> t = client.GetStringAsync("/graphLocalFile?filename=" + json.FullPath);
        // t.Wait();
        //
        // Console.Write(t.Result);
    
        var vm = new FrontendViewModel();
        vm.InitializeContent();
        vm.Title = vhdl.Name;
        _logger.Log("Selected file: " + vhdl.FullPath);
        vm.File = await resp.Content.ReadAsStreamAsync();
        _dockService.Show(vm, DockShowLocation.Document);
        _dockService.InitializeContent();
        vm.OpenFileImpl();
    }

    public async Task CreateVerilogNetlist(IProjectFile verilog)
    {
        IYosysService yosysService = ServiceManager.GetService<IYosysService>();
        
        await yosysService.LoadVerilogAsync(verilog);
        
        // ---------------
        // Big duplicate
        // ---------------
        // Quick and dirty
        // ---------------
        
        HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.spring-boot.actuator.v3+json"));
        client.DefaultRequestHeaders.Add("User-Agent", "Oneware.NetlistReaderFrontend");
        client.BaseAddress = new Uri("http://localhost:8080");
        client.Timeout = TimeSpan.FromSeconds(5000);
        
        string workingDirectory = Path.Combine(verilog.Root!.FullPath, "build", "netlist");

        //var content = await File.ReadAllTextAsync(Path.Combine(workingDirectory, "design.v"));
        var content = "a";

        IHashService hashService = ServiceManager.GetHashService();
        var contentByteSpan = new ReadOnlySpan<Byte>(Encoding.UTF8.GetBytes(content));
        
        ServiceManager.GetCustomLogger().Log("GetHashCode() hash: " + Path.Combine(workingDirectory, "design.v").GetHashCode(), true);
        ServiceManager.GetCustomLogger().Log("OAAT hash: " + ServiceManager.GetHashService().ComputeHash(new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(Path.Combine(workingDirectory, "design.v")))), true);
        
        var watch = Stopwatch.StartNew();
        UInt32 computedHash = hashService.ComputeHash(contentByteSpan);
        watch.Stop();
        ServiceManager.GetCustomLogger().Log("Full file hash is: " + computedHash, true);
        ServiceManager.GetCustomLogger().Log("Took " + watch.ElapsedMilliseconds + " milliseconds", true);

        MultipartFormDataContent formDataContent = new MultipartFormDataContent()
        {
            {new StreamContent(File.Open(Path.Combine(workingDirectory, "netlist.json"), FileMode.Open, FileAccess.Read)), "file", verilog.Name}
        };
    
        var resp = await client.PostAsync("/graphRemoteFile", formDataContent);
    
    
    
        // Task<String> t = client.GetStringAsync("/graphLocalFile?filename=" + json.FullPath);
        // t.Wait();
        //
        // Console.Write(t.Result);
    
        var vm = new FrontendViewModel();
        vm.InitializeContent();
        vm.Title = verilog.Name;
        _logger.Log("Selected file: " + verilog.FullPath);
        vm.File = await resp.Content.ReadAsStreamAsync();
        _dockService.Show(vm, DockShowLocation.Document);
        _dockService.InitializeContent();
        vm.OpenFileImpl();
    }
    
    public async Task CreateSystemVerilogNetlist(IProjectFile sVerilog)
    {
        IYosysService yosysService = ServiceManager.GetService<IYosysService>();
        
        await yosysService.LoadSystemVerilogAsync(sVerilog);
        
        // ---------------
        // Big duplicate
        // ---------------
        // Quick and dirty
        // ---------------
        
        HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.spring-boot.actuator.v3+json"));
        client.DefaultRequestHeaders.Add("User-Agent", "Oneware.NetlistReaderFrontend");
        client.BaseAddress = new Uri("http://localhost:8080");
        client.Timeout = TimeSpan.FromSeconds(5000);
        
        string workingDirectory = Path.Combine(sVerilog.Root!.FullPath, "build", "netlist");

        //var content = await File.ReadAllTextAsync(Path.Combine(workingDirectory, "design.v"));
        var content = "a";

        IHashService hashService = ServiceManager.GetHashService();
        var contentByteSpan = new ReadOnlySpan<Byte>(Encoding.UTF8.GetBytes(content));
        
        ServiceManager.GetCustomLogger().Log("GetHashCode() hash: " + Path.Combine(workingDirectory, "design.v").GetHashCode(), true);
        ServiceManager.GetCustomLogger().Log("OAAT hash: " + ServiceManager.GetHashService().ComputeHash(new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(Path.Combine(workingDirectory, "design.v")))), true);
        
        var watch = Stopwatch.StartNew();
        UInt32 computedHash = hashService.ComputeHash(contentByteSpan);
        watch.Stop();
        ServiceManager.GetCustomLogger().Log("Full file hash is: " + computedHash, true);
        ServiceManager.GetCustomLogger().Log("Took " + watch.ElapsedMilliseconds + " milliseconds", true);

        MultipartFormDataContent formDataContent = new MultipartFormDataContent()
        {
            {new StreamContent(File.Open(Path.Combine(workingDirectory, "netlist.json"), FileMode.Open, FileAccess.Read)), "file", sVerilog.Name}
        };
    
        var resp = await client.PostAsync("/graphRemoteFile", formDataContent);
    
    
    
        // Task<String> t = client.GetStringAsync("/graphLocalFile?filename=" + json.FullPath);
        // t.Wait();
        //
        // Console.Write(t.Result);
    
        var vm = new FrontendViewModel();
        vm.InitializeContent();
        vm.Title = sVerilog.Name;
        _logger.Log("Selected file: " + sVerilog.FullPath);
        vm.File = await resp.Content.ReadAsStreamAsync();
        _dockService.Show(vm, DockShowLocation.Document);
        _dockService.InitializeContent();
        vm.OpenFileImpl();
    }
    
    public async Task ShowViewer(IProjectFile json)
    {
        HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.spring-boot.actuator.v3+json"));
        client.DefaultRequestHeaders.Add("User-Agent", "Oneware.NetlistReaderFrontend");
        client.BaseAddress = new Uri("http://localhost:8080");

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
}