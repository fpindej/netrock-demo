using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Netrock.Infrastructure.Features.Email.Options;

/// <summary>
/// Configuration options for the email service.
/// Maps to the "Email" section in appsettings.json.
/// </summary>
public sealed class EmailOptions : IValidatableObject
{
    public const string SectionName = "Email";

    /// <summary>
    /// Gets or sets the sender email address used in the "From" header.
    /// </summary>
    [Required]
    [EmailAddress]
    public string FromAddress { get; init; } = "noreply@example.com";

    /// <summary>
    /// Gets or sets the display name used in the "From" header alongside the address.
    /// </summary>
    [Required]
    public string FromName { get; init; } = "Netrock";

    /// <summary>
    /// Gets or sets the base URL of the frontend application, used to construct links in emails
    /// (e.g., password reset, email verification).
    /// </summary>
    [Required]
    public string FrontendBaseUrl { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the support/contact email address shown in email footers.
    /// When empty, the contact line is omitted from emails.
    /// </summary>
    public string SupportEmail { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the Resend API configuration. When <see cref="ResendOptions.ApiKey"/> is non-empty,
    /// the application uses Resend for email delivery; otherwise it falls back to the no-op logger.
    /// </summary>
    public ResendOptions Resend { get; init; } = new();

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Resend.ApiKey))
        {
            yield break;
        }

        if (!Resend.ApiKey.StartsWith("re_", StringComparison.Ordinal))
        {
            yield return new ValidationResult(
                "Resend API key must start with 're_'.",
                [$"{nameof(Resend)}.{nameof(ResendOptions.ApiKey)}"]);
        }

        if (string.IsNullOrWhiteSpace(FrontendBaseUrl))
        {
            yield return new ValidationResult(
                "FrontendBaseUrl is required when Resend is enabled.",
                [nameof(FrontendBaseUrl)]);
        }
    }

    /// <summary>
    /// Configuration options for the Resend email API (<see href="https://resend.com/docs/api-reference/emails/send-email"/>).
    /// </summary>
    public sealed class ResendOptions
    {
        /// <summary>
        /// Gets or sets the Resend API key (starts with <c>re_</c>).
        /// </summary>
        public string ApiKey { get; [UsedImplicitly] init; } = string.Empty;
    }
}
