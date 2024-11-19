using Prism.Ioc;

namespace Oneware.NetlistReaderFrontend.Services;

public class ServiceManager
{
    private static IContainerProvider _containerProvider;
    
    public ServiceManager(IContainerProvider containerProvider)
    {
        _containerProvider = containerProvider;
    }

    public static IJsonLoader GetJsonLoader()
    {
        return _containerProvider.Resolve<IJsonLoader>();
    }

    public static IViewportDimensionService GetViewportDimensionService()
    {
        return _containerProvider.Resolve<IViewportDimensionService>();
    }

    public static IFileOpener GetFileOpener()
    {
        return _containerProvider.Resolve<IFileOpener>();
    }
}