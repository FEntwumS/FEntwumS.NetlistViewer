using Avalonia;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Rendering;
using FEntwumS.Common.Types;

namespace FEntwumS.Common.Controls;

public class GraphJunctionControl : GenericGraphElementControl, ICustomHitTest
{
	#region Properties

	private JunctionShape _junctionShape = JunctionShape.Circle;
	
	/// <summary>
	/// The shape requested for this junction
	/// </summary>
	public static readonly StyledProperty<JunctionShape> JunctionShapeProperty =
		AvaloniaProperty.Register<GraphJunctionControl, JunctionShape>(nameof(_junctionShape),
			defaultBindingMode: BindingMode.TwoWay,
			defaultValue: JunctionShape.Circle);

	#endregion

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