using Desafio.Domain.Services;

namespace Desafio.Application.Results;

public sealed class PixLimitResult
{
    public bool Allowed { get; }
    public string Reason { get; }
    public decimal RequestedAmount { get; }
    public decimal CurrentLimit { get; }
    public decimal RemainingLimit { get; }
    public decimal AvailableDailyLimit { get; }

    public PixLimitResult(PixLimitEvaluationResult evaluationResult)
    {
        Allowed = evaluationResult.Allowed;
        Reason = evaluationResult.Reason;
        RequestedAmount = evaluationResult.RequestedAmount.Amount;
        CurrentLimit = evaluationResult.CurrentLimit.Amount;
        RemainingLimit = evaluationResult.RemainingLimit.Amount;
        AvailableDailyLimit = evaluationResult.AvailableDailyLimit.Amount;
    }
}
