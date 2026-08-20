using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using FEntwumS.Common.Assets;
using FEntwumS.Common.Controls;

namespace FEntwumS.Common.Builders;

public class GraphNodeBuilder : GenericGraphElementBuilder<GraphNodeControl>
{
	private string? _cellName;
	private string? _cellType;
	private StackPanel? _topLeftInteractionControls;
	private StackPanel? _topRightInteractionControls;
	private StackPanel? _topCenterInteractionControls;
	private StackPanel? _centerLeftInteractionControls;
	private StackPanel? _centerRightInteractionControls;
	private StackPanel? _centerCenterInteractionControls;
	private StackPanel? _bottomLeftInteractionControls;
	private StackPanel? _bottomRightInteractionControls;
	private StackPanel? _bottomCenterInteractionControls;
	private string? _locationPath;
	
	public new static GraphNodeBuilder Create()
	{
		return new GraphNodeBuilder();
	}

	public GraphNodeBuilder WithCellName(string cellName)
	{
		this._cellName = cellName;
		return this;
	}

	public GraphNodeBuilder WithCellType(string cellType)
	{
		this._cellType = cellType;
		return this;
	}
	
	#region Interaction control addition

	public GraphNodeBuilder WithTopLeftInteractionControl(Button button)
	{
		_topLeftInteractionControls ??= new StackPanel()
		{
			Spacing = 10.0d,
			HorizontalAlignment = HorizontalAlignment.Left,
			VerticalAlignment = VerticalAlignment.Top,
			Orientation = Orientation.Vertical,
			IsHitTestVisible = false
		};
		
		_topLeftInteractionControls.Children.Add(button);
		return this;
	}

	public GraphNodeBuilder WithTopRightInteractionControl(Button button)
	{
		_topRightInteractionControls ??= new StackPanel()
		{
			Spacing = 10.0d,
			HorizontalAlignment = HorizontalAlignment.Right,
			VerticalAlignment = VerticalAlignment.Top,
			Orientation = Orientation.Vertical,
			IsHitTestVisible = false
		};
		
		_topRightInteractionControls.Children.Add(button);
		return this;
	}

	public GraphNodeBuilder WithTopCenterInteractionControl(Button button)
	{
		_topCenterInteractionControls ??= new StackPanel()
		{
			Spacing = 10.0d,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Top,
			Orientation = Orientation.Horizontal,
			//IsHitTestVisible = false
		};
		
		_topCenterInteractionControls.Children.Add(button);
		return this;
	}
	
	public GraphNodeBuilder WithCenterLeftInteractionControl(Button button)
	{
		_centerLeftInteractionControls ??= new StackPanel()
		{
			Spacing = 10.0d,
			HorizontalAlignment = HorizontalAlignment.Left,
			VerticalAlignment = VerticalAlignment.Center,
			Orientation = Orientation.Vertical,
			IsHitTestVisible = false
		};
		
		_centerLeftInteractionControls.Children.Add(button);
		return this;
	}

	public GraphNodeBuilder WithCenterRightInteractionControl(Button button)
	{
		_centerRightInteractionControls ??= new StackPanel()
		{
			Spacing = 10.0d,
			HorizontalAlignment = HorizontalAlignment.Right,
			VerticalAlignment = VerticalAlignment.Center,
			Orientation = Orientation.Vertical,
			IsHitTestVisible = false
		};
		
		_centerRightInteractionControls.Children.Add(button);
		return this;
	}

	public GraphNodeBuilder WithCenterCenterInteractionControl(Button button)
	{
		_centerCenterInteractionControls ??= new StackPanel()
		{
			Spacing = 10.0d,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Orientation = Orientation.Horizontal,
			IsHitTestVisible = false
		};

		_centerCenterInteractionControls.Children.Add(button);
		return this;
	}
	
	public GraphNodeBuilder WithBottomLeftInteractionControl(Button button)
	{
		_bottomLeftInteractionControls ??= new StackPanel()
		{
			Spacing = 10.0d,
			HorizontalAlignment = HorizontalAlignment.Left,
			VerticalAlignment = VerticalAlignment.Bottom,
			Orientation = Orientation.Vertical,
			IsHitTestVisible = false
		};
		
		_bottomLeftInteractionControls.Children.Add(button);
		return this;
	}

	public GraphNodeBuilder WithBottomRightInteractionControl(Button button)
	{
		_bottomRightInteractionControls ??= new StackPanel()
		{
			Spacing = 10.0d,
			HorizontalAlignment = HorizontalAlignment.Right,
			VerticalAlignment = VerticalAlignment.Bottom,
			Orientation = Orientation.Vertical,
			IsHitTestVisible = false
		};
		
		_bottomRightInteractionControls.Children.Add(button);
		return this;
	}

