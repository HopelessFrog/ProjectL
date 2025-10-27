using Application.Dtos;
using Application.Transactions.Dtos;
using FluentResults;

namespace Application.Transactions;

public interface ITransactionService
{
    Task<PagedResult<TransactionDto>> GetTransactionsByWalletId(Guid walletId, int page, int pageSize,
        CancellationToken ct = default);

    Task<Result<TransactionDto>> GetTransactionAsync(Guid transactionId,
        CancellationToken ct = default);
}