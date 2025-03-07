using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Conductor.Api.Diagnostics;

public static class ExceptionHandlerExtensions
{
    public static IApplicationBuilder UseConductorExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errApp => errApp
            .Run(async ctx => await ExceptionsHandler(ctx)));

        return app;
    }

    private static async Task ExceptionsHandler(HttpContext ctx)
    {
        var exHandlerFeature = ctx.Features.Get<IExceptionHandlerFeature>();
        if (exHandlerFeature is null)
        {
            return;
        }

        var problemInstance = exHandlerFeature.Endpoint?.DisplayName?.Split(" => ")[0] ?? string.Empty;
        ctx.Response.ContentType = "application/problem+json";

        if (exHandlerFeature.Error is ErrorOrException errorOr)
        {
            var problem = errorOr.ConvertToProblem(problemInstance);
            ctx.Response.StatusCode = problem.Status;
            
            await ctx.Response.WriteAsJsonAsync(problem);
        }
        else
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // TODO: если в разработке или стейджинге - кидать на клиент трейсы ошибок!!!
            var problem = new ProblemDetails
            {
                Instance = problemInstance,
                Status = StatusCodes.Status500InternalServerError,
                Detail = "One or more errors occurred"
            };

            // TODO: писать в логгер
            await ctx.Response.WriteAsJsonAsync(problem);
        }
    }
}
