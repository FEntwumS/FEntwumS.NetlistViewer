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

public class FrontendViewModel: FlexibleWindowViewModelBase
{
    public ICommand LoadJSONCommand { get; }
    public string? StatusText
    {
        get { return statusText; }
        set { statusText = value; OnPropertyChanged(); }
    }

    private string? testString;
    public string TestString
    {
        get { return testString; }
        set { testString = value; OnPropertyChanged(); }
    }

    public ICommand TestAsyncCommand { get; }
    public ICommand UpdateScaleCommand { get; }

    private string? statusText;
    private double scale { get; set; }

    private AvaloniaList<NetlistElement> items { get; set; }

    public AvaloniaList<NetlistElement> Items
    {
        get => this.items;
        set { this.items.Clear(); this.items.AddRange(value); OnPropertyChanged(); }
    }
    public double Scale { 
        get => this.scale;
        set { this.scale = value; OnPropertyChanged(); }
    }
        
    private double offX { get; set; }

    public double OffX
    {
        get => this.offX;
        set { this.offX = value; OnPropertyChanged(); }
    }
        
    private double offY { get; set; }

    public double OffY
    {
        get => this.offY;
        set { this.offY = value; OnPropertyChanged(); }
    }
        
    private bool fitToZoom { get; set; }

    public bool FitToZoom
    {
        get => this.fitToZoom;
        set { this.fitToZoom = value; OnPropertyChanged(); }
    }
        
    private bool isLoaded { get; set; }

    public bool IsLoaded
    {
        get => this.isLoaded;
        set { this.isLoaded = value; OnPropertyChanged(); }
    }

    public ICommand FitToZoomCommand { get; }
    public FrontendViewModel(IProjectFile jsonfile)
    {
        items = new AvaloniaList<NetlistElement>();
            Items = new AvaloniaList<NetlistElement>();

            Scale = 0.2;
            
            OffX = 0;
            OffY = 0;
            FitToZoom = false;
            
            LoadJSONCommand =  ReactiveCommand.CreateFromTask(OpenFileImpl);

            FitToZoomCommand = ReactiveCommand.Create(() =>
            {
                FitToZoom = !FitToZoom;
            });
            
            TestAsyncCommand = ReactiveCommand.Create(() =>
            {
                int nodecnt = 1884;
                int portcnt = 24658;
                int edgecnt = 20368;
                int labelcnt = 27533;
                int h = 253647;
                int w = 165139;
                int bendcnt = 6;
                int charcnt = 10;
                int junctioncnt = 0;
                
                // uncomment for smaller netlist
                nodecnt = 269;
                portcnt = 3225;
                edgecnt = 2027;
                labelcnt = 3628;
                bendcnt = 6;
                charcnt = 9;
                h = 41493;
                w = 18169;
                junctioncnt = 1157;
                
                Random rnd = new Random();
                
                // generate nodes

                for (int i = 0; i < nodecnt; i++)
                {
                    Items.Add(new NetlistElement()
                    {
                        Type = 1,
                        Width = rnd.Next(0, w / 25),
                        Height = rnd.Next(0, h / 25),
                        xPos = rnd.Next(0, (int)(w * 0.99)),
                        yPos = rnd.Next(0, (int)(h * 0.99))
                    });
                }
                
                //generate ports

                for (int i = 0; i < portcnt; i++)
                {
                    Items.Add(new NetlistElement()
                    {
                        Type = 5,
                        xPos = rnd.Next(0, w - 10),
                        yPos = rnd.Next(0, h - 10),
                    });
                }
                
                // generate labels

                for (int i = 0; i < labelcnt; i++)
                {
                    Items.Add(new NetlistElement()
                    {
                        Type = 3,
                        xPos = rnd.Next(0, w - 100),
                        yPos = rnd.Next(0, h - 100),
                        LabelText = "aaaaaaaaaa",
                    });
                }
                
                //generate junctions 

                for (int i = 0; i < junctioncnt; i++)
                {
                    Items.Add(new NetlistElement()
                    {
                        Type = 4,
                        xPos = rnd.Next(0, w),
                        yPos = rnd.Next(0, h),
                    });
                }
                
                // generate edges

                for (int i = 0; i < edgecnt; i++)
                {
                    StreamGeometry geo = new StreamGeometry();
                    
                    StreamGeometryContext con = geo.Open();
                    
                    con.BeginFigure(new Point(0, 0), false);

                    for (int j = 0; j < bendcnt; j++)
                    {
                        con.LineTo(new Point(rnd.Next(0, w / 2), rnd.Next(0, h / 2)));
                    }
                    
                    con.EndFigure(false);

                    Items.Add(new NetlistElement()
                    {
                        Type = 2,
                        xPos = rnd.Next(0, w),
                        yPos = rnd.Next(0, h)
                    });
                }

                Scale = 0.005;
                
                TestString = "works";
            });
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