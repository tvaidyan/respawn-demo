using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace RespawnDemo.IntegrationTests.Shared;
public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
{
    public Action<IServiceCollection> Registrations { get; set; }

    public CustomWebApplicationFactory() : this(null)
    {
    }

    public CustomWebApplicationFactory(Action<IServiceCollection> registrations = null)
    {
        Registrations = registrations ?? (collection => { });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            Registrations?.Invoke(services);
        });
    }
}