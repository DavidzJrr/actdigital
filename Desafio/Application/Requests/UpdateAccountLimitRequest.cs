namespace Desafio.Application.Requests;

public sealed class UpdateAccountLimitRequest
{
    public string Cpf { get; init; } = string.Empty;
    public string Agency { get; init; } = string.Empty;
    public string Account { get; init; } = string.Empty;
    public decimal PixLimit { get; init; }
    public string ActorId { get; init; } = string.Empty;
    public string ActorName { get; init; } = string.Empty;
    public string ActorRole { get; init; } = string.Empty;
}
