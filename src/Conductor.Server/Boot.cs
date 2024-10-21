using Conductor.Api;
using Conductor.Application;
using Conductor.Infrastructure;
using FastEndpoints;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.AspNetCore.Http.Json;
using Server.Boot;

namespace Conductor.Server;

public static class Boot
{
    public static void PreBuild(this WebApplicationBuilder builder)
    {
        // ATTENTION: Must be first in order.
        builder.Configuration.MergeServerSettingsFromConsulIfNeed(builder.Configuration);

        // https://fast-endpoints.com/docs/configuration-settings#specify-json-serializer-options
        builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.WriteIndented = true);

        builder.Services.AddHttpClient();

        builder.Services
            .AddOpenTelemetryLogs(builder)
            .AddOpenTelemetryTracesOrMetrics(
                builder,
                tracerProviderBuilder: tpb =>
                {
                    // Masstransit
                    tpb.AddSource(DiagnosticHeaders.DefaultListenerName);
                },
                meterProviderBuilder: mpb =>
                {
                    // Masstransit
                    mpb.AddMeter(InstrumentationOptions.MeterName);
                });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSystemVersion();
        // builder.Services.AddMaintenanceBot(builder.Configuration);

        builder.Services.AddMasstransitLocal(builder.Configuration);

        builder.Services
            // .AddLangModelOpenAiServices(builder.Configuration)
            .AddApplicationServices(builder.Configuration)
            .AddInfrastructureServices(builder.Configuration)
            .AddApiServices(builder.Configuration);
    }

    public static async Task PostBuild(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseFastEndpoints();

        await Task.CompletedTask;
    }
}
