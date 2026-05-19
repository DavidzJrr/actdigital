using System;
using System.Linq;

namespace Desafio.Domain.ValueObjects;

public sealed class Cpf
{
    public string Value { get; }

    public Cpf(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("CPF is required.", nameof(value));
        }

        var digits = new string(value.Where(char.IsDigit).ToArray());
        if (digits.Length != 11)
        {
            throw new ArgumentException("CPF must contain 11 digits.", nameof(value));
        }

        Value = digits;
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) => obj is Cpf other && string.Equals(Value, other.Value, StringComparison.Ordinal);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);
}
