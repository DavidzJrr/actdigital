using Desafio.Domain.ValueObjects;

namespace Desafio.Application.Results;

public sealed class TransferResult
{
    public bool Allowed { get; }
    public string Reason { get; }
    public Money RequestedAmount { get; }
    public Money CurrentLimit { get; }
    public Money RemainingBalance { get; }
    public Money AvailableDailyLimit { get; }

    public TransferResult(bool allowed, string reason, Money requestedAmount, Money currentLimit, Money remainingBalance, Money availableDailyLimit)
    {
        Allowed = allowed;
        Reason = reason;
        RequestedAmount = requestedAmount;
        CurrentLimit = currentLimit;
        RemainingBalance = remainingBalance;
        AvailableDailyLimit = availableDailyLimit;
    }
}
