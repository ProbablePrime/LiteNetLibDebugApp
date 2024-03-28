using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LiteNetLibDebugApp;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHostedSingleton<THostedService>(this IServiceCollection services)
            where THostedService : class, IHostedService
    {
        // https://stackoverflow.com/questions/52036998/how-do-i-get-a-reference-to-an-ihostedservice-via-dependency-injection-in-asp-ne
        // Basically registers it twice, but once as a hosted service and once as a singleton. 
        services.AddSingleton<THostedService>();
        services.AddHostedService(provider => provider.GetRequiredService<THostedService>());

        return services;
    }
}
