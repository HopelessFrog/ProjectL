using System.Data;
using Application.Dtos;
using Application.Services;
using Application.Transactions.Dtos;
using Application.Wallets.Dtos;
using Domain;
using Domain.Errors;
using FluentResults;
using Transaction = Domain.Transaction;

namespace Application.Wallets;

public class WalletService : IWalletService
{
    private readonly TimeProvider _timeProvider;
    private readonly ITransactionalExecutor _transactionExecutor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWalletReadRepository _walletReadtRepository;

    public WalletService(
        IUnitOfWork unitOfWork,
        IWalletReadRepository walletReadtRepository,
        TimeProvider timeProvider,
        ITransactionalExecutor transactionExecutor)
    {
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _transactionExecutor = transactionExecutor;
        _walletReadtRepository =  walletReadtRepository;
    }

    public async Task<Result<Wallet>> CreateWalletAsync(string name, string currency, decimal initialBalance)
    {
        var walletResult = Wallet.Create(currency, initialBalance, name);

        if (walletResult.IsFailed)
            return walletResult;

        var wallet = walletResult.Value;

        await _unitOfWork.WalletRepository.AddAsync(wallet);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(wallet);
    }

    public async Task<Result<WalletReadDto>> GetWalletByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _walletReadtRepository.GetWalletAsync(id, ct);
    }


    public async Task<PagedResult<WalletPreviewDto>> GetWalletsAsync(int page, int pageSize,
        CancellationToken ct = default)
    {
        return await _walletReadtRepository.GetWalletsAsync(page, pageSize, ct);
    }


    public async Task<Result> RenameWalletAsync(Guid walletId, string newName)
    {
        var wallet = await  _unitOfWork.WalletRepository.GetByIdAsync(walletId);

        if (wallet is null)
            return Result.Fail(new NotFoundError("Кошелёк не найден"));

        var renameResult = wallet.Rename(newName);

        if (renameResult.IsFailed)
            return renameResult;

        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> DeleteWalletAsync(Guid walletId)
    {
        var wallet = await  _unitOfWork.WalletRepository.GetByIdAsync(walletId);

        if (wallet is null)
            return Result.Fail(new NotFoundError("Кошелёк не найден"));

        _unitOfWork.WalletRepository.Delete(wallet);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result<Transaction>> AddTransaction(CreateTransactionDto dto)
    {
        return await _transactionExecutor.ExecuteAsync(async () =>
        {
            var wallet = await  _unitOfWork.WalletRepository.GetByIdAsync(dto.WalletId);
            
            if (wallet is null)
                return Result.Fail<Transaction>("Кошелёк не найден");

            var currentBalanceResult = await _walletReadtRepository.GetBalanceAsync(wallet.Id);
            if (currentBalanceResult.IsFailed)
                return currentBalanceResult.ToResult<Transaction>();

            var result = wallet.AddTransaction(
                _timeProvider.GetUtcNow(),
                dto.Amount,
                dto.Type,
                dto.Description,
                currentBalanceResult.Value);

            if (result.IsFailed)
                return result;
            
            await _unitOfWork.SaveChangesAsync();
            
            return result;
        }, IsolationLevel.Serializable);
    }
}