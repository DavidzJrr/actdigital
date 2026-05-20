using Desafio.Domain.ValueObjects;

namespace Desafio.Domain.Events;

public sealed class PixLimitCheckedEvent : IDomainEvent
{
    public PixLimitCheckedEvent(Guid accountId, string cpf, Money pixLimit, Money availableDailyLimit, DateTimeOffset occurredAt)
    {
        AccountId = accountId;
        Cpf = cpf;
        PixLimit = pixLimit;
        AvailableDailyLimit = availableDailyLimit;
        OccurredAt = occurredAt;
    }

    public Guid AccountId { get; }
    public string Cpf { get; }
    public Money PixLimit { get; }
    public Money AvailableDailyLimit { get; }
    public DateTimeOffset OccurredAt { get; }
}
