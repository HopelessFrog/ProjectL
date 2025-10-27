using Application.Transactions;
using Application.Wallets;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services.AddScoped<IWalletService, WalletService>()
            .AddScoped<ITransactionService, TransactionService>()
            .AddScoped<IAdvancedWalletsQueryService, AdvancedWalletsQueryService>()
            .AddScoped<IAdvancedTransactionsQueryService, AdvancedTransactionsQueryService>();
    }
}