	public GraphNodeBuilder WithBottomCenterInteractionControl(Button button)
	{
		_bottomCenterInteractionControls ??= new StackPanel()
		{
			Spacing = 10.0d,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Bottom,
			Orientation = Orientation.Horizontal,
			IsHitTestVisible = false
		};

		_bottomCenterInteractionControls.Children.Add(button);
		return this;
	}

	public GraphNodeBuilder WithInteractionControl(Button button,
		HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
		VerticalAlignment verticalAlignment = VerticalAlignment.Top)
	{
		switch (horizontalAlignment)
		{
			case HorizontalAlignment.Left:
				switch (verticalAlignment)
				{
					case VerticalAlignment.Top:
							return this.WithTopLeftInteractionControl(button);
						break;
					
					case VerticalAlignment.Bottom:
							return this.WithBottomLeftInteractionControl(button);
						break;
					
					default:
							return this.WithCenterLeftInteractionControl(button);
						break;
				}
				break;
			
			case HorizontalAlignment.Right:
				switch (verticalAlignment)
				{
					case VerticalAlignment.Top:
							return this.WithTopRightInteractionControl(button);
						break;
					
					case VerticalAlignment.Bottom:
							return this.WithBottomRightInteractionControl(button);
						break;
					
					default:
							return this.WithCenterRightInteractionControl(button);
						break;
				}
				break;
			
			default:
				switch (verticalAlignment)
				{
					case VerticalAlignment.Top:
							return this.WithTopCenterInteractionControl(button);
						break;
					
					case VerticalAlignment.Bottom:
							return this.WithBottomCenterInteractionControl(button);
						break;
					
					default:
							return this.WithCenterCenterInteractionControl(button);
						break;
				}
				break;
		}
	}
	
	#endregion

	public GraphNodeBuilder WithJumpToSourceInteractionControl(
		HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
		VerticalAlignment verticalAlignment = VerticalAlignment.Top)
	{
		// Make button
		// Add the bindings and behavior
		// add to relevant interaction control panel
		//What to do on error? Nothing???
		
		Dispatcher.UIThread.Invoke(() =>
		{
			Button jumpToSourceButton = new Button()
			{
				Padding = new Thickness(5.0d),
				CornerRadius = new CornerRadius(3.0d),
				BorderThickness = new Thickness(1.0d),
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5.0d)
			};

			jumpToSourceButton.Initialized += (sender, args) =>
			{
				bool found = jumpToSourceButton.TryGetResource("FluentIconsFilled.FullScreenZoomFilled",
					jumpToSourceButton.ActualThemeVariant, out var contentImage);

				int i = 0;
			};

			jumpToSourceButton.ActualThemeVariantChanged += (sender, args) =>
			{
				bool found = jumpToSourceButton.TryFindResource("FluentIconsFilled.FullScreenZoomFilled",
					jumpToSourceButton.ActualThemeVariant, out var contentImage);
				jumpToSourceButton.TryFindResource("ThemeBackgroundBrush",
					jumpToSourceButton.ActualThemeVariant, out var bgbrush);
				jumpToSourceButton.TryFindResource("ThemeBorderLowBrush",
					jumpToSourceButton.ActualThemeVariant, out var bobrush);

				if (found)
				{
					jumpToSourceButton.Content = new Image()
					{
						Source = (DrawingImage)contentImage!,
						Height = 16.0d
					};
				
					((Visual)jumpToSourceButton.Content).ZIndex = jumpToSourceButton.ZIndex;

					// jumpToSourceButton.Content = "Hallo";
					jumpToSourceButton.BorderBrush = (IBrush?)bobrush;
					jumpToSourceButton.Background = (IBrush?)bgbrush;
				}

				int i = 0;
			};

			// jumpToSourceButton.Bind(ContentControl.ContentProperty,
			// 	jumpToSourceButton.GetResourceObservable("FluentIconsFilled.FullScreenZoomFilled"));
		
			jumpToSourceButton.Click += JumpToSourceButtonOnClick;

			this.WithInteractionControl(jumpToSourceButton, horizontalAlignment, verticalAlignment);
		});

