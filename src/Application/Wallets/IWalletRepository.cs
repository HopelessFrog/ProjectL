using Domain;

namespace Application.Wallets;

public interface IWalletRepository
{
    
    Task<Wallet?> GetByIdAsync(Guid id, bool withTransactions = false, CancellationToken ct = default);
    Task AddAsync(Wallet wallet);
    void Delete(Wallet wallet);
}