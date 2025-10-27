using Application.Dtos;
using Application.Transactions;
using Application.Transactions.Dtos;
using Data.Extensions;
using Domain.Errors;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class TransactionsReadRepository : ITransactionsReadRepository
{
    private readonly AppDbContext _context;

    public TransactionsReadRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<TransactionDto>> GetTransactionsAsync(Guid walletId, int page, int pageSize,
        CancellationToken ct = default)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Where(t => t.WalletId == walletId)
            .Select(transaction => new TransactionDto(
                transaction.Id,
                transaction.Date,
                transaction.Amount.Value,
                transaction.Type,
                transaction.Description.Value,
                transaction.WalletId
            )).ToPagedResultAsync(page, pageSize, ct);
    }

    public async Task<Result<TransactionDto>> GetTransactionAsync(Guid transactionId, CancellationToken ct = default)
    {
      var transaction =  await _context.Transactions.AsNoTracking()
            .Where(transaction => transaction.Id == transactionId)
            .Select(transaction => new TransactionDto(
                transaction.Id,
                transaction.Date,
                transaction.Amount.Value,
                transaction.Type,
                transaction.Description.Value,
                transaction.WalletId))
            .FirstOrDefaultAsync(ct);
      
      if (transaction is null)
              return Result.Fail<TransactionDto>(new NotFoundError("Транзакция не найдена"));

      return transaction;
    }
}