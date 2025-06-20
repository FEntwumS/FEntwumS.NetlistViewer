using Avalonia.Media;

namespace FEntwumS.NetlistViewer.Types.HierarchyView;

public class HierarchyViewPort : HierarchyViewShape
{
    // arrow_swap_regular
    // arrow_export_rtl_regular
    // arrow_left_regular
    // arrow_right_regular
    // arrow_import_regular
    
    private StreamGeometry? _geometry;

    public StreamGeometry Geometry
    {
        get => _geometry ?? new StreamGeometry();
        set => _geometry = value;
    }
}