using Desafio.Domain.ValueObjects;
using System;

namespace Desafio.Domain.Entities;

public sealed class Account
{
    public Guid AccountId { get; }
    public Cpf Cpf { get; }
    public AgencyNumber Agency { get; }
    public AccountNumber Number { get; }
    public Money PixLimit { get; private set; }
    public LimitAudit? LimitAudit { get; private set; }
    public Money Balance { get; private set; }
    public Money DailyLimitConsumed { get; private set; }
    public DateOnly DailyLimitConsumedAt { get; private set; }

    public Account(Cpf cpf, AgencyNumber agency, AccountNumber number, Money pixLimit, LimitAudit? limitAudit = null, Guid? accountId = null, Money? balance = null, Money? dailyLimitConsumed = null, DateOnly? dailyLimitConsumedAt = null)
    {
        AccountId = accountId ?? Guid.NewGuid();
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        Agency = agency ?? throw new ArgumentNullException(nameof(agency));
        Number = number ?? throw new ArgumentNullException(nameof(number));
        PixLimit = pixLimit ?? throw new ArgumentNullException(nameof(pixLimit));
        LimitAudit = limitAudit;
        Balance = balance ?? Money.Zero;
        DailyLimitConsumed = dailyLimitConsumed ?? Money.Zero;
        DailyLimitConsumedAt = dailyLimitConsumedAt ?? DateOnly.MinValue;
    }

    public bool CanExecutePix(Money amount)
    {
        ArgumentNullException.ThrowIfNull(amount);

        return amount <= PixLimit;
    }

    public void UpdatePixLimit(Money newLimit, FraudAnalyst actor)
    {
        ArgumentNullException.ThrowIfNull(newLimit);

        PixLimit = newLimit;
        LimitAudit = new LimitAudit(actor, DateTimeOffset.UtcNow);
    }

    public Money GetAvailableDailyLimit(DateTimeOffset requestedAt)
    {
        var currentDay = DateOnly.FromDateTime(requestedAt.UtcDateTime);
        if (currentDay != DailyLimitConsumedAt)
        {
            return PixLimit;
        }

        var remainingAmount = PixLimit.Amount - DailyLimitConsumed.Amount;
        return remainingAmount <= 0 ? Money.Zero : new Money(remainingAmount);
    }

    public bool CanExecuteTransfer(Money amount, DateTimeOffset requestedAt)
    {
        if (amount is null) throw new ArgumentNullException(nameof(amount));
        return amount.Amount <= Balance.Amount && amount <= GetAvailableDailyLimit(requestedAt);
    }

    public void Deposit(Money amount)
    {
        if (amount is null) throw new ArgumentNullException(nameof(amount));
        Balance = new Money(Balance.Amount + amount.Amount);
    }

    public void Withdraw(Money amount)
    {
        if (amount is null) throw new ArgumentNullException(nameof(amount));
        if (amount.Amount > Balance.Amount) throw new InvalidOperationException("Insufficient balance.");
        Balance = new Money(Balance.Amount - amount.Amount);
    }

    public void ConsumeDailyLimit(Money amount, IActor actor, DateTimeOffset requestedAt)
    {
        if (amount is null) throw new ArgumentNullException(nameof(amount));
        if (actor is null) throw new ArgumentNullException(nameof(actor));

        var currentDay = DateOnly.FromDateTime(requestedAt.UtcDateTime);
        if (currentDay != DailyLimitConsumedAt)
        {
            DailyLimitConsumed = Money.Zero;
            DailyLimitConsumedAt = currentDay;
        }

        if (!CanExecuteTransfer(amount, requestedAt))
        {
            throw new InvalidOperationException("Insufficient balance or amount exceeds available PIX limit for today.");
        }

        Balance = new Money(Balance.Amount - amount.Amount);
        DailyLimitConsumed = new Money(DailyLimitConsumed.Amount + amount.Amount);
        LimitAudit = new LimitAudit(actor, requestedAt);
    }
}
