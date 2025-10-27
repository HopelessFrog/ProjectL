namespace Application.Transactions.Dtos;

public record ExpenseTransactionDto(Guid Id, DateTimeOffset Date, decimal Amount, string Description, Guid WalletId);