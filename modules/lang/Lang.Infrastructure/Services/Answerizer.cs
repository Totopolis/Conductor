using Betalgo.Ranul.OpenAI.Interfaces;
using Betalgo.Ranul.OpenAI.ObjectModels;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using Lang.Application.Abstractions;
using NodaTime;
using System.Diagnostics;

namespace Lang.Infrastructure.Services;

internal sealed class Answerizer : IAnswerizer
{
    private readonly IOpenAIService _ai;

    public Answerizer(IOpenAIService ai)
    {
        _ai = ai;
    }

    public async Task<AnswerizerResult> CompleteSequence(
        string prompt,
        IReadOnlyList<string> userQuestions,
        bool needJson,
        CancellationToken ct)
    {
        var stopWatch = Stopwatch.StartNew();

        var messages = new List<ChatMessage>
        {
            ChatMessage.FromSystem(prompt)
        };

        foreach (var question in userQuestions)
        {
            messages.Add(ChatMessage.FromUser(question));
        }

        var aiRequest = new ChatCompletionCreateRequest
        {
            Messages = messages,
            ResponseFormat = new ResponseFormat
            {
                Type = needJson ? StaticValues.CompletionStatics.ResponseFormat.Json :
                    StaticValues.CompletionStatics.ResponseFormat.Text
            },
            Tools = [],
            Model = Models.Gpt_4o_mini,
            Temperature = 0.1f,
            // optional
            MaxTokens = 4096
        };

        var response = await _ai.ChatCompletion.CreateCompletion(
            chatCompletionCreate: aiRequest,
            cancellationToken: ct);

        if (!response.Successful)
        {
            throw new InvalidOperationException(
                message: response.Error?.Message ?? string.Empty);
        }

        stopWatch.Stop();

        var choice = response.Choices.FirstOrDefault();
        if (choice is null)
        {
            throw new InvalidOperationException("No answer found");
        }

        var message = choice.Message;
        if (message.Content is null)
        {
            throw new InvalidOperationException("No answer content found");
        }

        return new AnswerizerResult(
            Answer: message.Content,
            Duration: Duration.FromTimeSpan(stopWatch.Elapsed),
            PromptTokensUsage: response.Usage.PromptTokens,
            CompletionTokensUsage: response.Usage.CompletionTokens ?? 0);
    }
}
