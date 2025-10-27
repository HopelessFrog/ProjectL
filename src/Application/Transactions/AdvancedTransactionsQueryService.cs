using Application.Transactions.Dtos;

namespace Application.Transactions;

public class AdvancedTransactionsQueryService : IAdvancedTransactionsQueryService
{
    private readonly IReadOnlyContext _context;

    public AdvancedTransactionsQueryService(IReadOnlyContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<TransactionGroupByTypeDto>> GetTransactionsGroupedByTypeAsync(
        int year, int month, CancellationToken cancellationToken = default)
    {
        var grouped = _context.Transactions
            .Where(t => t.Date.Year == year && t.Date.Month == month)
            .GroupBy(t => t.Type)
            .Select(g => new TransactionGroupByTypeDto
            {
                Type = g.Key,
                TotalAmount = g.Sum(t => t.Amount.Value),
                Transactions = g.Select(t => new TransactionDto(
                    t.Id,
                    t.Date,
                    t.Amount.Value,
                    t.Type,
                    t.Description.Value,
                    t.WalletId
                )).ToList()
            });

        return await _context.ToListAsync(grouped, cancellationToken);
    }
}