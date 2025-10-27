using Domain.Errors;
using FluentResults;

namespace Domain.ValueObjects;

public sealed record MoneyAmount
{
    private MoneyAmount(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public static Result<MoneyAmount> Create(decimal value)
    {
        if (value < 0)
            return Result.Fail(new ValidationError("Сумма не может быть отрицательной"));

        return Result.Ok(new MoneyAmount(value));
    }

    public override string ToString()
    {
        return Value.ToString("C");
    }
}