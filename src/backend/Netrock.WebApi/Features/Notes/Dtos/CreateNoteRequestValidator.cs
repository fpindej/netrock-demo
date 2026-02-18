using FluentValidation;

namespace Netrock.WebApi.Features.Notes.Dtos;

/// <summary>
/// Validates <see cref="CreateNoteRequest"/> fields at runtime.
/// </summary>
public class CreateNoteRequestValidator : AbstractValidator<CreateNoteRequest>
{
    /// <summary>
    /// Initializes validation rules for note creation requests.
    /// </summary>
    public CreateNoteRequestValidator()
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
