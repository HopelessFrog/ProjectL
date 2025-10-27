using Application.Dtos;
using Application.Transactions.Dtos;
using Application.Wallets.Dtos;
using Domain;
using FluentResults;

namespace Application.Wallets;

public interface IWalletService
{
    Task<Result<Wallet>> CreateWalletAsync(string name, string currency, decimal initialBalance);
    Task<Result<WalletReadDto>> GetWalletByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<WalletPreviewDto>> GetWalletsAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Result> RenameWalletAsync(Guid walletId, string newName);
    Task<Result> DeleteWalletAsync(Guid walletId);
    Task<Result<Transaction>> AddTransaction(CreateTransactionDto dto);
}