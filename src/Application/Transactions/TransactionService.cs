using Application.Dtos;
using Application.Transactions.Dtos;
using FluentResults;

namespace Application.Transactions;

public class TransactionService : ITransactionService
{
    private readonly ITransactionsReadRepository _readRepository;

    public TransactionService(ITransactionsReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<PagedResult<TransactionDto>> GetTransactionsByWalletId(Guid walletId, int page, int pageSize,
        CancellationToken ct = default)
    {
        return await _readRepository.GetTransactionsAsync(walletId, page, pageSize, ct);
    }

    public async Task<Result<TransactionDto>> GetTransactionAsync(Guid transactionId, CancellationToken ct = default)
    {
        return await _readRepository.GetTransactionAsync(transactionId, ct);
    }
}
