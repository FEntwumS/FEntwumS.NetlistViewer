using System.Net.Http.Headers;
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

    public async Task ShowViewer(IProjectFile json)
    {
        HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.spring-boot.actuator.v3+json"));
        client.DefaultRequestHeaders.Add("User-Agent", "Oneware.NetlistReaderFrontend");
        client.BaseAddress = new Uri("http://localhost:8080");

        //var content = File.ReadAllText(json.FullPath);

        MultipartFormDataContent formDataContent = new MultipartFormDataContent()
        {
            {new StreamContent(File.Open(json.FullPath, FileMode.Open, FileAccess.Read)), "file", json.Name}
        };
    
        //formDataContent.Add(new StreamContent(File.Open(json.FullPath, FileMode.Open, FileAccess.Read)), "file");
    
        var resp = await client.PostAsync("/graphRemoteFile", formDataContent);
    
        //Console.WriteLine(resp.Content.ReadAsStringAsync().Result);
    
    
    
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