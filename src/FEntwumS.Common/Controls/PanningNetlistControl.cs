using Avalonia;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace FEntwumS.Common.Controls;

public class PanningNetlistControl : PanningControl
{
    #region Properties

    public ulong NetlistId
    {
        get => GetValue(NetlistIdProperty);
        set => SetValue(NetlistIdProperty, value);
    }
    
    public static readonly StyledProperty<ulong> NetlistIdProperty =
        AvaloniaProperty.Register<PanningNetlistControl, ulong>(nameof(NetlistId),
            defaultBindingMode: BindingMode.TwoWay);
    
    public string ProjectRootFolder
    {
	    get => GetValue(ProjectRootFolderProperty);
	    set => SetValue(ProjectRootFolderProperty, value);
    }
    
    public static readonly  StyledProperty<string> ProjectRootFolderProperty =
	    AvaloniaProperty.Register<PanningNetlistControl, string>(nameof(ProjectRootFolder),
		    defaultBindingMode: BindingMode.TwoWay);

    #endregion
    
    #region Event Handling

    protected override void OnLoaded(RoutedEventArgs e)
    {
	    base.OnLoaded(e);

	    if (Child is not null && double.IsNaN(Child.Scale))
	    {
		    Child = null;
	    }

	    if (Child is not null)
	    {
		    ZoomToFit();
	    }
    }

    #endregion
    
    #region Rendering

    public override void Render(DrawingContext context)
    {
        context.DrawRectangle(Brushes.White, null, Bounds);
    }
    
    #endregion
}