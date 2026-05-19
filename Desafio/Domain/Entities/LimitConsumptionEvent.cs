namespace Desafio.Domain.Entities;

public sealed class LimitConsumptionEvent : IActor
{
    public string Id => "system-limit-consumption";
    public string Name => "Consumo do Limite";
    public string Role => "SystemEvent";
}
