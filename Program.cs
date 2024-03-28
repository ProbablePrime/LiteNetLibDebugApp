
using LiteNetLibDebugApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    static CancellationTokenSource stopRunning = new CancellationTokenSource();
    static bool ShouldRun => !stopRunning.IsCancellationRequested;
    public async static Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        // Force LNLNetLoggerAdapterToBeCreated // TODO: Is this the best path?
        var logger = host.Services.GetService<LNLNetLoggerAdapter>();

        await host.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            var env = hostingContext.HostingEnvironment;

            // What does this actually do?
            config.AddEnvironmentVariables();

            //TODO: Can we make this automatic?
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
        })
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<LNLNetLoggerAdapter>();
            services.AddOptions<ServerServiceOptions>().Bind(hostContext.Configuration.GetSection(nameof(ServerServiceOptions)));
            services.AddOptions<ClientServiceOptions>().Bind(hostContext.Configuration.GetSection(nameof(ClientServiceOptions)));

            //TODO: can we do this in a better way, its got a lot of. stuff
            var appOptions = hostContext.Configuration.GetSection(nameof(AppOptions)).Get<AppOptions>();
            
            if (appOptions?.ClientEnabled ?? true)
            {
                services.AddHostedSingleton<ClientService>();
                services.AddHostedSingleton<InputService>();
            }
                
            if (appOptions?.ServerEnabled ?? true)
            {
                services.AddHostedSingleton<ServerService>();
            }
            
            services.AddLogging();
        }).UseConsoleLifetime();
}


