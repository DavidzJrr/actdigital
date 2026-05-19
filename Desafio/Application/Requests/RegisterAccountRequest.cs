namespace Desafio.Application.Requests;

public sealed class RegisterAccountRequest
{
    public string Cpf { get; init; } = string.Empty;
    public string Agency { get; init; } = string.Empty;
    public string Account { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;

}
