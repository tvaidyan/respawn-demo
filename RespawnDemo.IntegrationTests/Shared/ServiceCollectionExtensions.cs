using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Microsoft.Data.SqlClient;

namespace RespawnDemo.IntegrationTests.Shared;
public static class ServiceCollectionExtensions
{
    public static void SwapTransient<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
    {
        if (services.Any(x => x.ServiceType == typeof(TService) && x.Lifetime == ServiceLifetime.Transient))
        {
            var serviceDescriptors = services.Where(x => x.ServiceType == typeof(TService) && x.Lifetime == ServiceLifetime.Transient).ToList();
            foreach (var serviceDescriptor in serviceDescriptors)
            {
                services.Remove(serviceDescriptor);
            }
        }

        services.AddTransient(typeof(TService), (sp) => implementationFactory(sp));
    }

    public static void RemoveService(this IServiceCollection services, string serviceName)
    {
        var serviceDescriptors = services.Where(x => x.ServiceType.FullName.Contains(serviceName)).ToList();
        foreach (var serviceDescriptor in serviceDescriptors)
        {
            services.Remove(serviceDescriptor);
        }
    }

    public static void SetupDatabaseConnection(this IServiceCollection services, string connectionSring)
    {
        services.RemoveService("IDbConnection");
        services.AddScoped<IDbConnection, SqlConnection>(ctx =>
        {
            var connection = new SqlConnection(connectionSring);
            return connection;
        });
    }
}
