using Application.Wallets;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly AppDbContext _context;

    public WalletRepository(AppDbContext db)
    {
        _context = db;
    }

    public async Task<Wallet?> GetByIdAsync(Guid id, bool withTransactions = false, CancellationToken ct = default)
    {
        var query = _context.Wallets.AsQueryable();
    
        if (withTransactions)
        {
            query = query.Include(w => w.Transactions);
        }
    
        return await query.FirstOrDefaultAsync(w => w.Id == id, ct);
    }

    public Task AddAsync(Wallet wallet)
    {
        _context.Wallets.Add(wallet);
        return Task.CompletedTask;
    }
    
    public void Delete(Wallet wallet)
    {
        _context.Wallets.Remove(wallet);
    }
}