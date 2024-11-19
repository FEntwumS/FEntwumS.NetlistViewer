using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Oneware.NetlistReaderFrontend.Controls;
using Oneware.NetlistReaderFrontend.ViewModels;

namespace Oneware.NetlistReaderFrontend.Views;

public partial class Frontend : UserControl
{
    private FrontendViewModel? _vm;
    
    public Frontend()
    {
        InitializeComponent();

        if (DataContext is FrontendViewModel vm)
        {
            Initialize(vm);
        }
    }

    // move netlist
    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var pointerpoints = e.GetIntermediatePoints(sender as Control);
        double dx, dy;
        
        var NetlistControlElem = this.Find<NetlistControl>("NetlistControl");

        if (pointerpoints.Count > 1 && pointerpoints.First().Properties.IsLeftButtonPressed)
        {
            dx = pointerpoints.Last().Position.X -  pointerpoints.First().Position.X;
            dy = pointerpoints.Last().Position.Y -  pointerpoints.First().Position.Y;
        }
        else
        {
            dx = 0;
            dy = 0;
        }

        NetlistControlElem.DeltaX += dx;
        NetlistControlElem.DeltaY += dy;
        NetlistControlElem.DeltaScale = 0;
    }

    // change zoom
    private void InputElement_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        var wheel = e.Delta;
        var NetlistControlElem = this.Find<NetlistControl>("NetlistControl");

        // only react to vertical scrolling for now
        NetlistControlElem.DeltaScale += wheel.Y;
        
        NetlistControlElem.PointerX = e.GetPosition(NetlistControlElem).X;
        NetlistControlElem.PointerY = e.GetPosition(NetlistControlElem).Y;
    }

    private void NetlistControl_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Get click event to Control
        // There should be a better way
        
        ((NetlistControl) sender).NetlistControl_PointerPressed(sender, e);
    }

    private void NetlistControl_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        ((NetlistControl) sender).NetlistControl_PointerReleased(sender, e);
    }

    private void NetlistControl_OnTapped(object? sender, TappedEventArgs e)
    {
        Console.WriteLine("Tap!");
        
        ((NetlistControl) sender).NetlistControl_OnTapped(sender, e);
    }

    private void NetlistControl_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        ((NetlistControl) sender).Redraw();
    }

    private void Initialize(FrontendViewModel vm)
    {
        _vm = vm;
    }
}