using Application.Shared.Abstractions;
using Domain.Shared;
using ErrorOr;
using Lang.Application.Abstractions;
using Lang.Application.Diagnostics;
using Lang.Contracts.Ask;
using Lang.Contracts.IntegrationEvents;
using MediatR;

namespace Lang.Application.Handlers;

internal sealed class AskHandler : IRequestHandler<
    AskCommand,
    ErrorOr<AskCommandResult>>
{
    private readonly IAnswerizer _completionizer;
    private readonly IEventBusPublisher _eventBus;
    private readonly TimeProvider _timeProvider;

    public AskHandler(
        IAnswerizer completionizer,
        IEventBusPublisher eventBus,
        TimeProvider timeProvider)
    {
        _completionizer = completionizer;
        _eventBus = eventBus;
        _timeProvider = timeProvider;
    }

    public async Task<ErrorOr<AskCommandResult>> Handle(
        AskCommand request,
        CancellationToken cancellationToken)
    {
        var prompt = request.Sequence.Items
            .FirstOrDefault(x => x.Role == "system");

        if (prompt is null)
        {
            return ApplicationErrors.IncorrectPrompt;
        }

        var questions = request.Sequence.Items
            .Where(x => x.Role == "user")
            .Select(x => x.Content)
            .ToList()
            .AsReadOnly();

        if (questions.Count == 0)
        {
            return ApplicationErrors.IncorrectQuestion;
        }

        try
        {
            var result = await _completionizer.CompleteSequence(
                prompt: prompt.Content,
                userQuestions: questions,
                needJson: request.NeedJson,
                ct: cancellationToken);

            // TODO: use trans outbox with domain layer
            await _eventBus.Publish(new AnswerFormedEvent(
                RequestId: request.RequestId,
                Timestamp: _timeProvider.GetInstantNow(),
                TotalDuration: result.Duration,
                PromptTokensCount: result.PromptTokensUsage,
                CompletionTokensCount: result.CompletionTokensUsage), cancellationToken);

            return new AskCommandResult(
                IsJson: request.NeedJson,
                Answer: result.Answer,
                Duration: result.Duration,
                PromptTokensUsage: result.PromptTokensUsage,
                CompletionTokensCount: result.CompletionTokensUsage,
                CacheHit: false);
        }
        catch
        {
            return ApplicationErrors.AskError;
        }
    }
}
