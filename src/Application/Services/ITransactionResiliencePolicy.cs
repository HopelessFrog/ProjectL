namespace Application.Services;

public interface ITransactionResiliencePolicy
{
    Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action);
}