namespace FEntwumS.NetlistViewer.Services;

public interface IFpgaBbService
{
    public void SubscribeToSettings();
    
    public string getBbCommand();
}