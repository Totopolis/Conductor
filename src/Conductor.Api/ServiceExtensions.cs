using Conductor.Api.Processes.Create;
using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddTransient<IValidator<CreateProcessRequest>, CreateProcessRequestValidator>();
        services.AddAutoMapper(typeof(CreateProcessMapper));

        return services;
    }
}
