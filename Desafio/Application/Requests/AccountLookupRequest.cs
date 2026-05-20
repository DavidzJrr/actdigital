namespace Desafio.Application.Requests;

public sealed class AccountLookupRequest
{
    public string Cpf { get; init; } = string.Empty;
}
