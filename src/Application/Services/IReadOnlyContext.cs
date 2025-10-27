using Domain;

namespace Application;

public interface IReadOnlyContext
{
    IQueryable<Wallet> Wallets { get; }
    IQueryable<Transaction> Transactions { get; }

    Task<List<T>> ToListAsync<T>(IQueryable<T> query, CancellationToken ct = default);
}