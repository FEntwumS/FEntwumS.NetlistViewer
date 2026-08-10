using Avalonia;
using Avalonia.Data;
using FEntwumS.Common.Types;

namespace FEntwumS.Common.Controls;

public class GenericGraphElementControl : PositionableSubControl
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

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == ParentProperty)
		{
			var newParent = e.NewValue;

			if (newParent is PanningControl { Child: GenericGraphElementControl childControl })
				childControl.PropertyChanged += (sender, args) =>
				{
					if (args.Property == ScaleProperty && args.NewValue is double newScale)
					{
						childControl.NetlistTheme.RegenerateBrushesAndPens(newScale);
					}
				};
		}
		
		base.OnPropertyChanged(e);
	}

	#endregion
	
	#region Rendering

	protected override Size MeasureCore(Size availableSize)
	{
		return Double.IsNaN(Width) || Double.IsNaN(Height) ? new Size(100, 100) : new Size(Width, Height);
	}

	protected override void ArrangeCore(Rect finalRect)
	{
		Bounds = finalRect;
	}

	#endregion

}