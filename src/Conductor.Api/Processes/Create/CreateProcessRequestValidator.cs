using FluentValidation;

namespace Conductor.Api.Processes.Create;

internal class CreateProcessRequestValidator : AbstractValidator<CreateProcessRequest>
{
    public CreateProcessRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.DisplayName)
            .NotEmpty();
    }
}
