using OneWare.Vcd.Parser.Data;
using OneWare.Vcd.Viewer.Models;

namespace FEntwumS.Common.Services;

public interface IWaveformInteractorService
{
    // Methods communicating directly via shared Interface with diagram component
    public Task<string> TransferSignalStatesAsync(ExtendedVcdScopeModel.ExtendedSignal[] signals);
    
    public void GoToSignal(int bitIndex);
}
