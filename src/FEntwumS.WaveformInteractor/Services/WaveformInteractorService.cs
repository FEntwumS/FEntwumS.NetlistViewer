using FEntwumS.Common;
using OneWare.Vcd.Viewer.Models;

// namespace Fentwums.Common;

public class WaveformInteractorService : IWaveformInteractorService
{
    // transmit all signal states to frontend
    public Task<string> TransferSignalStates(ExtendedVcdScopeModel.ExtendedSignal[] signals)
    {
        throw new NotImplementedException();
    }

    public void GoToSignal(int bitIndex)
    {
        Console.WriteLine("GO TO SIGNAL");
        throw new NotImplementedException();
    }

    public void getSignals(string[] signalNames)
    {
        throw new NotImplementedException();
    }
}