using System;
using System.Linq;

namespace Desafio.Domain.ValueObjects;

public sealed class AgencyNumber
{
    public string Value { get; }

    public AgencyNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Agency number is required.", nameof(value));
        }

        var digits = value.Trim();
        if (!digits.All(char.IsDigit))
        {
            throw new ArgumentException("Agency number must contain only digits.", nameof(value));
        }

        if (digits.Length != 4)
        {
            throw new ArgumentException("Agency number must contain exactly 4 digits.", nameof(value));
        }

        Value = digits;
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) => obj is AgencyNumber other && string.Equals(Value, other.Value, StringComparison.Ordinal);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);
}
