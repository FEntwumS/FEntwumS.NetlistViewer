using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FEntwumS.NetlistViewer.Helpers;
using FEntwumS.NetlistViewer.Services;
using FEntwumS.NetlistViewer.Types.Messages;
using OneWare.Essentials.ViewModels;
using ReactiveUI;

namespace FEntwumS.NetlistViewer.ViewModels;

public class HierarchyViewModel : ExtendedTool
{
    public ICommand FitToZoomCommand { get; }

    public HierarchyViewModel() : base("Hierarchy")
    {
        FitToZoomCommand = new RelayCommand(() =>
        {
            WeakReferenceMessenger.Default.Send(new ZoomToFitmessage(true),
                FentwumSNetlistViewerSettingsHelper.HierarchyMessageChannel);
        });
    }   
}