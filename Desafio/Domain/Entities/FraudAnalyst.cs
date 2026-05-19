using System;

namespace Desafio.Domain.Entities;

public sealed class FraudAnalyst : IActor
{
    public string Id { get; }
    public string Name { get; }
    public string Role { get; }

    public FraudAnalyst(string id, string name, string role)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(role)) throw new ArgumentException("Role is required.", nameof(role));

        Id = id.Trim();
        Name = name.Trim();
        Role = role.Trim();
    }
}
