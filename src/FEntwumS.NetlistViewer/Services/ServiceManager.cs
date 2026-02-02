using OneWare.Essentials.Services;

namespace FEntwumS.NetlistViewer.Services;

public class ServiceManager
{
	public static IJsonLoader GetJsonLoader()
	{
		return ContainerLocator.Current.Resolve<IJsonLoader>();
	}

	public static IViewportDimensionService? GetViewportDimensionService()
	{
		return ContainerLocator.Current.Resolve<IViewportDimensionService>();
	}

	public static ICustomLogger GetCustomLogger()
	{
		return ContainerLocator.Current.Resolve<ICustomLogger>();
	}

	public static ILanguageManager GetLanguageManager()
	{
		return ContainerLocator.Current.Resolve<ILanguageManager>();
	}

	public static IHashService GetHashService()
	{
		return ContainerLocator.Current.Resolve<IHashService>();
	}

	public static T GetService<T>()
	{
		return ContainerLocator.Current.Resolve<T>();
	}
}