using OneWare.Essentials.Models;

namespace FEntwumS.NetlistViewer.Services;

public interface IFpgaBbService : ISettingsSubscriber
{
	public string getBbCommand(IProjectFile? root = null);
}