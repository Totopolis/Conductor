using Application.Shared;
using Application.Shared.Settings;
using Bi.Application;
using Bi.Application.Abstractions;
using Bi.Domain.Abstractions;
using Bi.Infrastructure.Database;
using Bi.Infrastructure.Repositories;
using Bi.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bi.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddBiInfrastructureOptions(
        this IServiceCollection services)
    {
        services.AddSettingsWithValidation<
            InfrastructureSettings,
            InfrastructureSettingsValidator,
            BiModule>();

        return services;
    }

    public static IServiceCollection AddBiInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IModule, BiModule>();

        // It is scoped service
        services.AddDbContext<BiDbContext>();

        services
            .AddScoped<IDbSourceRepository, DbSourceRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddTransient<IPostgresConnector, PostgresConnector>();

        return services;
    }

}
