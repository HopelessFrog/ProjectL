using Application.Transactions;
using Application.Wallets;

namespace Application.Services;

public interface IUnitOfWork
{
    IWalletRepository  WalletRepository { get; }
    Task SaveChangesAsync(CancellationToken ct = default);
}