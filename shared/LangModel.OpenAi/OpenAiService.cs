using LangModel.Abstractions;
using LangModel.Abstractions.Errors;
using LangModel.Abstractions.Vectorize;
using LangModel.Tooling.Abstractions;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace LangModel.OpenAi;

internal class OpenAiService : ILangModel
{
    public const decimal Incoming1kCost = 0.0432m;
    public const decimal Outcoming1kCost = 0.1728m;

    private readonly IOpenAIService _ai;
    private readonly IEnumerable<IToolDefinition> _allTools;
    private readonly IServiceProvider _serviceProvider;

    public OpenAiService(
        IOpenAIService ai,
        IEnumerable<IToolDefinition> allTools,
        IServiceProvider serviceProvider)
    {
        _ai = ai;
        _allTools = allTools;
        _serviceProvider = serviceProvider;
    }

    public IQuestionBuilder CreateQuestion()
    {
        // TODO: remove from oai to factory???
        return new OpenAiQuestionBuilder(_allTools);
    }

    public async Task<Answer> Ask(Question question, CancellationToken ct)
    {
        var aiQuestion = question as OpenAiQuestion;
        if (aiQuestion is null)
        {
            throw new BadQuestionException();
        }

        try
        {
            var aiRequest = new ChatCompletionCreateRequest
            {
                Messages = aiQuestion.Messages.ToList(),
                // ResponseFormat = new ResponseFormat
                // {
                // Type = StaticValues.CompletionStatics.ResponseFormat.Json
                // },
                Tools = aiQuestion.AvailableTools.ToList(),
                Model = Models.Gpt_4o_mini,
                Temperature = 0.1f,
                // optional
                MaxTokens = 1000
            };

            var messagesToAnswer = new List<ChatMessage>();
            var accumStat = new AccumStat();

            await foreach (var message in ProcessRequest(aiRequest, accumStat, ct))
            {
                messagesToAnswer.Add(message);
            }

            var totalSpent = Incoming1kCost * accumStat.PromptsTokens / 1000m +
                    Outcoming1kCost * accumStat.CompletionsTokens / 1000m;

            var stat = new AnswerStat(
                PromptsTokens: accumStat.PromptsTokens,
                PromptsPrice: accumStat.PromptsTokens * Incoming1kCost / 1000m,
                CompletionsTokens: accumStat.CompletionsTokens,
                CompletionsPrice: accumStat.CompletionsTokens * Outcoming1kCost / 1000m,
                TotalSpent: totalSpent,
                LangModelCalls: accumStat.LangModelCalls,
                LangModelDuration: accumStat.LangModelDuration,
                ToolsCalls: accumStat.ToolsCalls,
                ToolsDuration: accumStat.ToolsDuration);

            var answer = new OpenAiAnswer(messagesToAnswer)
            {
                Stat = stat
            };

            return answer;
        }
        catch (Exception ex)
        {
            throw new BadAnswerException("Working with OpenAi error", ex);
        }
    }

    private async IAsyncEnumerable<ChatMessage> ProcessRequest(
        ChatCompletionCreateRequest request,
        AccumStat accumStat,
        [EnumeratorCancellation] CancellationToken ct)
    {
        while (true)
        {
            var startChat = DateTime.UtcNow;
            var response = await _ai.ChatCompletion.CreateCompletion(request);

            accumStat.LangModelCalls++;
            accumStat.LangModelDuration += (DateTime.UtcNow - startChat);

            if (!response.Successful)
            {
                throw new BadAnswerException(response.Error!.Message!);
            }

            accumStat.PromptsTokens += response.Usage.PromptTokens;
            accumStat.CompletionsTokens += response.Usage.CompletionTokens ?? 0;

            var choice = response.Choices.FirstOrDefault();
            if (choice is null)
            {
                throw new BadAnswerException("No choises in response");
            }

            var message = choice.Message;

            if (message.ToolCalls is not null)
            {
                request.Messages.Add(message);
                
                // Tooling message
                yield return message;

                foreach (var toolCall in message.ToolCalls)
                {
                    var fn = toolCall.FunctionCall;
                    if (fn is null)
                    {
                        throw new BadAnswerException("Tool call doesnt contains function call");
                    }

                    var toolExecutor = _serviceProvider.GetRequiredKeyedService<IToolExecutor>(
                        fn.Name);
                    
                    var startTool = DateTime.UtcNow;
                    var toolResponse = await toolExecutor.Run(
                        fn.Arguments ?? "{}",
                        ct);

                    accumStat.ToolsCalls++;
                    accumStat.ToolsDuration += (DateTime.UtcNow - startTool);

                    var toolMessage = ChatMessage.FromTool(toolResponse, toolCall.Id!);
                    request.Messages.Add(toolMessage);

                    // Tool message
                    yield return toolMessage;
                }
            }
            else
            {
                // Assistant answer
                yield return message;
                yield break;
            }
        }
    }

    public async Task<VectorizeResponse> Vectorize(
        VectorizeRequest request,
        CancellationToken ct)
    {
        var usage = 0;
        var zz = new List<Vector<double>>();
        var xx = request.Content
            .Chunk(100)
            .ToList();

        foreach (var it in xx)
        {
            var embeddindResponse = await _ai.Embeddings
                    .CreateEmbedding(new EmbeddingCreateRequest()
                    {
                        InputAsList = it.ToList(),
                        Model = Models.TextEmbeddingV3Large
                    }, ct);

            if (!embeddindResponse.Successful)
            {
                throw new VectorizationErrorException(
                    embeddindResponse.Error?.Message ?? string.Empty);
            }

            usage += embeddindResponse.Usage.TotalTokens;

            var vectors = embeddindResponse.Data
                .OrderBy(x => x.Index)
                .Select(x => Vector<double>.Build.DenseOfArray(x.Embedding.ToArray()));

            zz.AddRange(vectors);
        }

        var response = new VectorizeResponse(
            Embeddings: zz.ToImmutableArray(),
            TokensUsage: usage);

        return response;
    }

    private class AccumStat
    {
        public int LangModelCalls { get; set; } = 0;

        public TimeSpan LangModelDuration { get; set; } = TimeSpan.FromSeconds(0);

        public int ToolsCalls { get; set; } = 0;

        public TimeSpan ToolsDuration { get; set; } = TimeSpan.FromSeconds(0);

        public int PromptsTokens { get; set; } = 0;

        public int CompletionsTokens { get; set; } = 0;
    }
}
