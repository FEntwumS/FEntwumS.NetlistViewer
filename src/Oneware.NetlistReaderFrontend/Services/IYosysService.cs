namespace Oneware.NetlistReaderFrontend.Services;

public interface IYosysService
{
    Task LoadVhdlAsync();
    Task LoadVerilogAsync();
    Task CreateJsonNetlistAsync();
}