using OneWare.Essentials.Services;
using Prism.Ioc;

namespace FEntwumS.NetlistViewer.Services;

public class ServiceManager
{
	private static IContainerProvider? _containerProvider;

	public ServiceManager(IContainerProvider? containerProvider)
	{
		_containerProvider = containerProvider;
	}

	public static IJsonLoader GetJsonLoader()
	{
		return _containerProvider.Resolve<IJsonLoader>();
	}

	public static IViewportDimensionService? GetViewportDimensionService()
	{
		return _containerProvider.Resolve<IViewportDimensionService>();
	}

	public static ILogger GetLogger()
	{
		return _containerProvider.Resolve<ILogger>();
	}

	public static ICustomLogger GetCustomLogger()
	{
		return _containerProvider.Resolve<ICustomLogger>();
	}

	public static ILanguageManager GetLanguageManager()
	{
		return _containerProvider.Resolve<ILanguageManager>();
	}

	public static IHashService GetHashService()
	{
		return _containerProvider.Resolve<IHashService>();
	}

	public static T GetService<T>()
	{
		return _containerProvider.Resolve<T>();
	}
}