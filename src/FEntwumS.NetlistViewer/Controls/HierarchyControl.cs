using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Threading;
using FEntwumS.NetlistViewer.Types.HierarchyView;

namespace FEntwumS.NetlistViewer.Controls;

public class HierarchyControl : TemplatedControl, ICustomHitTest
{

    #region Properties
    
    private ObservableCollection<HierarchyViewElement>? _items = new();

    public ObservableCollection<HierarchyViewElement> Items
    {
        get => _items ??= new ObservableCollection<HierarchyViewElement>();
        set => _items = value;
    }

    public static readonly DirectProperty<HierarchyControl, ObservableCollection<HierarchyViewElement>> ItemsProperty =
        AvaloniaProperty.RegisterDirect<HierarchyControl, ObservableCollection<HierarchyViewElement>>(nameof(Items),
            control => control.Items, (control, items) => control.Items = items,
            defaultBindingMode: BindingMode.TwoWay);
    
    #endregion

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
    }

    public void Redraw()
    {
        _ = Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Render);
    }

    public override void Render(DrawingContext context)
    {
        if (!IsInitialized)
        {
            return;
        }
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}