using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FEntwumS.WaveformInteractor.ViewModels;

namespace FEntwumS.WaveformInteractor.Views;

public partial class WaveformInteractorView : UserControl
{
    private WaveformInteractorViewModel? _viewModel;

    public WaveformInteractorView()
    {
        InitializeComponent();

        if (DataContext is WaveformInteractorViewModel vm)
        {
            Initialize(vm);
        }

        DataContextChanged += OnDataContextChanged;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == DataContextProperty)
        {
            if (_viewModel == null)
            {
                _viewModel = e.NewValue as WaveformInteractorViewModel;
            }
        }

        base.OnPropertyChanged(e);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (IsInitialized)
        {
            _viewModel = DataContext as WaveformInteractorViewModel;
        }
        else
        {
            Initialized += delegate { OnDataContextChanged(sender, e); };
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private void Initialize(WaveformInteractorViewModel vm)
    {
        _viewModel = vm;
    }

    // Button click event handlers to update the Text
    private void Button1_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _viewModel.Button1Clicked();
    }

    private void Button2_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _viewModel.Button2Clicked();
    }

    private void Button3_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _viewModel.Button3Clicked();
    }
}
