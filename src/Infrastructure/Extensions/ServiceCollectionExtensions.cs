using Application.Services;
using Data.Executors;
using Infrastructure.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseRetryOptions(configuration);

        return services.AddScoped<ITransactionalExecutor, TransactionalExecutor>();
    }
    
    public static IServiceCollection AddDatabaseRetryOptions(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DatabaseRetryOptions>(
            configuration.GetSection(nameof(DatabaseRetryOptions)));

        return services;
    }
}