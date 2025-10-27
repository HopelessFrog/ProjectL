using Application.Transactions.Dtos;
using Application.Wallets.Dtos;
using Domain;

namespace Application.Wallets;

public class AdvancedWalletsQueryService : IAdvancedWalletsQueryService
{
    private readonly IReadOnlyContext _context;

    public AdvancedWalletsQueryService(IReadOnlyContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<TopExpensesByWalletDto>> GetAllTopExpensesByWalletAsync(int year, int month,
        int count,
        CancellationToken ct = default)
    {
        var topExpenses = _context.Transactions
            .Where(t => t.Type == TransactionType.Expense &&
                        t.Date.Year == year &&
                        t.Date.Month == month)
            .GroupBy(t => t.Wallet)
            .Select(g => new TopExpensesByWalletDto
            {
                WalletId = g.Key.Id,
                WalletName = g.Key.Name.Value,
                TopExpenses = g
                    .OrderByDescending(t => t.Amount.Value)
                    .Take(count)
                    .Select(t => new ExpenseTransactionDto(
                        t.Id,
                        t.Date,
                        t.Amount.Value,
                        t.Description.Value,
                        t.WalletId))
                    .ToList()
            });

        return await _context.ToListAsync(topExpenses, ct);
    }
}