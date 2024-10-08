using ErrorOr;

namespace LangModel.Tooling.Abstractions;

public interface IToolExecutor
{
    Task<ErrorOr<ToolResponse>> Run(ToolRequest request, CancellationToken ct);
}
