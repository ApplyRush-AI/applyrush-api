using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MessageBroker;

/// <summary>
/// Observes MassTransit bus lifecycle events and logs them with structured, readable messages
/// instead of raw exception stack traces. Handles the common case where RabbitMQ starts
/// after the application (e.g. in Docker Compose) — MassTransit retries automatically,
/// and this observer logs those attempts cleanly.
/// </summary>
internal sealed class RabbitMqBusObserver : IBusObserver
{
    private readonly ILogger<RabbitMqBusObserver> _logger;
    private readonly string _hostName;
    private readonly ushort _port;

    public RabbitMqBusObserver(ILogger<RabbitMqBusObserver> logger, string hostName, ushort port)
    {
        _logger = logger;
        _hostName = hostName;
        _port = port;
    }

    public void PostCreate(IBus bus)
    {
        _logger.LogDebug("RabbitMQ bus created");
    }

    public void CreateFaulted(Exception exception)
    {
        _logger.LogError(exception, "RabbitMQ bus creation faulted");
    }

    public Task PreStart(IBus bus)
    {
        _logger.LogInformation("RabbitMQ bus starting, connecting to {HostName}:{Port}...", _hostName, _port);
        return Task.CompletedTask;
    }

    public Task PostStart(IBus bus, Task<BusReady> busReady)
    {
        _logger.LogInformation("RabbitMQ bus started successfully");
        return Task.CompletedTask;
    }

    public Task StartFaulted(IBus bus, Exception exception)
    {
        // This fires on each failed connection attempt during startup retry — log as warning,
        // not error, since MassTransit will keep retrying until the broker is reachable.
        _logger.LogWarning(
            "RabbitMQ connection attempt failed — broker may still be starting up. Retrying... ({ExceptionMessage})",
            exception.Message);
        return Task.CompletedTask;
    }

    public Task PreStop(IBus bus)
    {
        _logger.LogInformation("RabbitMQ bus stopping...");
        return Task.CompletedTask;
    }

    public Task PostStop(IBus bus)
    {
        _logger.LogInformation("RabbitMQ bus stopped");
        return Task.CompletedTask;
    }

    public Task StopFaulted(IBus bus, Exception exception)
    {
        _logger.LogError(exception, "RabbitMQ bus stop faulted");
        return Task.CompletedTask;
    }
}
