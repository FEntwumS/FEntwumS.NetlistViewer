using Avalonia;
using Avalonia.Data;
using FEntwumS.Common.Types;

namespace FEntwumS.Common.Controls;

public class GenericGraphElementControl : PositionableSubControl
{
	#region Properties
	
	public NetlistTheme NetlistTheme { get; set; }
	
	public static readonly StyledProperty<NetlistTheme> ThemeProperty =
		AvaloniaProperty.Register<GenericGraphElementControl, NetlistTheme>(nameof(NetlistTheme),
			defaultBindingMode: BindingMode.TwoWay);
	
	#endregion
}