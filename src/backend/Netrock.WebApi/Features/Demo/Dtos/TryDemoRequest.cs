using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Demo.Dtos;

/// <summary>
/// Request to create a short-lived demo account. Requires a valid CAPTCHA token.
/// </summary>
public class TryDemoRequest
{
    /// <summary>
    /// The Cloudflare Turnstile CAPTCHA token proving the caller is human.
    /// </summary>
    [Required]
    [MaxLength(8192)]
    public string CaptchaToken { get; [UsedImplicitly] init; } = string.Empty;
}
