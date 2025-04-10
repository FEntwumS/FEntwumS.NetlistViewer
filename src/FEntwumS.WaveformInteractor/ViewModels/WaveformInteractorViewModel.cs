using System.ComponentModel;
using OneWare.Essentials.ViewModels;

namespace FEntwumS.WaveformInteractor.ViewModels;

public class WaveformInteractorViewModel : ExtendedTool
{
    private string _text = "FEntwumS Waveform Interactor";

    public WaveformInteractorViewModel() : base("WaveformInteractor")
    {
    }

    public string Text
    {
        get => _text;
        set
        {
            if (_text != value)
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
    }

    public void Button1Clicked() => Text = "Button 1 clicked";
    public void Button2Clicked() => Text = "Button 2 clicked";
    public void Button3Clicked() => Text = "Button 3 clicked";
}
