using System.Text;
using FEntwumS.WfInteractor.Common;
using Newtonsoft.Json;
using OneWare.Vcd.Viewer.Models;

// namespace Fentwums.Common;

public class WaveformInteractorService : IWaveformInteractorService
{
    
    
    public WaveformInteractorService()
    {
        Console.WriteLine($"Instance of {nameof(IWaveformInteractorService)} created at {DateTime.Now}");
    }
    
    // transmit all signal states to frontend
    public Task<string> TransferSignalStates(ExtendedVcdScopeModel.ExtendedSignal[] signals)
    {
        throw new NotImplementedException();
    }

    void FEntwumS.WfInteractor.Common.IWaveformInteractorService.GoToSignal(string signalName)
    {
        throw new NotImplementedException();
    }

    public void getSignals(string[] signalNames)
    {
        throw new NotImplementedException();
    }
}