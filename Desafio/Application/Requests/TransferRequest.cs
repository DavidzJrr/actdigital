using System;

namespace Desafio.Application.Requests;

public sealed class TransferRequest
{
    public string Cpf { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string PixKey { get; init; } = string.Empty;
    public DateTimeOffset Date { get; init; } = DateTimeOffset.UtcNow;
    public string ActorId { get; init; } = string.Empty;
    public string ActorName { get; init; } = string.Empty;
}
