using API.Middleware;
using Application;
using Data;
using FluentResults.Extensions.AspNetCore;
using FluentValidation;
using Host.Profiles;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using Infrastructure.Services.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace Host;

public static class Startup
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var connectionString = builder.Configuration.GetConnectionString("db");

            var retryOptions = sp.GetRequiredService<IOptions<DatabaseRetryOptions>>().Value;

            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: retryOptions.MaxRetryCount,
                        maxRetryDelay: TimeSpan.FromSeconds(retryOptions.MaxRetryDelaySeconds),
                        errorCodesToAdd: retryOptions.ErrorCodesToRetry);
                });
        });

        var services = builder.Services;

        services.AddControllers();

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        services.AddProblemDetails();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.FullName!.Replace("+", "."));
        });

        services.AddData();
        services.AddInfrastructure(builder.Configuration);
        services.AddApplication();

        builder.Services.AddSingleton(TimeProvider.System);

        services.AddHostedService<Migrator<AppDbContext>>();

        AspNetCoreResult.Setup(config => config.DefaultProfile = new FluentResultsEndpointProfile());
    }

    public static void ConfigureApp(this WebApplication app)
    {
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();
    }
}