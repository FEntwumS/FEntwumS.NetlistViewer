
using System.Runtime.Serialization;
using System.Windows.Input;
using Avalonia.Data;
using CommunityToolkit.Mvvm.Input;
using FEntwumS.Common.Controls;
using FEntwumS.Common.Types;
using OneWare.Essentials.ViewModels;

namespace FEntwumS.Common.ViewModels;

public class NetlistViewModel : ExtendedTool
{
	[DataMember]
	private ulong _netlistId;

	public ulong NetlistId
	{
		get => _netlistId;
		set
		{
			_netlistId = value;
			OnPropertyChanged(nameof(NetlistId));
		}
	}

	private GenericGraphElementControl _rootNode = new GraphNodeControl();

	public GenericGraphElementControl RootNode
	{
		get => _rootNode;
		set
		{
			_rootNode = value;

			if (_rootNode is not null)
			{
				var b = new Binding()
				{
					Source = this,
					Path = nameof(this.NetlistTheme)
				};
		
				_rootNode.Bind(
					GenericGraphElementControl.NetlistThemeProperty,
					b
				);
			}
			
			OnPropertyChanged(nameof(RootNode));
		}
	}
	
	private ICommand _scaleChangedCommand;

	public ICommand ScaleChangedCommand
	{
		get => _scaleChangedCommand;
		set
		{
			_scaleChangedCommand = value;
			OnPropertyChanged(nameof(ScaleChangedCommand));
		}
	}
	
	private NetlistTheme _netlistTheme = new();

	public NetlistTheme NetlistTheme
	{
		get => _netlistTheme;
		set
		{
			_netlistTheme = value;
			
			_netlistTheme.PropertyChanged += (sender, args) =>
			{
				OnPropertyChanged(nameof(NetlistTheme));
			};
			OnPropertyChanged(nameof(NetlistTheme));
		}
	}
	
	public NetlistViewModel() : base("NetlistViewer_Netlist")
	{
		ScaleChangedCommand = new RelayCommand(() =>
		{
			if (RootNode is null)
			{
				return;
			}

			RootNode.NetlistTheme.RegenerateBrushesAndPens(RootNode.Scale);
			
			OnPropertyChanged(nameof(NetlistTheme));
		});
	}
}