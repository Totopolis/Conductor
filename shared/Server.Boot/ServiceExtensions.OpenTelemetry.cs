using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Server.Boot.OpenTelemetry;
using Settings.Extensions;
using System.Net;

namespace Server.Boot;

public static partial class ServiceExtensions
{
    private static Func<IReadOnlyDictionary<string, string>, HttpClient> _httpClientFactory = (headers) =>
    {
        // new OtlpHttpHandler(new HttpClientHandler())
        var httpClient = new HttpClient();
        if (headers.TryGetValue("Authorization", out var authValue))
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", authValue);
        }
        return httpClient;
    };

    public static IServiceCollection AddOpenTelemetryLogs(
        this IServiceCollection services,
        WebApplicationBuilder builder)
    {
        IWebHostEnvironment environment = builder.Environment;
        
        // var toBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_instance}:{_token}"));
        var settings = builder.Configuration.ValidateAndReturnPreBuildSettings<
            OpenTelemetrySettings,
            OpenTelemetrySettingsValidator>(OpenTelemetrySettings.SectionName);

        if (!settings.EnableLogs)
        {
            return services;
        }

        var attributes = new Dictionary<string, object>
        {
            { "env", environment.EnvironmentName },
            { "host", Dns.GetHostName() }
        };

        if (settings.SuppressConsole)
        {
            builder.Logging.ClearProviders();
        }

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddAttributes(attributes)
            .AddService(settings.ServiceName);

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeScopes = true;

            logging
                .SetResourceBuilder(resourceBuilder)
                // .AddConsoleExporter()
                .AddOtlpExporter(opt =>
                {
                    opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                    opt.Endpoint = new Uri(settings.BaseUrl + "/v1/logs");

                    opt.HttpClientFactory = () => _httpClientFactory(settings.Headers);
                });
        });

        return services;
    }

    public static IServiceCollection AddOpenTelemetryTracesOrMetrics(
        this IServiceCollection services,
        WebApplicationBuilder builder,
        Action<TracerProviderBuilder> tracerProviderBuilder,
        Action<MeterProviderBuilder> meterProviderBuilder)
    {
        IWebHostEnvironment environment = builder.Environment;

        var settings = builder.Configuration.ValidateAndReturnPreBuildSettings<
            OpenTelemetrySettings,
            OpenTelemetrySettingsValidator>(OpenTelemetrySettings.SectionName);

        if (!settings.EnableMetrics && !settings.EnableTraces)
        {
            return services;
        }

        var attributes = new Dictionary<string, object>
        {
            { "env", environment.EnvironmentName },
            { "host", Dns.GetHostName() }
        };

        var openTelemetryBuilder = services
            .AddOpenTelemetry()
            .ConfigureResource(rb =>
            {
                rb.AddService(settings.ServiceName).AddAttributes(attributes);
            });

        if (settings.EnableTraces)
        {
            EnableTraces(openTelemetryBuilder, settings, tracerProviderBuilder);
        }

        if (settings.EnableMetrics)
        {
            EnableMetrics(openTelemetryBuilder, settings, meterProviderBuilder);
        }

        return services;
    }

    private static void EnableTraces(
        OpenTelemetryBuilder openTelemetryBuilder,
        OpenTelemetrySettings settings,
        Action<TracerProviderBuilder> tracerProviderBuilder)
    {
        openTelemetryBuilder
            .WithTracing(cfg =>
            {
                tracerProviderBuilder(cfg);

                cfg.AddAspNetCoreInstrumentation()
                // .AddConsoleExporter()
                .AddOtlpExporter(opt =>
                {
                    opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                    opt.Endpoint = new Uri(settings.BaseUrl + "/v1/traces");

                    opt.HttpClientFactory = () => _httpClientFactory(settings.Headers);
                });
            });
    }

    private static void EnableMetrics(
        OpenTelemetryBuilder openTelemetryBuilder,
        OpenTelemetrySettings settings,
        Action<MeterProviderBuilder> meterProviderBuilder)
    {
        openTelemetryBuilder
            .WithMetrics(cfg =>
            {
                meterProviderBuilder(cfg);

                cfg.AddAspNetCoreInstrumentation()
                   .AddRuntimeInstrumentation()
                   .AddHttpClientInstrumentation()
                   // .AddConsoleExporter()
                   .AddOtlpExporter(opt =>
                   {
                       opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                       opt.Endpoint = new Uri(settings.BaseUrl + "/v1/metrics");

                       opt.HttpClientFactory = () => _httpClientFactory(settings.Headers);
                   });
            });
    }

    internal class OtlpHttpHandler : DelegatingHandler
    {
        public OtlpHttpHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var resp = base.Send(request, cancellationToken);
            var xx = resp.Content.ReadAsStringAsync();
            xx.Wait();
            var zz = xx.Result;
            return resp;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var resp = await base.SendAsync(request, cancellationToken);
            return resp;
        }
    }
}
