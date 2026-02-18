using FluentValidation;
using Netrock.Application.Identity.Constants;

namespace Netrock.WebApi.Features.Demo.Dtos;

/// <summary>
/// Validates <see cref="SwitchRoleRequest"/> fields at runtime.
/// </summary>
public class SwitchRoleRequestValidator : AbstractValidator<SwitchRoleRequest>
{
    /// <summary>
    /// Initializes validation rules for role switch requests.
    /// </summary>
    public SwitchRoleRequestValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role => AppRoles.All.Contains(role))
            .WithMessage($"Role must be one of: {string.Join(", ", AppRoles.All)}");
    }
}
