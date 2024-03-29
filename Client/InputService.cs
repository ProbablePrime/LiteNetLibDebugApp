using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LiteNetLibDebugApp;

public class InputService : BackgroundService
{
    private readonly ILogger Log;
    private readonly IHostApplicationLifetime HostLifetime;
    private readonly ClientService Client;

    public InputService(ILoggerFactory logF, IHostApplicationLifetime hostLifetime, ClientService client)
    {
        Client = client;
        Log = logF.CreateLogger(nameof(InputService));
        HostLifetime = hostLifetime;

        Log.LogInformation("Starting up Input Service");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(InputLoop, stoppingToken);
        return Task.CompletedTask;
    }

    private async Task InputLoop()
    {
        while (!HostLifetime.ApplicationStopping.IsCancellationRequested)
        {
            var line = await Console.In.ReadLineAsync(HostLifetime.ApplicationStopping).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(line))
                Client.Send(line);
        }
    }
}
