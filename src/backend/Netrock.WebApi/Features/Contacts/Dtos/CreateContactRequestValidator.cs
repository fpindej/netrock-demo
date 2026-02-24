using FluentValidation;
using Netrock.WebApi.Shared;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Validates <see cref="CreateContactRequest"/> fields at runtime.
/// </summary>
public class CreateContactRequestValidator : AbstractValidator<CreateContactRequest>
{
    /// <summary>
    /// Initializes validation rules for contact creation requests.
    /// </summary>
    public CreateContactRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Company)
            .MaximumLength(200);

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.Source)
            .IsInEnum();

        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Notes)
            .MaximumLength(2000);

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .Matches(ValidationConstants.PhoneNumberPattern)
            .WithMessage("Phone number must be a valid format (e.g. +420123456789)")
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }
}
