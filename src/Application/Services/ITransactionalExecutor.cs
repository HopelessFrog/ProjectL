using System.Data;
using FluentResults;

namespace Application.Services;

public interface ITransactionalExecutor
{
    Task<Result<TResult>> ExecuteAsync<TResult>(
        Func<Task<Result<TResult>>> action,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
}