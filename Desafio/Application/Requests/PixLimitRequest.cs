namespace Desafio.Application.Requests;

public sealed class PixLimitRequest
{
    public string Cpf { get; init; } = string.Empty;
    public string Agency { get; init; } = string.Empty;
    public string Account { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
