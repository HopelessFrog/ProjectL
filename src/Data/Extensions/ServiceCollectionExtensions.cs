using Application;
using Application.Services;
using Application.Transactions;
using Application.Wallets;
using Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>();

        return services
            .AddScoped<IReadOnlyContext, ReadOnlyContext>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IWalletRepository, WalletRepository>()
            .AddScoped<IWalletReadRepository, WalletReadRepository>()
            .AddScoped<ITransactionsReadRepository, TransactionsReadRepository>();
    }
}