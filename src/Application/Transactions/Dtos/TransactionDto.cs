using Domain;

namespace Application.Transactions.Dtos;

public record TransactionDto(
    Guid Id,
    DateTimeOffset Date,
    decimal Amount,
    TransactionType Type,
    string Description,
    Guid WalletId);