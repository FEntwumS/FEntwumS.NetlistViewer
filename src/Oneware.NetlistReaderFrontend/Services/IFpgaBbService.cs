namespace Oneware.NetlistReaderFrontend.Services;

public interface IFpgaBbService
{
    public void SubscribeToSettings();
    
    public string getBbCommand();
}