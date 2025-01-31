namespace FEntwumS.WaveformInteractor.Services;

using OneWare.Essentials.Models;

public interface IYosysSimService
{
    Task<bool> LoadVerilogAsync(IProjectFile file);
}