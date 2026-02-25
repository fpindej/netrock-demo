using FluentValidation;

namespace Netrock.WebApi.Features.Demo.Dtos;

/// <summary>
/// Validates <see cref="ElevateRequest"/> fields at runtime.
/// </summary>
public class ElevateRequestValidator : AbstractValidator<ElevateRequest>
{
    /// <summary>
    /// Initializes validation rules for demo elevation requests.
    /// </summary>
    public ElevateRequestValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty()
            .MaximumLength(50);
    }
}
