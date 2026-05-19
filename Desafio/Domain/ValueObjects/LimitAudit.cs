using System;
using Desafio.Domain.Entities;

namespace Desafio.Domain.ValueObjects;

public sealed class LimitAudit
{
    public IActor Actor { get; }
    public DateTimeOffset ChangedAt { get; }

    public LimitAudit(IActor actor, DateTimeOffset changedAt)
    {
        Actor = actor ?? throw new ArgumentNullException(nameof(actor));
        ChangedAt = changedAt;
    }
}
