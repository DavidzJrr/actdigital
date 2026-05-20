using Desafio.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Desafio.Application.Events;

public sealed class LoggingDomainEventHandler<TEvent> : IDomainEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    private readonly ILogger<LoggingDomainEventHandler<TEvent>> _logger;

    public LoggingDomainEventHandler(ILogger<LoggingDomainEventHandler<TEvent>> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Domain event published: {EventType} at {OccurredAt}", typeof(TEvent).Name, DateTimeOffset.UtcNow);
        return Task.CompletedTask;
    }
}
