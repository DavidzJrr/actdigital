using System;

namespace Desafio.Domain.ValueObjects;

public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }

    public Money(decimal amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        }

        Amount = decimal.Round(amount, 2);
    }

    public static Money Zero => new(0m);
    public static Money Default => new(5000m);

    public static Money operator -(Money left, Money right) => new(left.Amount - right.Amount);

    public static bool operator <=(Money left, Money right) => left.Amount <= right.Amount;

    public static bool operator >=(Money left, Money right) => left.Amount >= right.Amount;

    public static bool operator <(Money left, Money right) => left.Amount < right.Amount;

    public static bool operator >(Money left, Money right) => left.Amount > right.Amount;

    public static bool operator ==(Money left, Money right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(Money left, Money right) => !(left == right);

    public bool Equals(Money? other) => other is not null && Amount == other.Amount;

    public override bool Equals(object? obj) => Equals(obj as Money);

    public override int GetHashCode() => Amount.GetHashCode();

    public override string ToString() => Amount.ToString("F2");
}
