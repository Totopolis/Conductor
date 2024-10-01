namespace Conductor.Api.Processes.Create;

public record CreateProcessResponse(
        Guid Id,
        string Name,
        string DisplayName,
        string Description,
        int Number);
