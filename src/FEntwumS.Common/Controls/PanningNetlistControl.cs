using Avalonia;
using Avalonia.Data;
using Avalonia.Input;
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
		    // ZoomToFit();
	    }
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
	    base.OnPointerWheelChanged(e);

	    UpdateViewPort();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
	    base.OnPointerMoved(e);

	    if (e.Properties.IsLeftButtonPressed)
	    {
		    UpdateViewPort();
	    }
    }

    private void UpdateViewPort()
    {
	    if (Child is GraphRootNodeControl graphRootNodeControl)
	    {
		    double horizontalOffset = this.Bounds.Width * 0.1d;
		    double verticalOffset = this.Bounds.Height * 0.1d;

		    double viewPortX = -OffsetX - horizontalOffset;
		    double viewPortY = -OffsetY - verticalOffset;
		    double viewPortWidth = this.Bounds.Width + 2 * horizontalOffset;
		    double viewPortHeight = this.Bounds.Height + 2 *verticalOffset;
		    
		    graphRootNodeControl.CurrentViewPort = new Rect(viewPortX, viewPortY, viewPortWidth, viewPortHeight);
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