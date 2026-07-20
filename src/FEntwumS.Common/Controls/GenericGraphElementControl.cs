using Avalonia;
using Avalonia.Data;
using FEntwumS.Common.Types;

namespace FEntwumS.Common.Controls;

public class GenericGraphElementControl : PositionableSubControl
{
	#region Properties
	
	public NetlistTheme NetlistTheme { get; set; }
	
	public static readonly StyledProperty<NetlistTheme> NetlistThemeProperty =
		AvaloniaProperty.Register<GenericGraphElementControl, NetlistTheme>(nameof(NetlistTheme),
			defaultBindingMode: BindingMode.TwoWay);
	
	public string srcLocation { get; set; } = "";

	public static readonly StyledProperty<string> SrcLocationProperty =
		AvaloniaProperty.Register<GenericGraphElementControl, string>(nameof(srcLocation),
			defaultBindingMode: BindingMode.TwoWay,
			defaultValue: "");
	
	#endregion

	#region Event Handling

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == ScaleProperty)
		{
			NetlistTheme.RegenerateBrushesAndPens();
		}

		if (e.Property == ParentProperty)
		{
			var newParent = e.NewValue;

			if (newParent is PanningControl { Child: GenericGraphElementControl childControl })
				childControl.PropertyChanged += (sender, args) =>
				{
					if (args.Property == ScaleProperty)
					{
						childControl.NetlistTheme.RegenerateBrushesAndPens();
					}
				};
		}
		
		base.OnPropertyChanged(e);
	}

	#endregion

}