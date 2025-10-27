using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrations(this DatabaseFacade database,
        CancellationToken token = default)
    {
        var migrator = database.GetService<IMigrator>();
        var migrations = await database.GetPendingMigrationsAsync(token);
        var connection = database.GetConnectionString()!;

        foreach (var migration in migrations) await migrator.MigrateAsync(migration, token);
    }
}