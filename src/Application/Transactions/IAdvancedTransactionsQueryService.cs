using Application.Transactions.Dtos;

namespace Application.Transactions;

public interface IAdvancedTransactionsQueryService
{
    Task<IReadOnlyCollection<TransactionGroupByTypeDto>> GetTransactionsGroupedByTypeAsync(
        int year, int month, CancellationToken cancellationToken = default);
}