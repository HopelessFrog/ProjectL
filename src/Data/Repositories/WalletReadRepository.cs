using Application.Dtos;
using Application.Wallets;
using Application.Wallets.Dtos;
using Data.Extensions;
using Domain.Errors;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class WalletReadRepository : IWalletReadRepository
{
    private readonly AppDbContext _context;

    public WalletReadRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<WalletReadDto>> GetWalletAsync(Guid walletId, CancellationToken ct = default)
    {
        var walletData = await _context.Wallets
            .AsNoTracking()
            .Where(w => w.Id == walletId)
            .Select(w => new 
            {
                w.Id,
                Name = w.Name.Value,
                Currency = w.Currency.Value,
                InitialBalance = w.InitialBalance.Value
            })
            .FirstOrDefaultAsync(ct);

        if (walletData is null)
        {
            return Result.Fail<WalletReadDto>(new NotFoundError("Кошелёк не найден"));
        }

        var transactionsSum = await _context.Transactions
            .Where(t => t.WalletId == walletId)
            .SelectSignedAmount()
            .SumAsync(ct);

        var currentBalance = walletData.InitialBalance + transactionsSum;
    
        var walletDto = new WalletReadDto(
            walletData.Id,
            walletData.Name,
            walletData.Currency,
            walletData.InitialBalance,
            currentBalance);

        return Result.Ok(walletDto);
    }

    public async Task<PagedResult<WalletPreviewDto>> GetWalletsAsync(int page, int pageSize,
        CancellationToken ct = default)
    {
        return await _context.Wallets
            .AsNoTracking()
            .Select(wallet => new WalletPreviewDto(
                wallet.Id,
                wallet.Name.Value,
                wallet.Currency.Value
            )).ToPagedResultAsync(page, pageSize, ct);
    }

    public async Task<Result<decimal>> GetBalanceAsync(Guid walletId, CancellationToken ct = default)
    {
        var initialBalance = await _context.Wallets
            .AsNoTracking()
            .Where(w => w.Id == walletId)
            .Select(w => (decimal?)w.InitialBalance.Value)
            .FirstOrDefaultAsync(ct);

        if (initialBalance is null)
        {
            return Result.Fail<decimal>("Кошелёк не найден");
        }

        var transactionsSum = await _context.Transactions
            .Where(t => t.WalletId == walletId)
            .SelectSignedAmount()
            .SumAsync(ct);

        return Result.Ok(initialBalance.Value + transactionsSum);
    }
}