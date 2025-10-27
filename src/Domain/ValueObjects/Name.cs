using System.Text.RegularExpressions;
using Domain.Errors;
using FluentResults;

namespace Domain.ValueObjects;

public sealed record Name
{
    private const int MaxLength = 30;
    private static readonly Regex ValidNameRegex = new(@"^[a-zA-Zа-яА-ЯёЁ0-9\s\-]+$", RegexOptions.Compiled);

    private Name(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Name> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail(new ValidationError("Имя не может быть пустым"));

        var trimValue = value.Trim();

        if (trimValue.Length > MaxLength)
            return Result.Fail(new ValidationError($"Превышен максимальный размер имени({MaxLength} символов)"));

        if (!ValidNameRegex.IsMatch(trimValue))
            return Result.Fail(
                new ValidationError(
                    "Имя содержит недопустимые символы (разрешены только буквы, цифры, пробелы и дефисы)"));

        return Result.Ok(new Name(trimValue));
    }

    public override string ToString()
    {
        return Value;
    }
}