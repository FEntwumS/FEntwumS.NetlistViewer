using System.Runtime.Serialization;
using FEntwumS.Common.Controls;
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

	private PositionableSubControl _rootNode = new();

	public PositionableSubControl RootNode
	{
		get => _rootNode;
		set
		{
			_rootNode = value;
			OnPropertyChanged(nameof(RootNode));
		}
	}
	
	public NetlistViewModel() : base("NetlistViewer_Netlist")
	{
		
	}
}