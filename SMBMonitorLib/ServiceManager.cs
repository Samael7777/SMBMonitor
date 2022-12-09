using Microsoft.Extensions.DependencyInjection;
using SmbMonitorLib.Interfaces;
using SmbMonitorLib.Services;

namespace SmbMonitorLib;

public static class ServiceManager
{
    private static readonly IServiceCollection services;
    private static readonly IServiceProvider serviceProvider;

    static ServiceManager()
    {
        services = new ServiceCollection();
        ConfigureServices();
        serviceProvider = services.BuildServiceProvider();
    }

    private static void ConfigureServices()
    {
        services.AddSingleton<ISmbService, SmbService>();
        services.AddSingleton<IHostMonitoringService, HostMonitoringService>();
        services.AddSingleton<ISmbMonitoringService, SmbMonitoringService>();
        services.AddSingleton<IWifiMonitoringService, WifiMonitoringService>();
        services.AddSingleton<IExternalSharesMonitorService, ExternalSharesMonitoringService>();
        services.AddSingleton<IStorageService, StorageService>();
        services.AddSingleton<IDiskLettersService, DiskLettersService>();
        services.AddSingleton<ISettings, Settings>();
    }

    public static void StartServices()
    {
        var list = GetServicesAs<IControlledService>();
        list.ForEach(s => s.Start());
    }

    public static void StopServices()
    {
        var list = GetServicesAs<IControlledService>();
        list.ForEach(s => s.Stop());
    }

    public static T? GetService<T>()
        where T : class
    {
        return serviceProvider.GetService(typeof(T)) as T;
    }

    public static List<T> GetServicesAs<T>()
        where T : class
    {
        var result = new List<T>();
        foreach (var serviceDescriptor in services)
            if (serviceProvider.GetService(serviceDescriptor.ServiceType) is T service)
                result.Add(service);

        return result;
    }
}