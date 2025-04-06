using ErrorOr;

namespace Lang.Application.Diagnostics;

public static class ApplicationErrors
{
    public static readonly Error IncorrectPrompt = Error.Validation(
        code: "Lang.Application.IncorrectPrompt",
        description: "Incorrect prompt content");

    public static readonly Error IncorrectQuestion = Error.Validation(
        code: "Lang.Application.IncorrectQuestion",
        description: "Incorrect question content");

    public static readonly Error AskError = Error.Unexpected(
        code: "Lang.Application.AskError",
        description: "Ask unexpected error");
}
