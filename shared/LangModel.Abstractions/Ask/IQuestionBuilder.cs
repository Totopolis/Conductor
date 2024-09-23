namespace LangModel.Abstractions;

public interface IQuestionBuilder
{
    // IQuestionBuilder AddLimit(double money);

    IQuestionBuilder AddTooling(IEnumerable<string> toolsTags);

    IQuestionBuilder AddSystem(string prompt);

    IQuestionBuilder AddUser(string content);

    IQuestionBuilder AddAssistant(string content);

    IQuestionBuilder AddToolsRequest(IList<(
        string callId,
        string toolName,
        string arguments)> requests);

    IQuestionBuilder AddTool(string callId, string content);

    IQuestionBuilder AddSampleQuestion(string content);

    IQuestionBuilder AddSampleAnswer(string content);

    IQuestionBuilder AddUserQuestion(string content);

    Question Build();
}
