using Domain.Errors;
using FluentResults;

namespace Domain.ValueObjects;

public sealed record TransactionAmount
{
    private TransactionAmount(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public static Result<TransactionAmount> Create(decimal value)
    {
        if (value <= 0)
            return Result.Fail(new ValidationError("Сумма транзакции должна быть больше 0 "));

        return Result.Ok(new TransactionAmount(value));
    }

    public override string ToString()
    {
        return Value.ToString("C");
    }
}