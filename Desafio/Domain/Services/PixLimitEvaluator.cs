using System;
using Desafio.Domain.Entities;
using Desafio.Domain.ValueObjects;

namespace Desafio.Domain.Services;

public sealed class PixLimitEvaluator
{
    public PixLimitEvaluationResult Evaluate(Account account, Money requestedAmount, DateTimeOffset requestedAt)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(requestedAmount);

        var availableDailyLimit = account.GetAvailableDailyLimit(requestedAt);
        var canExecute = account.CanExecuteTransfer(requestedAmount, requestedAt);

        if (!canExecute)
        {
            var reason = requestedAmount.Amount > account.Balance.Amount
                ? "Transação negada: saldo insuficiente."
                : "Transação negada: limite PIX diário excedido.";

            return new PixLimitEvaluationResult(
                Allowed: false,
                Reason: reason,
                RequestedAmount: requestedAmount,
                CurrentLimit: account.PixLimit,
                RemainingLimit: Money.Zero,
                AvailableDailyLimit: availableDailyLimit);
        }

        return new PixLimitEvaluationResult(
            Allowed: true,
            Reason: "Transação autorizada dentro do limite PIX.",
            RequestedAmount: requestedAmount,
            CurrentLimit: account.PixLimit,
            RemainingLimit: account.PixLimit - requestedAmount,
            AvailableDailyLimit: availableDailyLimit - requestedAmount);
    }
}

public sealed record PixLimitEvaluationResult(
    bool Allowed,
    string Reason,
    Money RequestedAmount,
    Money CurrentLimit,
    Money RemainingLimit,
    Money AvailableDailyLimit);
