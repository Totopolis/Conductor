using LangModel.Abstractions.Answerizer;
using OpenAI.ObjectModels.RequestModels;
using static OpenAI.ObjectModels.StaticValues;

namespace LangModel.OpenAi.Answerizer;

internal class OpenAiAnswer : Answer
{
    private readonly List<ChatMessage> _messages;

    public OpenAiAnswer(IEnumerable<ChatMessage> messages)
    {
        _messages = new(messages);
    }

    public override void FillSequence(ISequenceFiller filler)
    {
        Fill(_messages, filler);
    }

    public static void Fill(
        IEnumerable<ChatMessage> messages,
        ISequenceFiller filler)
    {
        foreach (ChatMessage message in messages)
        {
            if (message.Role == ChatMessageRoles.System)
            {
                filler.OnSystem(message.Content!);
            }
            else if (message.Role == ChatMessageRoles.User)
            {
                filler.OnUser(message.Content!);
            }
            else if (message.Role == ChatMessageRoles.Assistant)
            {
                if (message.ToolCalls != null)
                {
                    var tooling = new List<(string callId, string toolName, string arguments)>();
                    foreach (var toolCall in message.ToolCalls)
                    {
                        var xx = (
                            callId: toolCall.Id!,
                            toolName: toolCall.FunctionCall!.Name!,
                            arguments: toolCall.FunctionCall.Arguments!);

                        tooling.Add(xx);
                    }
                    filler.OnToolsRequest(tooling);
                }
                else
                {
                    filler.OnAssistant(message.Content!);
                }
            }
            else if (message.Role == ChatMessageRoles.Tool)
            {
                filler.OnTool(message.ToolCallId!, message.Content!);
            }
        }
    }
}
