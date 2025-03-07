using ErrorOr;
using FastEndpoints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Conductor.Api.Diagnostics;

namespace Conductor.Api;

public static class ServiceExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddFastEndpoints(opt =>
            {
                opt.Assemblies = [typeof(ServiceExtensions).Assembly];
            });

        return services;
    }

    public static T ValueOrThrow<T>(this IErrorOr<T> errorOr)
    {
        if (errorOr.IsError)
        {
            throw new ErrorOrException(errorOr);
        }

        return errorOr.Value;
    }
}
