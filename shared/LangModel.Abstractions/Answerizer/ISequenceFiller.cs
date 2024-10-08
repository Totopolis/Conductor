namespace LangModel.Abstractions.Answerizer;

public interface ISequenceFiller
{
    void OnSystem(string prompt);

    void OnUser(string content);

    void OnAssistant(string content);

    void OnTool(string callId, string content);

    void OnToolsRequest(IList<(string callId, string toolName, string arguments)> requests);
}
