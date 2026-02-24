using FluentValidation;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Validates <see cref="GenerateContactsRequest"/> fields at runtime.
/// </summary>
public class GenerateContactsRequestValidator : AbstractValidator<GenerateContactsRequest>
{
    /// <summary>
    /// Initializes validation rules for sample contact generation requests.
    /// </summary>
    public GenerateContactsRequestValidator()
    {
        RuleFor(x => x.Count)
            .InclusiveBetween(1, 100);
    }
}
