using Desafio.Domain.ValueObjects;

namespace Desafio.Domain.Events;

public sealed class DailyLimitConsumedEvent : IDomainEvent
{
    public DailyLimitConsumedEvent(Guid accountId, string cpf, Money transferAmount, Money balanceAfter, Money availableDailyLimit, DateTimeOffset occurredAt)
    {
        AccountId = accountId;
        Cpf = cpf;
        TransferAmount = transferAmount;
        BalanceAfter = balanceAfter;
        AvailableDailyLimit = availableDailyLimit;
        OccurredAt = occurredAt;
    }

    public Guid AccountId { get; }
    public string Cpf { get; }
    public Money TransferAmount { get; }
    public Money BalanceAfter { get; }
    public Money AvailableDailyLimit { get; }
    public DateTimeOffset OccurredAt { get; }
}
