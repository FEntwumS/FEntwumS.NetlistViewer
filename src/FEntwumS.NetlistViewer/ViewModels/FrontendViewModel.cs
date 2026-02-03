using System.Runtime.Serialization;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using FEntwumS.NetlistViewer.Controls;
using FEntwumS.NetlistViewer.Services;
using FEntwumS.NetlistViewer.Types;
using Microsoft.Extensions.Logging;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;

namespace FEntwumS.NetlistViewer.ViewModels;

public class FrontendViewModel : ExtendedTool
{
	public ICommand LoadJsonCommand { get; }
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

	private Stream? file { get; set; }

	public Stream? File
	{
		get => file;
		set
		{
			this.file = value;
			OnPropertyChanged();
		}
	}

	private string? clickedElementPath { get; set; }

	public string? ClickedElementPath
	{
		get => clickedElementPath;
		set
		{
			this.clickedElementPath = value;
			OnPropertyChanged();
			_ = ClickedElementPathChangedAsync();
		}
	}

	public ICommand FitToZoomCommand { get; }

	public UInt64 NetlistId
	{
		get => netlistId;
		set
		{
			netlistId = value;
			OnPropertyChanged();
		}
	}

	[DataMember] private UInt64 netlistId { get; set; }

	private bool fileLoaded { get; set; }

	[DataMember]
	public bool FileLoaded
	{
		get => fileLoaded;
		set
		{
			fileLoaded = value;
			OnPropertyChanged();
		}
	}


	private ILogger _logger { get; set; }
	private readonly IApplicationStateService _applicationStateService;

	private FrontendService _frontendService { get; set; }

	public FrontendViewModel() : base("Frontend")
	{
		items = new AvaloniaList<NetlistElement>();
		Items = new AvaloniaList<NetlistElement>();

		FitToZoom = false;

		_logger = ServiceManager.GetService<ILogger>();
		_applicationStateService = ServiceManager.GetService<IApplicationStateService>();

		// OneWare uses the Community MVVM Toolkit. If ReactiveUI is used in an extension, any access to a bound property
		// inside a ReactiveCommand leads to an exception
		LoadJsonCommand = new RelayCommand(() => _ = OpenFileImplAsync());

		FitToZoomCommand = new RelayCommand(() => { FitToZoom = !FitToZoom; });

		_frontendService = ServiceManager.GetService<FrontendService>();

		FileLoaded = false;
	}

	public async Task ClickedElementPathChangedAsync()
	{
		await _frontendService.ExpandNodeAsync(clickedElementPath, this);
	}

	public override bool OnClose()
	{
		_ = ServiceManager.GetService<FrontendService>().CloseNetlistOnServerAsync(netlistId);

		return base.OnClose();
	}

	private void UpdateScaleImpl()
	{
		IsLoaded = !IsLoaded;
		FitToZoom = !FitToZoom;
	}

	public async Task OpenFileImplAsync()
	{
		var jsonLoader = ServiceManager.GetJsonLoader();

		await Task.Run(() =>
		{
			try
			{
				ApplicationProcess readProc = _applicationStateService.AddState("Reading response", AppState.Loading);

				_logger.Log("Opening file...");

				//var file = fileOpener.OpenFileAsync();

				if (File is null)
				{
					_logger.Error("File is empty.");
					return;
				}

				_logger.Log("File loaded");

				_applicationStateService.RemoveState(readProc);

				ApplicationProcess loadProc = _applicationStateService.AddState("Loading JSON", AppState.Loading);

				Task t = jsonLoader.OpenJsonAsync(File, netlistId);
				t.Wait();

				_applicationStateService.RemoveState(loadProc);

				File.Close();

				FileLoaded = false;

				Items.Clear();

				Items.AddRange(jsonLoader.ParseJsonAsync(0, 0, this, netlistId).Result);

				UpdateScaleImpl();

				FileLoaded = true;

				_logger.Log("JSON read");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		});
	}
}