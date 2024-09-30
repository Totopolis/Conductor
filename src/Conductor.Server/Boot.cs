using Conductor.Api;
using Conductor.Application;
using Conductor.Infrastructure;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Server.Boot;

namespace Conductor.Server;

public static class Boot
{
    public static void PreBuild(this WebApplicationBuilder builder)
    {
        // ATTENTION: Must be first in order.
        // builder.Configuration.MergeServerSettingsFromConsul(builder.Configuration);

        builder.Services
            .AddFastEndpoints(opt =>
            {
                opt.Assemblies = [typeof(RootEndpoint).Assembly];
            });

        // https://fast-endpoints.com/docs/configuration-settings#specify-json-serializer-options
        builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.WriteIndented = true);

        builder.Services.AddHttpClient();
        // builder.Services.AddOpenTelemetry(builder.Configuration, builder.Environment);

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
