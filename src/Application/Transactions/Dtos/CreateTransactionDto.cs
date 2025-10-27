using Domain;

namespace Application.Transactions.Dtos;

public sealed record CreateTransactionDto(
    decimal Amount,
    TransactionType Type,
    string Description,
    Guid WalletId);