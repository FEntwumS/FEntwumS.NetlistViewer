using OneWare.Essentials.Models;

namespace FEntwumS.NetlistViewer.Services;

public interface IFpgaBbService
{
    public void SubscribeToSettings();
    
    public string getBbCommand(IProjectFile? root = null);
}