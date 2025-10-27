using Domain.Errors;
using FluentResults;

namespace Domain.ValueObjects;

public sealed record Description
{
    private const int MaxLength = 200;

    private Description(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Description> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Ok(new Description(string.Empty));

        var trimValue = value.Trim();

        if (trimValue.Length > MaxLength)
            return Result.Fail(new ValidationError($"Превышен максимальный размер описания({MaxLength} символов)"));

        return Result.Ok(new Description(trimValue));
    }

    public override string ToString()
    {
        return Value;
    }
}