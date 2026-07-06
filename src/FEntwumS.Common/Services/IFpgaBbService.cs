namespace FEntwumS.Common.Services;

public interface IFpgaBbService
{
    public void SubscribeToSettings();
    
    public string getBbCommand();
}