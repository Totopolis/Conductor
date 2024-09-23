namespace LangModel.Tooling.Abstractions;

public interface IToolExecutor
{
    Task<string> Run(string argumentsJson, CancellationToken ct);
}
