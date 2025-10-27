using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ReadOnlyContext : IReadOnlyContext
{
    private readonly AppDbContext _context;

    public ReadOnlyContext(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Wallet> Wallets => _context.Wallets.AsNoTracking();
    public IQueryable<Transaction> Transactions => _context.Transactions.AsNoTracking();

    public async Task<List<T>> ToListAsync<T>(IQueryable<T> query, CancellationToken ct = default)
    {
        return await query.ToListAsync(ct);
    }
}