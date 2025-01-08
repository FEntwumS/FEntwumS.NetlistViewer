using OneWare.Vcd.Parser.Data;
using OneWare.Vcd.Viewer.Models;

namespace FEntwumS.WfInteractor.Common;

public interface IWaveformInteractorService
{
    // Methods communicating with backend 
    public Task<string> TransferSignalStates(ExtendedVcdScopeModel.ExtendedSignal[] signals);
    
    // Methods communicating directly via shared Interface with diagram component
    public void GoToSignal(string signalName);
    
    
    // Methods that need to be implemented in a shared interface offered by OneWare
    public void getSignals(string[] signalNames);
}
