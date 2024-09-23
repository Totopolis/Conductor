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

    public static void AddOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        // var toBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_instance}:{_token}"));

        var settings = configuration.ValidateAndReturnPreBuildSettings<
            OpenTelemetrySettings,
            OpenTelemetrySettingsValidator>(OpenTelemetrySettings.SectionName);

        if (!settings.EnableLogs &&
            !settings.EnableMetrics &&
            !settings.EnableTraces)
        {
            return;
        }

        var attributes = new Dictionary<string, object>
        {
            { "env", environment.EnvironmentName },
            { "host", Dns.GetHostName() }
        };

        if (settings.EnableMetrics || settings.EnableTraces)
        {
            var openTelemetryBuilder = services
                .AddOpenTelemetry()
                .ConfigureResource(rb =>
                {
                    rb.AddService(settings.ServiceName).AddAttributes(attributes);
                });

            if (settings.EnableTraces)
            {
                EnableTraces(openTelemetryBuilder, settings);
            }

            if (settings.EnableMetrics)
            {
                EnableMetrics(openTelemetryBuilder, settings);
            }
        }

        if (settings.EnableLogs)
        {
            // builder.Logging.ClearProviders();
            // EnableLogs(builder, settings, attributes);
        }
    }

    private static void EnableLogs(
        WebApplicationBuilder builder,
        OpenTelemetrySettings settings,
        Dictionary<string, object> attributes)
    {
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
    }

    private static void EnableTraces(
        OpenTelemetryBuilder openTelemetryBuilder,
        OpenTelemetrySettings settings)
    {
        openTelemetryBuilder
            .WithTracing(cfg =>
            {
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
        OpenTelemetrySettings settings)
    {
        openTelemetryBuilder
            .WithMetrics(cfg =>
            {
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
