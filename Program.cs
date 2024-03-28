
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

        await host.StartAsync();

        // Force LNLNetLoggerAdapterToBeCreated // TODO: Is this the best path?
        var logger = host.Services.GetService<LNLNetLoggerAdapter>();
        var sender = host.Services.GetService<ClientService>();

        while (ShouldRun)
        {
            var line = Console.ReadLine()?.ToLower();
            if (line == null)
                continue;

            if (!string.IsNullOrEmpty(line) && sender != null)
                sender.Send(line);
        }

        await host.StopAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            var env = hostingContext.HostingEnvironment;

            config.AddEnvironmentVariables();
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
        })
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<LNLNetLoggerAdapter>();
            services.AddOptions<ServerServiceOptions>().Bind(hostContext.Configuration.GetSection(nameof(ServerServiceOptions)));
            services.AddOptions<ClientServiceOptions>().Bind(hostContext.Configuration.GetSection(nameof(ClientServiceOptions)));

            //TODO: can we do this in a better way?
            var appOptions = hostContext.Configuration.GetSection(nameof(AppOptions)).Get<AppOptions>();

            // https://stackoverflow.com/questions/52036998/how-do-i-get-a-reference-to-an-ihostedservice-via-dependency-injection-in-asp-ne
            if (appOptions?.ClientEnabled ?? true)
            {
                services.AddSingleton<ClientService>();
                services.AddHostedService(provider => provider.GetRequiredService<ClientService>());
            }
                
            if (appOptions?.ServerEnabled ?? true)
            {
                services.AddSingleton<ServerService>();
                services.AddHostedService(provider => provider.GetRequiredService<ServerService>());
            }


            services.AddLogging();
        }).UseConsoleLifetime();
}


