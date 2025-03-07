using ErrorOr;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Conductor.Api.Diagnostics;

public sealed class ErrorOrException : Exception
{
    private readonly IErrorOr _error;

    public ErrorOrException(IErrorOr error)
    {
        _error = error;
    }

    public bool IsError => _error.IsError;

    public IReadOnlyList<Error> Errors => _error.Errors ?? [];

    public ProblemDetails ConvertToProblem(
        string instance)
    {
        if (Errors.Any(e => e.Type == ErrorType.Unauthorized))
        {
            return new ProblemDetails
            {
                Instance = instance,
                Status = StatusCodes.Status401Unauthorized,
                Detail = Errors
                    .First(e => e.Type == ErrorType.Unauthorized)
                    .Description
            };
        }

        if (Errors.Any(e => e.Type == ErrorType.Forbidden))
        {
            return new ProblemDetails
            {
                Instance = instance,
                Status = StatusCodes.Status403Forbidden,
                Detail = Errors
                    .First(e => e.Type == ErrorType.Forbidden)
                    .Description
            };
        }

        if (Errors.Any(e => e.Type == ErrorType.Conflict))
        {
            return new ProblemDetails
            {
                Instance = instance,
                Status = StatusCodes.Status409Conflict,
                Detail = Errors
                    .First(e => e.Type == ErrorType.Conflict)
                    .Description
            };
        }

        if (Errors.Any(e => e.Type == ErrorType.NotFound))
        {
            return new ProblemDetails
            {
                Instance = instance,
                Status = StatusCodes.Status404NotFound,
                Detail = "Not found error",
                Errors = Errors.Select(x => new ProblemDetails.Error
                {
                    Code = x.Code,
                    Name = x.Description
                })
            };
        }

        if (Errors.Any(e => e.Type == ErrorType.Validation))
        {
            return new ProblemDetails
            {
                Instance = instance,
                Status = StatusCodes.Status400BadRequest,
                Detail = "Validation error",
                Errors = Errors.Select(x=>new ProblemDetails.Error
                {
                    Code = x.Code,
                    Name = x.Description
                })
            };
        }

        return new ProblemDetails
        {
            Instance = instance,
            Status = StatusCodes.Status500InternalServerError,
            Detail = "Internal server error",
            Errors = Errors.Select(x => new ProblemDetails.Error
            {
                Code = x.Code,
                Name = x.Description
            })
        };
    }
}
