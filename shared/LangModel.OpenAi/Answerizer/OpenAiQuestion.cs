using LangModel.Abstractions.Answerizer;
using OpenAI.ObjectModels.RequestModels;
using SharpToken;
using System.Collections.Immutable;
using static OpenAI.ObjectModels.StaticValues;

namespace LangModel.OpenAi.Answerizer;

internal sealed class OpenAiQuestion : Question
{
    private readonly Guid _correlationId;
    private readonly ChatMessage _systemMessage;
    private readonly ImmutableList<ChatMessage> _samplesPrompts;
    private readonly ImmutableList<ChatMessage> _fishMemory;
    private readonly ChatMessage _userQuestion;

    private readonly ImmutableList<ToolDefinition> _availableTools;

    internal OpenAiQuestion(
        Guid correlationId,
        ChatMessage systemMessage,
        IEnumerable<ChatMessage> samplesPrompt,
        IEnumerable<ChatMessage> fishMemory,
        ChatMessage userQuestion,
        IEnumerable<ToolDefinition> availableTools)
    {
        _correlationId = correlationId;
        _systemMessage = systemMessage;
        _samplesPrompts = samplesPrompt.ToImmutableList();
        _fishMemory = fishMemory.ToImmutableList();
        _userQuestion = userQuestion;
        _availableTools = availableTools.ToImmutableList();
    }

    public override Guid CorrelationId => _correlationId;

    public override decimal Cost
    {
        get
        {
            var encoder = GptEncoding.GetEncoding("o200k_base");
            // TODO: calculate tools price!!!
            var tokensCount = encoder.CountTokens(_systemMessage.Content) +
                _samplesPrompts.Sum(x => encoder.CountTokens(x.Content)) +
                _fishMemory.Sum(x => encoder.CountTokens(x.Content)) +
                encoder.CountTokens(_userQuestion.Content);

            var cost = AnswerizerService.Incoming1kCost * tokensCount / 1000m;
            return cost;
        }
    }

    public override bool NoSamplesAndNoFish =>
        !_samplesPrompts.Any() && !_fishMemory.Any();

    public IReadOnlyList<ChatMessage> Messages
    {
        get
        {
            IEnumerable<ChatMessage> chatMessages = [_systemMessage];
            chatMessages = chatMessages
                .Union(_samplesPrompts)
                .Union(_fishMemory)
                .Union([_userQuestion]);

            return chatMessages.ToImmutableList();
        }
    }

    public IReadOnlyList<ToolDefinition> AvailableTools => _availableTools;

    public override ushort FishBlocksCount => (ushort)_fishMemory
        .Count(x => x.Role == ChatMessageRoles.Assistant && x.ToolCalls is null);

    // Отрезается блоками [userQuestion, {assist-tool, tool}*, assist]
    // начиная со старых. Сначала рыбу, потом промпты с сэмплами.
    public override Question TrimTopBlockAndCreateNewQuestion()
    {
        var isAnswer = (ChatMessage msg) =>
            msg.Role == ChatMessageRoles.Assistant &&
            msg.ToolCalls is null;

        if (!_fishMemory.IsEmpty)
        {
            var sequence = _fishMemory.AsEnumerable()
                .SkipWhile(x => !isAnswer(x))
                // skip assistant answer
                .Skip(1);

            return new OpenAiQuestion(
                correlationId: _correlationId,
                systemMessage: _systemMessage,
                samplesPrompt: _samplesPrompts,
                fishMemory: sequence,
                userQuestion: _userQuestion,
                availableTools: _availableTools);
        }
        else if (!_samplesPrompts.IsEmpty)
        {
            var sequence = _samplesPrompts.AsEnumerable()
                .SkipWhile(x => !isAnswer(x))
                // skip assistant answer
                .Skip(1);

            return new OpenAiQuestion(
                correlationId: _correlationId,
                systemMessage: _systemMessage,
                samplesPrompt: sequence,
                fishMemory: [],
                userQuestion: _userQuestion,
                availableTools: _availableTools);
        }
        else
        {
            throw new ApplicationException("Bad OpenAiQuestion state");
            // return this;
        }
    }

    public override void FillFishSequence(ISequenceFiller fishFiller)
    {
        var sequence = _samplesPrompts
            .Union(_fishMemory)
            .Union([_userQuestion]);

        OpenAiAnswer.Fill(sequence, fishFiller);
    }
}
