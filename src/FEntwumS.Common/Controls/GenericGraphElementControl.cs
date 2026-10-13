using Avalonia;
using Avalonia.Data;
using FEntwumS.Common.Types;

namespace FEntwumS.Common.Controls;

public abstract class GenericGraphElementControl : PositionableSubControl
{
	#region Properties

	public NetlistTheme NetlistTheme
	{
		get => GetValue(NetlistThemeProperty);
		set
		{
			SetValue(NetlistThemeProperty, value);
		}
	}
	
	public static readonly StyledProperty<NetlistTheme> NetlistThemeProperty =
		AvaloniaProperty.Register<GenericGraphElementControl, NetlistTheme>(nameof(NetlistTheme),
			defaultBindingMode: BindingMode.TwoWay,
			inherits: true);

	public string srcLocation
	{
		get => GetValue(SrcLocationProperty);
		set => SetValue(SrcLocationProperty, value);
	}

	public static readonly StyledProperty<string> SrcLocationProperty =
		AvaloniaProperty.Register<GenericGraphElementControl, string>(nameof(srcLocation),
			defaultBindingMode: BindingMode.TwoWay,
			defaultValue: "");
	
	#endregion

	#region Event Handling

	#endregion
	
	#region Rendering

	protected override Size MeasureCore(Size availableSize)
	{
		return Double.IsNaN(ElementWidth) || Double.IsNaN(ElementHeight) ? new Size(100, 100) : new Size(ElementWidth, ElementHeight);
	}

	protected override void ArrangeCore(Rect finalRect)
	{
		Bounds = finalRect;
	}

	#endregion

}