		return this;
	}

	private void JumpToSourceButtonOnClick(object? sender, RoutedEventArgs e)
	{
		return;
		throw new NotImplementedException();
	}

	public GraphNodeBuilder WithExpandCollapseInteractionControlIf(bool condition,
		HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
		VerticalAlignment verticalAlignment = VerticalAlignment.Top)
	{
		if (condition)
		{
			return this.WithExpandCollapseInteractionControl(horizontalAlignment, verticalAlignment);
		}

		return this;
	}

	public GraphNodeBuilder WithExpandCollapseInteractionControl(
		HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
		VerticalAlignment verticalAlignment = VerticalAlignment.Top)
	{
		
		
		// Make button
		// Add the bindings and behavior
		// add to relevant interaction control panel
		//What to do on error? Nothing???

		Dispatcher.UIThread.Invoke(() =>
		{
			Button expandCollapseButton = new Button()
			{
				Padding = new Thickness(5.0d),
				CornerRadius = new CornerRadius(3.0d),
				BorderThickness = new Thickness(1.0d),
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5.0d)
			};

			expandCollapseButton.Initialized += (sender, args) =>
			{
				bool found = expandCollapseButton.TryGetResource("FluentIcons.add_regular",
					expandCollapseButton.ActualThemeVariant, out var contentImage);

				int i = 0;
			};

			expandCollapseButton.ActualThemeVariantChanged += (sender, args) =>
			{
				expandCollapseButton.TryFindResource("ThemeBackgroundBrush",
					expandCollapseButton.ActualThemeVariant, out var bgbrush);
				expandCollapseButton.TryFindResource("ThemeBorderLowBrush",
					expandCollapseButton.ActualThemeVariant, out var bobrush);

				expandCollapseButton.Content = new PathIcon()
				{
					Data = AppIcons.PLUS,
					 Height = 16.0d
				};
			
				((Visual)expandCollapseButton.Content).ZIndex = expandCollapseButton.ZIndex;

				// jumpToSourceButton.Content = "Hallo";
				expandCollapseButton.BorderBrush = (IBrush?)bobrush;
				expandCollapseButton.Background = (IBrush?)bgbrush;

				int i = 0;
			};
		
			expandCollapseButton.Click += ExpandCollapseButtonOnClick;

			this.WithInteractionControl(expandCollapseButton, horizontalAlignment, verticalAlignment);
		});

		return this;
	}
	
	private void ExpandCollapseButtonOnClick(object? sender, RoutedEventArgs e)
	{
		return;
		throw new NotImplementedException();
	}

	public GraphNodeBuilder WithLocationPath(string locationPath)
	{
		this._locationPath = locationPath;
		return this;
	}

	public GraphNodeControl Build()
	{
		GraphNodeControl c = null;
		Dispatcher.UIThread.Invoke(() =>
		{
			c = base.Build();
			c.CellName = this._cellName ?? "";
			c.CellType = this._cellType ?? "";
			c.LocationPath =  this._locationPath;
			c.IsHitTestVisible = true;
			
			int interactionControlZIndex = c.ZIndex + 1;

			if (_topLeftInteractionControls is not null)
			{
				foreach (Control child in _topLeftInteractionControls.Children)
				{
					child.ZIndex = interactionControlZIndex;
				}
				
				c._interactionControls.Add(_topLeftInteractionControls);
			}

			if (_topRightInteractionControls is not null)
			{
				foreach (Control child in _topRightInteractionControls.Children)
				{
					child.ZIndex = interactionControlZIndex;
				}
				
				c._interactionControls.Add(_topRightInteractionControls);
			}

			if (_topCenterInteractionControls is not null)
			{
				foreach (Control child in _topCenterInteractionControls.Children)
				{
					child.ZIndex = interactionControlZIndex;
				}
				
				c._interactionControls.Add(_topCenterInteractionControls);
			}
			
			if (_bottomLeftInteractionControls is not null)
			{
				foreach (Control child in _bottomLeftInteractionControls.Children)
				{
					child.ZIndex = interactionControlZIndex;
				}
				
				c._interactionControls.Add(_bottomLeftInteractionControls);
			}

			if (_bottomRightInteractionControls is not null)
			{
				foreach (Control child in _bottomRightInteractionControls.Children)
				{
					child.ZIndex = interactionControlZIndex;
				}
				
				c._interactionControls.Add(_bottomRightInteractionControls);
			}

			if (_bottomCenterInteractionControls is not null)
			{
				foreach (Control child in _bottomCenterInteractionControls.Children)
				{
					child.ZIndex = interactionControlZIndex;
				}
				
				c._interactionControls.Add(_bottomCenterInteractionControls);
			}
			
			if (_centerLeftInteractionControls is not null)
			{
				foreach (Control child in _centerLeftInteractionControls.Children)
				{
					child.ZIndex = interactionControlZIndex;
				}
				
				c._interactionControls.Add(_centerLeftInteractionControls);
			}

			if (_centerRightInteractionControls is not null)
			{
				foreach (Control child in _centerRightInteractionControls.Children)
				{
					child.ZIndex = interactionControlZIndex;
				}
				
				c._interactionControls.Add(_centerRightInteractionControls);
			}

			if (_centerCenterInteractionControls is not null)
			{
				foreach (Control child in _centerCenterInteractionControls.Children)
				{
					child.ZIndex = interactionControlZIndex;
				}
				
				c._interactionControls.Add(_centerCenterInteractionControls);
			}
		});

		return c;
	}
}