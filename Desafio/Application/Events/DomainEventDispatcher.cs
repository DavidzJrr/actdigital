using Desafio.Domain.Events;

namespace Desafio.Application.Events;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var enumerableType = typeof(IEnumerable<>).MakeGenericType(handlerType);
        var handlers = (IEnumerable<object>?)_serviceProvider.GetService(enumerableType);

        if (handlers is null)
        {
            return;
        }

        foreach (dynamic handler in handlers)
        {
            await handler.HandleAsync((dynamic)domainEvent, cancellationToken);
        }
    }
}
