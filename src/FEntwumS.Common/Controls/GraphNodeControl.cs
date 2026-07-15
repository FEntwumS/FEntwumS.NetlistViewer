using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Rendering;

namespace FEntwumS.Common.Controls;

public class GraphNodeControl : GenericGraphElementControl, ICustomHitTest
{
	#region Properties

	private string CellName { get; set; } = "";
	
	public static readonly StyledProperty<string> CellNameProperty =
		AvaloniaProperty.Register<GraphNodeControl, string>(nameof(CellName),
			defaultBindingMode: BindingMode.TwoWay,
			defaultValue: "");
	
	private string CellType { get; set; } = "";
	
	public static readonly StyledProperty<string> CellTypeProperty =
		AvaloniaProperty.Register<GraphNodeControl, string>(nameof(CellType),
			defaultBindingMode: BindingMode.TwoWay,
			defaultValue: "");

	#endregion
	
	private AvaloniaList<Button> _buttons = new AvaloniaList<Button>();
	
	public static readonly DirectProperty<GraphNodeControl, AvaloniaList<Button>> ButtonsProperty =
		AvaloniaProperty.RegisterDirect<GraphNodeControl, AvaloniaList<Button>>(nameof(_buttons),
			control => control._buttons,
			(control, buttons) => control._buttons = buttons);

	#region Variables

	

	#endregion

	#region Event handling
	

	

	#endregion

	#region Rendering

	public override void Render(DrawingContext context)
	{
		base.Render(context);
	}

	#endregion

	#region Hittesting

	public bool HitTest(Point point)
	{
		throw new NotImplementedException();
	}

	#endregion
}