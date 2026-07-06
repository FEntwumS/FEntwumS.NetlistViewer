using FEntwumS.Common.Interfaces;
using OneWare.Essentials.Services;

namespace FEntwumS.Common.Services;

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

	public static IHashService GetHashService()
	{
		return ContainerLocator.Current.Resolve<IHashService>();
	}

	public static T GetService<T>()
	{
		return ContainerLocator.Current.Resolve<T>();
	}
}