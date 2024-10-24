﻿using LangModel.Abstractions.Answerizer;
using LangModel.Abstractions.Common;
using LangModel.Abstractions.Diagnostics;
using LangModel.Abstractions.Errors;
using LangModel.Tooling.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace LangModel.OpenAi.Answerizer;

internal sealed class AnswerizerService : IAnswerizerService
{
    public const decimal Incoming1kCost = 0.72m;
    public const decimal Outcoming1kCost = 2.88m;

    private readonly IOpenAIService _ai;
    private readonly IEnumerable<IToolDefinition> _allTools;
    private readonly IServiceProvider _serviceProvider;
    private readonly TracerComposite _tracerComposite;

    public AnswerizerService(
        IOpenAIService ai,
        IEnumerable<IToolDefinition> allTools,
        IServiceProvider serviceProvider,
        TracerComposite tracerComposite)
    {
        _ai = ai;
        _allTools = allTools;
        _serviceProvider = serviceProvider;
        _tracerComposite = tracerComposite;
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
                Model = Models.Gpt_4o,
                Temperature = 0.1f,
                // optional
                MaxTokens = 2000
            };

            var messagesToAnswer = new List<ChatMessage>();

            var usageValue = await ProcessReq(
                question.CorrelationId,
                aiRequest,
                messagesToAnswer,
                ct);

            var answer = new OpenAiAnswer(messagesToAnswer)
            {
                Usage = usageValue
            };

            return answer;
        }
        catch (Exception ex)
        {
            throw new BadAnswerException("Working with OpenAi error", ex);
        }
    }

    private async Task<UsageValue> ProcessReq(
        Guid correlationId,
        ChatCompletionCreateRequest request,
        IList<ChatMessage> feed,
        CancellationToken ct)
    {
        var result = UsageValue.Empty;

        while (true)
        {
            var startChat = DateTime.UtcNow;
            var response = await _ai.ChatCompletion.CreateCompletion(request);
            if (!response.Successful)
            {
                throw new BadAnswerException(response.Error!.Message!);
            }

            var stepUsage = UsageValue.CreateSingleAnswerizer(
                promptTokens: response.Usage.PromptTokens,
                prompt1kCost: Incoming1kCost,
                completionTokens: response.Usage.CompletionTokens,
                completion1kCost: Outcoming1kCost,
                span: DateTime.UtcNow - startChat);

            result += stepUsage;

            var choice = response.Choices.FirstOrDefault();
            if (choice is null)
            {
                throw new BadAnswerException("No choises in response");
            }

            var message = choice.Message;

            var options1 = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true
            };

            // Trace step
            await _tracerComposite.Trace(
                kind: LangModelTracerKind.Complete,
                request: JsonSerializer.SerializeToElement(request.Messages, options1),
                response: JsonSerializer.SerializeToElement(message, options1),
                usage: stepUsage);

            if (message.ToolCalls is not null)
            {
                request.Messages.Add(message);

                // Tooling message
                feed.Add(message);

                foreach (var toolCall in message.ToolCalls)
                {
                    var fn = toolCall.FunctionCall;
                    if (fn is null)
                    {
                        throw new BadAnswerException("Tool call doesnt contains function call");
                    }

                    var toolExecutor = _serviceProvider.GetRequiredKeyedService<IToolExecutor>(
                        fn.Name);

                    var toolRequestResult = ToolRequest.Create(
                        correlationId,
                        fn.Arguments ?? "{}");

                    if (toolRequestResult.IsError)
                    {
                        throw new BadAnswerException("Incorrect tool request");
                    }

                    var toolResponseResult = await toolExecutor.Run(
                        request: toolRequestResult.Value,
                        ct);

                    if (toolResponseResult.IsError)
                    {
                        throw new BadAnswerException("Error while tool executing");
                    }

                    var toolResponse = toolResponseResult.Value;

                    result += toolResponse.Usage;
                    
                    var toolMessage = ChatMessage.FromTool(
                        toolResponse.Content,
                        toolCall.Id!);

                    request.Messages.Add(toolMessage);

                    // Tool message
                    feed.Add(toolMessage);
                }
            }
            else
            {
                // Assistant answer
                feed.Add(message);
                break;
            }
        }

        return result;
    }
}
