using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;

namespace Domain;

public class Transaction : IEntity
{
    private Transaction(DateTimeOffset date, TransactionAmount amount, TransactionType type, Description description,
        Wallet wallet)
    {
        Date = date;
        Amount = amount;
        Type = type;
        Description = description;
        WalletId = wallet.Id;
        Wallet = wallet;
    }

    private Transaction()
    {
    }
    public Guid Id { get; private set; } 

    public DateTimeOffset Date { get; private set; }
    public TransactionAmount Amount { get; }
    public TransactionType Type { get; }
    public Description Description { get; private set; }

    public Guid WalletId { get; private set; }

    public Wallet Wallet { get; private set; }

    public decimal SignedAmount =>
        Type == TransactionType.Income ? Amount.Value : -Amount.Value;


    public static Result<Transaction> Create(DateTimeOffset date, decimal amount, TransactionType type,
        string description, Wallet wallet)
    {
        if (wallet is null)
            return Result.Fail(new NotFoundError("Кошелёк не указан"));

        var amountResult = TransactionAmount.Create(amount);

        if (amountResult.IsFailed)
            return amountResult.ToResult<Transaction>();

        var descriptionResult = Description.Create(description);

        if (descriptionResult.IsFailed)
            return descriptionResult.ToResult<Transaction>();

        return Result.Ok(new Transaction(date, amountResult.Value, type, descriptionResult.Value, wallet));
    }
}