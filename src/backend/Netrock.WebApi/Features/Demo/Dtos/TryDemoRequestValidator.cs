using FluentValidation;

namespace Netrock.WebApi.Features.Demo.Dtos;

/// <summary>
/// Validates <see cref="TryDemoRequest"/> fields at runtime.
/// </summary>
public class TryDemoRequestValidator : AbstractValidator<TryDemoRequest>
{
    /// <summary>
    /// Initializes validation rules for demo account creation requests.
    /// </summary>
    public TryDemoRequestValidator()
    {
        RuleFor(x => x.CaptchaToken)
            .NotEmpty()
            .MaximumLength(8192);
    }
}
