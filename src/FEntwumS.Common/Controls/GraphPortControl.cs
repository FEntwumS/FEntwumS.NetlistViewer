using Avalonia;
using Avalonia.Data;
using FEntwumS.Common.Types;

namespace FEntwumS.Common.Controls;

public class GraphPortControl : GenericGraphElementControl
{
	#region Properties

	private PortShape _portShape = PortShape.Square;
	
	public static readonly StyledProperty<PortShape> PortShapeProperty =
		AvaloniaProperty.Register<GraphPortControl, PortShape>(nameof(_portShape),
			defaultBindingMode: BindingMode.TwoWay,
			defaultValue: PortShape.Square);

	#endregion

	#region Variables



	#endregion

	#region Event handling




	#endregion

	#region Rendering



	#endregion
}