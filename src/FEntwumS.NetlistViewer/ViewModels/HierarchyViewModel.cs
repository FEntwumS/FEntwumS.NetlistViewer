using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using OneWare.Essentials.ViewModels;

namespace FEntwumS.NetlistViewer.ViewModels;

public class HierarchyViewModel : ExtendedTool
{
    public ICommand FitToZoomCommand { get; }

    public HierarchyViewModel() : base("Hierarchy")
    {
        FitToZoomCommand = new RelayCommand(() => { });
    }   
}