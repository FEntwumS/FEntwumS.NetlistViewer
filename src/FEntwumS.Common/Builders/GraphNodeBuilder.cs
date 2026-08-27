using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Input;
using FEntwumS.Common.Assets;
using FEntwumS.Common.Controls;
using FEntwumS.Common.Interfaces;
using FEntwumS.Common.Services;
using OneWare.Essentials.Helpers;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;

namespace FEntwumS.Common.Builders;

public partial class GraphNodeBuilder : GenericGraphElementBuilder<GraphNodeControl>
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
	private ContextMenu? _contextMenu;
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
				CornerRadius = new CornerRadius(3.0d),
				BorderThickness = new Thickness(1.0d),
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Center
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
				CornerRadius = new CornerRadius(3.0d),
				BorderThickness = new Thickness(1.0d),
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Center,
				RenderTransformOrigin = new RelativePoint(0.5d, 0.0d, RelativeUnit.Relative),
			};

			expandCollapseButton.AttachedToVisualTree += (sender, args) =>
			{
				// Try to find the parent graphnode
				
				Visual? ancestor = expandCollapseButton.GetVisualParent();
				bool done = false;
				GraphNodeControl? directAncestorNode = null;

				while (!done)
				{
					if (ancestor is null)
					{
						return;
					}

					if (ancestor is GraphNodeControl graphNodeControl)
					{
						directAncestorNode = graphNodeControl;
						done = true;
					}
					else
					{
						ancestor = ancestor.GetVisualParent();
					}
				}

				if (directAncestorNode is not null)
				{
					if (directAncestorNode.Items.Any(g => g is not GraphLabelControl))
					{
						(expandCollapseButton.Content as PathIcon)?.Data = AppIcons.MINUS;
					}
					
					directAncestorNode.PropertyChanged += (o, eventArgs) =>
					{
						if (eventArgs.Property == PositionableSubControl.ScaleProperty &&
						    expandCollapseButton.Content is PathIcon buttonContent)
						{
							double newScale = (double)eventArgs.NewValue!;
							double oldScale = (double)eventArgs.OldValue!;
							double scaleDifference = newScale / oldScale;
							
							expandCollapseButton.RenderTransform = new ScaleTransform(newScale, newScale);
							// TODO apply rendertransform to stackpanel to greatly simplify layout when several interactioncontrols are used in one location
						}
					};
				}
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

				expandCollapseButton.BorderBrush = (IBrush?)bobrush;
				expandCollapseButton.Background = (IBrush?)bgbrush;
			};
		
			expandCollapseButton.Click += ExpandCollapseButtonOnClick;

			this.WithInteractionControl(expandCollapseButton, horizontalAlignment, verticalAlignment);
		});

		return this;
	}

	private void ExpandCollapseButtonOnClick(object? sender, RoutedEventArgs e)
	{
		if (sender is Button expandCollapseButton)
		{
			bool done = false;
			
			Visual? sendingControl = expandCollapseButton.GetVisualParent();
			GraphNodeControl? sendingNode = null;
			PanningNetlistControl? sendingPanningNetlist = null;

			while (!done)
			{
				if (sendingControl is GraphNodeControl graphNode)
				{
					done = true;
					sendingNode = graphNode;
				}

				if (sendingControl is null)
				{
					return;
				}
				else
				{
					sendingControl = sendingControl.GetVisualParent();
				}
			}

			done = false;
			
			sendingControl = sendingNode.GetVisualParent();

			while (!done)
			{
				if (sendingControl is PanningNetlistControl netlistControl)
				{
					done = true;
					sendingPanningNetlist = netlistControl;
				}

				if (sendingControl is null)
				{
					return;
				}
				else
				{
					sendingControl = sendingControl.GetVisualParent();
				}
			}
			
			var f = (async () =>
			{
				GenericGraphElementControl? newGraphNode = null;
				GraphNodeControl? clickedNode = null;
				
				(newGraphNode, clickedNode) = await ServiceManager.GetService<IFrontendService>()
					.ExpandNodeAsync(sendingNode.LocationPath, sendingPanningNetlist.NetlistId);
				
				if (newGraphNode is not null)
				{
					await Dispatcher.UIThread.InvokeAsync(() =>
					{
						sendingPanningNetlist.Child = newGraphNode;

						if (clickedNode is not null)
						{
							double absoluteX = clickedNode.X,  absoluteY = clickedNode.Y;
							bool done = false;
							Visual? parent = clickedNode.GetVisualParent();

							while (!done)
							{
								if (parent is not GraphNodeControl parentNode)
								{
									done = true;
								}
								else
								{
									absoluteX += parentNode.X;
									absoluteY += parentNode.Y;
									
									parent = parentNode.GetVisualParent();
								}
							}

							sendingPanningNetlist.ZoomBounds = new Rect(new Point(absoluteX, absoluteY),
								new Size(clickedNode.Width, clickedNode.Height));
						}
						else
						{
							sendingPanningNetlist.ZoomToFit();
						}
					});

				}
			});

			_ = f.Invoke();
		}
	}

	public GraphNodeBuilder WithLocationPath(string locationPath)
	{
		this._locationPath = locationPath;
		return this;
	}

	public GraphNodeBuilder WithJumpToSourceContextMenuAction()
	{
		if (_contextMenu is null)
		{
			Dispatcher.UIThread.Invoke(() =>
			{
				_contextMenu = new ContextMenu();
			});
		}

		Dispatcher.UIThread.Invoke(() =>
		{
			var currentMenuItems = _contextMenu.ItemsSource;

			if (currentMenuItems is null)
			{
				_contextMenu.ItemsSource = new[]
				{
					new MenuItem() { Header = "Jump To Source", Command = JumpToSourceCommand }
				};
			}
			else
			{
				List<MenuItem> newItems = new List<MenuItem>();
				
				newItems.AddRange(_contextMenu.ItemsSource as IEnumerable<MenuItem> ?? Array.Empty<MenuItem>());
				newItems.Add(new MenuItem() { Header = "Jump To Source", Command = JumpToSourceCommand });

				_contextMenu.ItemsSource = newItems;
			}
		});

		return this;
	}

	[RelayCommand]
	private async void JumpToSource(object? parameter)
	{
		if (parameter is GraphNodeControl graphNodeControl)
		{
			Visual? sendingControl = graphNodeControl.GetVisualParent();
			PanningNetlistControl? sendingPanningNetlist = null;
			bool done = false;
			
			while (!done)
			{
				if (sendingControl is PanningNetlistControl netlistControl)
				{
					done = true;
					sendingPanningNetlist = netlistControl;
				}

				if (sendingControl is null)
				{
					return;
				}
				else
				{
					sendingControl = sendingControl.GetVisualParent();
				}
			}

			if (sendingPanningNetlist is null)
			{
				return;
			}
			
			string srcLine = graphNodeControl.srcLocation;

			ulong NetlistID = sendingPanningNetlist.NetlistId;
			
			
			if (srcLine is null || srcLine == "")
	        {
	            return;
	        }

	        string?[] srclineSplit = srcLine.Split('|');

	        srcLine = srclineSplit.First();

	        int lastpos = -1;

	        if (PlatformHelper.Platform is PlatformId.WinArm64 or PlatformId.WinX64)
	        {
	            lastpos = srcLine!.LastIndexOfAny([':']);
	        } else if (PlatformHelper.Platform is not PlatformId.Unknown or PlatformId.Wasm)
	        {
	            lastpos = srcLine!.IndexOf(':');
	        }
	        
	        if (lastpos == -1)
	        {
	            lastpos = srcLine!.Length - 1;
	        }

	        long line = 1;
	        string filename = "";

	        // PMUXes somehow have the actual src attribute set two times; While both contain the correct source file, the
	        // first does not contain the line number. Only the second does

	        for (int i = 0; i < 2; i++)
	        {
	            filename = srcLine!.Substring(0, lastpos);
	            string lines = srcLine.Substring(lastpos + 1);
	            string[] linesSplit = lines.Split('.');

	            if (linesSplit.Length > 0)
	            {
	                try
	                {
	                    line = long.Parse(linesSplit[0]);
	                }
	                catch (Exception)
	                {
	                    line = 1;
	                }
	            }

	            if (line == 0)
	            {
	                if (srclineSplit.Length > 1)
	                {
	                    srcLine = srclineSplit[1];
	                }
	            }
	            else
	            {
	                break;
	            }
	        }

	        (string vhdlFilename, long vhdlLine, bool success) = ServiceManager.GetService<ICcVhdlFileIndexService>()
	            .GetActualSource(line, NetlistID);

	        if (success)
	        {
	            filename = vhdlFilename;
	            line = vhdlLine;
	        }

	        if (filename[0] != '/' && filename.Substring(1, 2) != ":\\")
	        {
		        filename = Path.Combine(sendingPanningNetlist.ProjectRootFolder, filename);
	        }

	        if (File.Exists(filename))
	        {
		        var ds = ServiceManager.GetService<IMainDockService>();

		        var document = await ds.OpenFileAsync(filename);

		        (document as IEditor)?.JumpToLine((int)line);
	        }
		}
	}

	// private ICommand? JumpToSourceCommand = new AsyncRelayCommand(async (x) =>
	// {
	// 	int i = 0;
	// });

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
			
			int interactionControlZIndex = c.ZIndex + 3;

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

			if (_contextMenu is not null)
			{
				foreach (var i in _contextMenu.ItemsSource)
				{
					if (i is MenuItem menuItem)
					{
						menuItem.CommandParameter = c;
					}
				}
				
				c.ContextMenu = _contextMenu;
			}
		});

		return c;
	}
}