using OneWare.Essentials.Models;

namespace FEntwumS.NetlistViewer.Services;

public interface IFpgaBbService : SettingsSubscriber
{
    public string getBbCommand(IProjectFile? root = null);
}