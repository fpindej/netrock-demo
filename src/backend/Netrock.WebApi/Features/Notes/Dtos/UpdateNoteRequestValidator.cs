using FluentValidation;

namespace Netrock.WebApi.Features.Notes.Dtos;

/// <summary>
/// Validates <see cref="UpdateNoteRequest"/> fields at runtime.
/// </summary>
public class UpdateNoteRequestValidator : AbstractValidator<UpdateNoteRequest>
{
    /// <summary>
    /// Initializes validation rules for note update requests.
    /// </summary>
    public UpdateNoteRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(10000);

        RuleFor(x => x.Category)
            .IsInEnum();
    }
}
