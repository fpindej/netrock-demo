using FluentValidation;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Validates <see cref="UpdateContactRequest"/> fields at runtime.
/// </summary>
public class UpdateContactRequestValidator : AbstractValidator<UpdateContactRequest>
{
    /// <summary>
    /// Initializes validation rules for contact update requests.
    /// </summary>
    public UpdateContactRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .MaximumLength(256)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Company)
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .MaximumLength(50);

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.Source)
            .IsInEnum();

        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Value.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(10000);
    }
}
