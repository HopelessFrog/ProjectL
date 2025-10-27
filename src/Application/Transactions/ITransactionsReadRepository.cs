using Application.Dtos;
using Application.Transactions.Dtos;
using FluentResults;

namespace Application.Transactions;

public interface ITransactionsReadRepository
{
    Task<PagedResult<TransactionDto>> GetTransactionsAsync(Guid walletId, int page, int pageSize,
        CancellationToken ct = default);

    Task<Result<TransactionDto>> GetTransactionAsync(Guid transactionId, CancellationToken ct = default);
}