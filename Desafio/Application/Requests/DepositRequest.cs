namespace Desafio.Application.Requests;

public sealed class DepositRequest
{
    public string Cpf { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string ActorId { get; init; } = string.Empty;
    public string ActorName { get; init; } = string.Empty;
    public string ActorRole { get; init; } = string.Empty;
}
