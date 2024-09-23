namespace LangModel.OpenAi;

public record OpenAiSettings
{
    public const string SectionName = "OpenAi";

    public required string BaseDomain { get; init; }

    public required string ApiKey { get; init; }
}
