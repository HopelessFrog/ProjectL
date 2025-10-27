using Application.Services;
using Application.Transactions;
using Application.Wallets;

namespace Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public IWalletRepository WalletRepository { get; }
    public IWalletReadRepository WalletReadRepository { get; }
    public ITransactionsReadRepository TransactionsReadRepository { get; }

    public UnitOfWork(AppDbContext dbContext, IWalletRepository walletRepository,
        IWalletReadRepository walletReadRepository, ITransactionsReadRepository transactionsReadRepository)
    {
        _dbContext = dbContext;
        WalletRepository = walletRepository;
        WalletReadRepository = walletReadRepository;
        TransactionsReadRepository = transactionsReadRepository;
    }


    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}