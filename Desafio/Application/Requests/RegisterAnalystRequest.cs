namespace Desafio.Application.Requests;

public sealed class RegisterAnalystRequest
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Name { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
