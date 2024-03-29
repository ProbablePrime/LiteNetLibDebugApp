
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

            // Manually overwrites the log path, quite complex? TODO?
            var tmpConfig = config.Build();
            var fileOpts = tmpConfig.GetSection("Logging").GetSection("File").Get<FileLoggerConfig>();
            var filePath = GetLogPath(fileOpts?.Path);

            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Logging:File:Path"] = filePath
            });
        })
        .ConfigureServices((hostContext, services) =>
        {
            //TODO: can this be automatic, its a little gross.
            services.AddOptions<ServerServiceOptions>().Bind(hostContext.Configuration.GetSection(nameof(ServerServiceOptions)));
            services.AddOptions<ClientServiceOptions>().Bind(hostContext.Configuration.GetSection(nameof(ClientServiceOptions)));

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();

                var loggingSection = hostContext.Configuration.GetSection("Logging");
                var loggingOpts = loggingSection.Get<FileLoggerConfig>();
                loggingBuilder.AddFile(loggingSection, opts =>
                {
                    // Creates an alternative file, if we cannot get a log lock, shouldn't happen
                    opts.HandleFileError = (err) => err.UseNewLogFileName(Path.GetFileNameWithoutExtension(err.LogFileName) + "_alt" + Path.GetExtension(err.LogFileName));
                });
            });

            //TODO: can we do this in a better way, its got a lot of.. stuff, that makes it a little gross.
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
                throw new Exception("This app will not do anything without a client or server enabled!");

            services.AddSingleton<LNLNetLoggerAdapter>();
        }).UseConsoleLifetime();

    static string GetLogPath(string? originalPath)
    {
        if (originalPath == null)
            return "app.log";

        var directory = Path.TrimEndingDirectorySeparator(Path.GetDirectoryName(originalPath) ?? "");
        var file = Path.GetFileNameWithoutExtension(originalPath);

        var timeStr = string.Format("-{0:yyyy}.{0:MM}.{0:dd} {0:HH}_{0:mm}_{0:ss}", DateTime.Now);

        return Path.Join(directory, file + timeStr + ".log");
    }
}


