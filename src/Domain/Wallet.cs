using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;

namespace Domain;

public class Wallet : IEntity
{
    private readonly List<Transaction> _transactions = new();
    public Guid Id { get; private set; } 
    public Currency Currency { get; private set; }
    public Name Name { get; private set; }
    public MoneyAmount InitialBalance { get; private set; }
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();
    
    private Wallet(Name name, Currency currency, MoneyAmount initialBalance)
    {
        Name = name;
        Currency = currency;
        InitialBalance = initialBalance;
    }

    private Wallet()
    {
    }
    
    public Result<Transaction> AddTransaction(DateTimeOffset date, decimal amount, TransactionType type,
        string description, decimal currentBalance)
    {
        var newBalance = currentBalance + (type == TransactionType.Income ? amount : -amount);

        if (newBalance < 0)
            return Result.Fail(new ConflictError("Недостаточно средств. Баланс не может стать отрицательным"));

        var transactionResult = Transaction.Create(date, amount, type, description, this);

        if (transactionResult.IsFailed)
            return transactionResult;

        var transaction = transactionResult.Value;

        _transactions.Add(transaction);

        return Result.Ok(transaction);
    }

    public Result Rename(string newName)
    {
        var nameResult = Name.Create(newName);
        if (nameResult.IsFailed) return nameResult.ToResult();

        Name = nameResult.Value;
        return Result.Ok();
    }

    public static Result<Wallet> Create(string currency, decimal startBalance, string name)
    {
        var currencyResult = Currency.Create(currency);
        if (currencyResult.IsFailed)
            return currencyResult.ToResult<Wallet>();

        var initialBalanceResult = MoneyAmount.Create(startBalance);
        if (initialBalanceResult.IsFailed)
            return initialBalanceResult.ToResult<Wallet>();

        var nameResult = Name.Create(name);
        if (nameResult.IsFailed)
            return nameResult.ToResult<Wallet>();

        return Result.Ok(new Wallet(nameResult.Value, currencyResult.Value, initialBalanceResult.Value));
    }
}