using FastEndpoints;
using Microsoft.Extensions.Logging;
using Server.Abstractions;

namespace Conductor.Api;

// https://fast-endpoints.com/docs/get-started#union-type-returning-handler
public class RootEndpoint : EndpointWithoutRequest<RootResponse>
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
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        _logger.LogWarning("Root endpoint handler executing");

        await SendAsync(new(
            Promt: "Hello funs!",
            Status: "Work",
            StartDateTime: _systemInfo.StartDateTime,
            BuildDateTime: _systemInfo.BuildDateTime,
            AssemblyConfiguration: ThisAssembly.Info.Configuration,
            AssemblyFileVersion: ThisAssembly.Info.FileVersion,
            AssemblyVersion: ThisAssembly.Info.Version,
            AssemblyInformationalVersion: ThisAssembly.Info.InformationalVersion));
    }
}

public record RootResponse(
    string Promt,
    string Status,
    DateTime StartDateTime,
    DateTime BuildDateTime,
    string AssemblyConfiguration,
    string AssemblyFileVersion,
    string AssemblyVersion,
    string AssemblyInformationalVersion);
