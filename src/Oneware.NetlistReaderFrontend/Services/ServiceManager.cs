﻿using OneWare.Essentials.Services;
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

    public static ILogger GetLogger()
    {
        return _containerProvider.Resolve<ILogger>();
    }

    public static IFileOpener GetFileOpener()
    {
        return _containerProvider.Resolve<IFileOpener>();
    }

    public static ICustomLogger GetCustomLogger()
    {
        return _containerProvider.Resolve<ICustomLogger>();
    }

    public static ILanguageManager GetLanguageManager()
    {
        return _containerProvider.Resolve<ILanguageManager>();
    }
}