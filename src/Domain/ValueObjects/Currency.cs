using System.Collections.Immutable;
using Domain.Errors;
using FluentResults;

namespace Domain.ValueObjects;

public sealed record Currency
{
    private static readonly IImmutableSet<string> AllowedCurrencies = ImmutableHashSet.Create(
        StringComparer.OrdinalIgnoreCase,
        "USD", "EUR", "GBP", "RUB", "CNY", "JPY"
    );

    private Currency(string value)
    {
        Value = value.ToUpperInvariant();
    }

    public string Value { get; }

    public static Result<Currency> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail(new ValidationError("Код валюты не может быть пустым."));

        if (!AllowedCurrencies.Contains(value))
            return Result.Fail(new ValidationError(
                $"Валюта '{value}' не поддерживается. Разрешены: {string.Join(", ", AllowedCurrencies)}."));

        return Result.Ok(new Currency(value));
    }

    public override string ToString()
    {
        return Value;
    }
}