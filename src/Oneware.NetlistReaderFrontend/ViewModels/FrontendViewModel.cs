using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Core;
using OneWare.Essentials.Models;
using OneWare.Essentials.ViewModels;
using Oneware.NetlistReaderFrontend.Services;
using Oneware.NetlistReaderFrontend.Types;
using ReactiveUI;

namespace Oneware.NetlistReaderFrontend.ViewModels;

public class FrontendViewModel : ExtendedTool
{
    public ICommand LoadJSONCommand { get; }

    public string? StatusText
    {
        get { return statusText; }
        set
        {
            statusText = value;
            OnPropertyChanged();
        }
    }

    private string? testString;

    public string TestString
    {
        get { return testString; }
        set
        {
            testString = value;
            OnPropertyChanged();
        }
    }

    public ICommand TestAsyncCommand { get; }
    public ICommand UpdateScaleCommand { get; }

    private string? statusText;
    private double scale { get; set; }

    private AvaloniaList<NetlistElement> items { get; set; }

    public AvaloniaList<NetlistElement> Items
    {
        get => this.items;
        set
        {
            this.items.Clear();
            this.items.AddRange(value);
            OnPropertyChanged();
        }
    }

    public double Scale
    {
        get => this.scale;
        set
        {
            this.scale = value;
            OnPropertyChanged();
        }
    }

    private double offX { get; set; }

    public double OffX
    {
        get => this.offX;
        set
        {
            this.offX = value;
            OnPropertyChanged();
        }
    }

    private double offY { get; set; }

    public double OffY
    {
        get => this.offY;
        set
        {
            this.offY = value;
            OnPropertyChanged();
        }
    }

    private bool fitToZoom { get; set; }

    public bool FitToZoom
    {
        get => this.fitToZoom;
        set
        {
            this.fitToZoom = value;
            OnPropertyChanged();
        }
    }

    private bool isLoaded { get; set; }

    public bool IsLoaded
    {
        get => this.isLoaded;
        set
        {
            this.isLoaded = value;
            OnPropertyChanged();
        }
    }

    public ICommand FitToZoomCommand { get; }

    public FrontendViewModel() : base("Frontend")
    {
        items = new AvaloniaList<NetlistElement>();
        Items = new AvaloniaList<NetlistElement>();

        Scale = 0.2;

        OffX = 0;
        OffY = 0;
        FitToZoom = false;

        LoadJSONCommand = ReactiveCommand.CreateFromTask(OpenFileImpl);

        FitToZoomCommand = ReactiveCommand.Create(() => { FitToZoom = !FitToZoom; });
    }

    public async Task UpdateScaleImpl()
    {
        var jsonLoader = ServiceManager.GetJsonLoader();
        var dimensionService = ServiceManager.GetViewportDimensionService();

        dimensionService.SetHeight(jsonLoader.GetMaxHeight());
        dimensionService.SetWidth(jsonLoader.GetMaxWidth());

        IsLoaded = !IsLoaded;

        Scale = 0.2;

        OffX = 0;
        OffY = 0;
    }

    private async Task OpenFileImpl()
    {
        try
        {
            StatusText = "Opening file";

            var fileOpener = ServiceManager.GetFileOpener();
            var jsonLoader = ServiceManager.GetJsonLoader();

            var file = await fileOpener.OpenFileAsync();

            if (file is null)
            {
                StatusText = "No file opened";
                return;
            }

            StatusText = "File loaded";

            jsonLoader.OpenJson(file);

            Items.Clear();

            jsonLoader.parseJson(Items, 0, 0, this);

            StatusText = "JSON read";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}