using System.Data;
using Application.Services;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Data.Executors;

public class TransactionalExecutor : ITransactionalExecutor
{
    private readonly AppDbContext _context;

    public TransactionalExecutor(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TResult>> ExecuteAsync<TResult>(
        Func<Task<Result<TResult>>> action,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        try
        {
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(isolationLevel);

                var result = await action();

                if (result.IsFailed)
                {
                    await transaction.RollbackAsync();
                    return result;
                }

                await transaction.CommitAsync();
                return result;
            });
        }
        catch (Exception ex)
        {
            return Result.Fail<TResult>("Попробуйте позже");
        }
    }
}