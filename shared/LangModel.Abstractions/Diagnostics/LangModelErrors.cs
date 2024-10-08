using ErrorOr;

namespace LangModel.Abstractions.Errors;

public static class LangModelErrors
{
    public static class ToolRequest
    {
        public static readonly Error ContentIsNullOrWhiteSpace = Error.Validation(
            code: "ToolRequest.ContentIsNullOrWhiteSpace",
            description: "Content can not be null or whitespace");

        public static readonly Error ContentIsNotCorrectJson = Error.Validation(
            code: "ToolRequest.ContentIsNotCorrectJson",
            description: "Content must be correct json string");

        public static readonly Error CastToDataError = Error.Validation(
            code: "ToolRequest.CastToDataError",
            description: "Can not cast json document to required data class");
    }
}
