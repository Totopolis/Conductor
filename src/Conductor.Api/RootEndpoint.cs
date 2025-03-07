using Application.Shared.Abstractions;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Conductor.Api;

public sealed class RootEndpoint : EndpointWithoutRequest<RootResponse>
{
    private readonly ILogger<RootEndpoint> _logger;
    private readonly ISystemInfo _systemInfo;

    public RootEndpoint(ILogger<RootEndpoint> logger, ISystemInfo systemInfo)
    {
        _logger = logger;
        _systemInfo = systemInfo;
    }

    public override void Configure()
    {
        Get("/");
        AllowAnonymous();

        Description(x =>
        {
            x.WithTags(Constants.AnonimTagName);
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        _logger.LogWarning("Root endpoint handler executing");

        await SendAsync(new(
            Promt: "Hello funs!",
            Status: "Work",
            Application: _systemInfo.ApplicationName,
            Environment: _systemInfo.EnvironmentName,
            StartDateTime: _systemInfo.StartDateTime,
            Uptime: _systemInfo.Uptime,
            BuildDateTime: _systemInfo.BuildDateTime,
            Version: _systemInfo.Version));
    }
}

public record RootResponse(
    string Promt,
    string Status,
    string Application,
    string Environment,
    DateTime StartDateTime,
    TimeSpan Uptime,
    DateTime BuildDateTime,
    string Version);
