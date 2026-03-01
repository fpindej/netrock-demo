using FluentValidation;

namespace Netrock.WebApi.Features.Authentication.Dtos.TwoFactor;

/// <summary>
/// Validates <see cref="TwoFactorRegenerateCodesRequest"/> fields at runtime.
/// </summary>
public class TwoFactorRegenerateCodesRequestValidator : AbstractValidator<TwoFactorRegenerateCodesRequest>
{
    /// <summary>
    /// Initializes validation rules for 2FA recovery code regeneration requests.
    /// </summary>
    public TwoFactorRegenerateCodesRequestValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(255);
    }
}
