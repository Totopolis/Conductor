using Application.Shared.Settings;
using Bi.Application;
using Bi.Infrastructure;
using Conductor.Api;
using Conductor.Api.Common;
using Conductor.Server.Settings;
using FastEndpoints;
using Infrastructure.Shared;
using Lang.Application;
using Lang.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Http.Json;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Scalar.AspNetCore;
using Server.Shared;
using System.Diagnostics;

namespace Conductor.Server;

internal static class Boot
{
    public static void PreBuild(this WebApplicationBuilder builder)
    {
        if (!builder.Environment.IsDevelopment() && !Debugger.IsAttached)
        {
            builder.Configuration.AddEnvironmentVariables("G_");
            builder.Configuration.MergeServerSettingsFromConsulIfNeedIn<ServerModule>(builder.Configuration);
        }
        else
        {
            builder.Configuration.AddUserSecrets<Program>();
        }

        var startupSettings = builder.Configuration.ValidateAndReturnPreBuildSettings<
            StartupSettings,
            StartupSettingsValidator,
            ServerModule>();

        // https://fast-endpoints.com/docs/configuration-settings#specify-json-serializer-options
        builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.WriteIndented = true);

        builder.Services
            .AddOpenTelemetryLogsTo<ServerModule>(builder)
            .AddOpenTelemetryTracesOrMetricsTo<ServerModule>(
                builder,
                tracerProviderBuilder: builder =>
                {
                    // Masstransit
                    // builder.AddSource(DiagnosticHeaders.DefaultListenerName);
                    // builder.AddSource(RequestActivities.ActivitiesName);
                },
                meterProviderBuilder: builder =>
                {
                    // MassTransit
                    // builder.AddMeter(InstrumentationOptions.MeterName);
                });

        if (startupSettings.Scalar.Enable)
        {
            builder.Services.AddOpenApi();
        }

        if (startupSettings.Cors.Enable)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(Api.Common.Constants.CorsPolicy, policy =>
                {
                    policy
                        .WithOrigins(startupSettings.Cors.Origins.ToArray())
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    //.AllowAnyOrigin();
                });
            });
        }

        builder.Services
            .AddSingleton<TimeProvider>(x => TimeProvider.System);

        builder.Services
            .AddBiApplicationOptions()
            .AddBiInfrastructureOptions()
            .AddLangApplicationOptions()
            .AddLangInfrastructureOptions();

        builder.Services.AddSettingsWithValidation<
            StartupSettings,
            StartupSettingsValidator,
            ServerModule>();

        // Services
        builder.Services
            .AddBiApplicationServices(builder.Configuration)
            .AddBiInfrastructureServices(builder.Configuration)
            .AddLangApplicationServices(builder.Configuration);

        // Infrastructure shared services: system info & eventBusPublisher
        builder.Services
            .AddSharedInfrastructureServices(builder.Configuration);

        // API
        builder.Services.AddApiServices(builder.Configuration);

        // Masstransit
        builder.Services.AddMasstransitLocal(builder.Configuration);
    }

    public static IServiceCollection AddMasstransitLocal(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddBiApplicationConsumers();

            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConcurrentMessageLimit = 3;
                cfg.PrefetchCount = 3;

                cfg.ConfigureJsonSerializerOptions(jsonOpt =>
                {
                    jsonOpt.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                    return jsonOpt;
                });

                cfg.UseMessageRetry(r =>
                {
                    r.Interval(3, TimeSpan.FromSeconds(5));
                });

                cfg.UseTimeout(r =>
                {
                    r.Timeout = TimeSpan.FromMinutes(2);
                });

                cfg.ConfigureEndpoints(context);
            });

        });

        return services;
    }

    public static Task PostBuild(this WebApplication app)
    {
        app.UseFastEndpoints();
        app.UseConductorExceptionHandler();

        var startupSettings = app.Configuration.ValidateAndReturnPreBuildSettings<
            StartupSettings,
            StartupSettingsValidator,
            ServerModule>();

        if (startupSettings.Scalar.Enable)
        {
            app.MapOpenApi();

            app.MapScalarApiReference(options =>
            {
                if (string.IsNullOrWhiteSpace(startupSettings.Scalar.Server))
                {
                    options.Servers = [];
                }
                else
                {
                    options.Servers = [new ScalarServer(startupSettings.Scalar.Server)];
                }
            });
        }

        if (startupSettings.Cors.Enable)
        {
            app.UseCors(Api.Common.Constants.CorsPolicy);
        }

        return Task.CompletedTask;
    }
}
