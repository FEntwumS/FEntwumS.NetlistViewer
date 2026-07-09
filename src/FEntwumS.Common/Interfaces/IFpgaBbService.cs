using OneWare.Essentials.Models;

namespace FEntwumS.Common.Interfaces;

public interface IFpgaBbService : ISettingsSubscriber
{
	public string getBbCommand(IProjectFile? root = null);
}