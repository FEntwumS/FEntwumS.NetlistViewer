namespace FEntwumS.NetlistReaderFrontend.Services;

public interface IFpgaBbService
{
    public void SubscribeToSettings();
    
    public string getBbCommand();
}