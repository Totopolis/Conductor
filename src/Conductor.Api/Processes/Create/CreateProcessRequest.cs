namespace Conductor.Api.Processes.Create;

public record CreateProcessRequest(
    string Name,
    string DisplayName,
    string Description);
