using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FEntwumS.NetlistViewer.Helpers;
using FEntwumS.NetlistViewer.Types.Messages;
using OneWare.Essentials.ViewModels;

namespace FEntwumS.NetlistViewer.ViewModels;

public class HierarchyViewModel : ExtendedTool
{
    public ICommand FitToZoomCommand { get; }
    
    private ulong _netlistId { get; set; }

    public ulong NetlistId
    {
        get => _netlistId;
        set
        {
            _netlistId = value;
            OnPropertyChanged(nameof(NetlistId));
        }
    }

    public HierarchyViewModel() : base("Hierarchy")
    {
        FitToZoomCommand = new RelayCommand(() =>
        {
            WeakReferenceMessenger.Default.Send(new ZoomToFitmessage(_netlistId),
                FentwumSNetlistViewerSettingsHelper.HierarchyMessageChannel);
        });
    }   
}