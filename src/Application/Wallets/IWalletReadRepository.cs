using Application.Dtos;
using Application.Wallets.Dtos;
using FluentResults;

namespace Application.Wallets;

public interface IWalletReadRepository
{
    Task<Result<WalletReadDto>> GetWalletAsync(Guid walletId, CancellationToken ct = default);
    Task<PagedResult<WalletPreviewDto>> GetWalletsAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Result<decimal>> GetBalanceAsync(Guid walletId, CancellationToken ct = default);
}