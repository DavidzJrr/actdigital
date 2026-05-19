using System;
using System.Linq;

namespace Desafio.Domain.ValueObjects;

public sealed class AccountNumber
{
    public string Value { get; }

    public AccountNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Account number is required.", nameof(value));
        }

        var digits = value.Trim();
        if (!digits.All(char.IsDigit))
        {
            throw new ArgumentException("Account number must contain only digits.", nameof(value));
        }

        if (digits.Length != 7)
        {
            throw new ArgumentException("Account number must contain exactly 7 digits.", nameof(value));
        }

        Value = digits;
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) => obj is AccountNumber other && string.Equals(Value, other.Value, StringComparison.Ordinal);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);
}
