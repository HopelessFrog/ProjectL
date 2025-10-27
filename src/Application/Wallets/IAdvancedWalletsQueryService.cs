using Application.Wallets.Dtos;

namespace Application.Wallets;

public interface IAdvancedWalletsQueryService
{
    Task<IReadOnlyCollection<TopExpensesByWalletDto>> GetAllTopExpensesByWalletAsync(
        int year, int month, int count, CancellationToken cancellationToken = default);
}