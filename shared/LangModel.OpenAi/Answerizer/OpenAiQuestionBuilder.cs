using LangModel.Abstractions.Answerizer;
using LangModel.Tooling.Abstractions;
using LangModel.Tooling.Abstractions.Arguments;
using OpenAI.Builders;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.SharedModels;
using System.Collections.Immutable;

namespace LangModel.OpenAi.Answerizer;

internal class OpenAiQuestionBuilder : IQuestionBuilder
{
    private readonly IReadOnlyList<IToolDefinition> _allTools;

    private ChatMessage _systemMessage = default!;
    private List<ChatMessage> _samplesPrompts = new();
    private List<ChatMessage> _fishMemory = new();
    private ChatMessage _userQuestion = default!;

    private readonly List<ToolDefinition> _availableTools = new();

    public OpenAiQuestionBuilder(IEnumerable<IToolDefinition> allTools)
    {
        _allTools = allTools.ToImmutableList();
    }

    public IQuestionBuilder AddTooling(IEnumerable<string> toolsTags)
    {
        var filtered = _allTools
            .Where(x => x.Tags.Any(y => toolsTags.Contains(y)))
            .Select(x =>
            {
                var builder = new FunctionDefinitionBuilder(x.Name, x.Description);
                foreach (var arg in x.Arguments)
                {
                    builder = arg switch
                    {
                        IntegerArgument intArg => builder.AddParameter(
                            name: arg.Name,
                            value: PropertyDefinition.DefineInteger(arg.Description),
                            required: arg.IsRequired),

                        IntegerArrayArgument intArrArg => builder.AddParameter(
                            name: arg.Name,
                            value: PropertyDefinition.DefineArray(PropertyDefinition.DefineInteger(arg.Description)),
                            required: arg.IsRequired),

                        StringArgument strArg => builder.AddParameter(
                            name: arg.Name,
                            value: PropertyDefinition.DefineString(arg.Description),
                            required: arg.IsRequired),

                        _ => throw new NotImplementedException()
                    };
                }

                var functionDefinition = builder.Build();
                return ToolDefinition.DefineFunction(functionDefinition);
            });

        _availableTools.AddRange(filtered);
        return this;
    }

    public IQuestionBuilder AddAssistant(string content)
    {
        _fishMemory.Add(ChatMessage.FromAssistant(content));
        return this;
    }

    public IQuestionBuilder AddSystem(string prompt)
    {
        _systemMessage = ChatMessage.FromSystem(prompt);
        return this;
    }

    public IQuestionBuilder AddTool(string callId, string content)
    {
        _fishMemory.Add(ChatMessage.FromTool(content, callId));
        return this;
    }

    public IQuestionBuilder AddToolsRequest(
        IList<(string callId, string toolName, string arguments)> requests)
    {
        var tooling = new List<ToolCall>();
        foreach (var request in requests)
        {
            tooling.Add(new ToolCall
            {
                Id = request.callId,
                Type = "function",
                FunctionCall = new FunctionCall
                {
                    Name = request.toolName,
                    Arguments = request.arguments
                }
            });
        }

        _fishMemory.Add(ChatMessage.FromAssistant(
            content: string.Empty,
            toolCalls: tooling));

        return this;
    }

    public IQuestionBuilder AddUser(string content)
    {
        _fishMemory.Add(ChatMessage.FromUser(content));
        return this;
    }

    public IQuestionBuilder AddSampleQuestion(string content)
    {
        _samplesPrompts.Add(ChatMessage.FromUser(content));
        return this;
    }

    public IQuestionBuilder AddSampleAnswer(string content)
    {
        _samplesPrompts.Add(ChatMessage.FromAssistant(content));
        return this;
    }

    public IQuestionBuilder AddUserQuestion(string content)
    {
        _userQuestion = ChatMessage.FromUser(content);
        return this;
    }

    public Question Build()
    {
        if (_systemMessage is null || _userQuestion is null)
        {
            throw new ApplicationException("bad system or user message");
        }

        return new OpenAiQuestion(
            systemMessage: _systemMessage,
            samplesPrompt: _samplesPrompts,
            fishMemory: _fishMemory,
            userQuestion: _userQuestion,
            availableTools: _availableTools);
    }
}
