
using LiteNetLibDebugApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NReco.Logging.File;

public class Program
{
    static CancellationTokenSource stopRunning = new CancellationTokenSource();
    static bool ShouldRun => !stopRunning.IsCancellationRequested;
    public async static Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        // Force LNLNetLoggerAdapterToBeCreated // TODO: Is this the best path?
        var lnlLogger = host.Services.GetService<LNLNetLoggerAdapter>();

        await host.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            var env = hostingContext.HostingEnvironment;

            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            config.AddEnvironmentVariables();

            config.AddCommandLine(args, Constants.SWITCH_MAPPINGS);
        })
        .ConfigureServices((hostContext, services) =>
        {
            
            //TODO: can this be automatic?
            services.AddOptions<ServerServiceOptions>().Bind(hostContext.Configuration.GetSection(nameof(ServerServiceOptions)));
            services.AddOptions<ClientServiceOptions>().Bind(hostContext.Configuration.GetSection(nameof(ClientServiceOptions)));

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();

                var loggingSection = hostContext.Configuration.GetSection("Logging");
                loggingBuilder.AddFile(loggingSection);
            });

            //TODO: can we do this in a better way, its got a lot of.. stuff
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

            if (appOptions?.ClientEnabled != true && appOptions?.ServerEnabled != true)
                throw new Exception("This app will not do anything without client or server enabled");

            services.AddSingleton<LNLNetLoggerAdapter>();
        }).UseConsoleLifetime();
}


