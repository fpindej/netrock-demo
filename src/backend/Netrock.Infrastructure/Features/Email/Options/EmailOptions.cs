using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Netrock.Infrastructure.Features.Email.Options;

/// <summary>
/// Configuration options for the email service.
/// Maps to the "Email" section in appsettings.json.
/// </summary>
public sealed class EmailOptions
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
    /// Gets or sets the SMTP connection configuration. Only required when a real SMTP email service is registered.
    /// </summary>
    public SmtpOptions Smtp { get; init; } = new();

    /// <summary>
    /// Gets or sets the Resend API configuration. When <see cref="ResendOptions.ApiKey"/> is set,
    /// the Resend-based email service is registered instead of the no-op fallback.
    /// </summary>
    public ResendOptions Resend { get; init; } = new();

    /// <summary>
    /// Configuration options for SMTP email delivery.
    /// </summary>
    public sealed class SmtpOptions
    {
        /// <summary>
        /// Gets or sets the SMTP server hostname.
        /// </summary>
        public string Host { get; [UsedImplicitly] init; } = string.Empty;

        /// <summary>
        /// Gets or sets the SMTP server port.
        /// </summary>
        public int Port { get; [UsedImplicitly] init; } = 587;

        /// <summary>
        /// Gets or sets the SMTP authentication username.
        /// </summary>
        public string Username { get; [UsedImplicitly] init; } = string.Empty;

        /// <summary>
        /// Gets or sets the SMTP authentication password.
        /// </summary>
        public string Password { get; [UsedImplicitly] init; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the SMTP connection should use SSL/TLS.
        /// </summary>
        public bool UseSsl { get; [UsedImplicitly] init; } = true;
    }

    /// <summary>
    /// Configuration options for Resend API email delivery.
    /// </summary>
    public sealed class ResendOptions
    {
        /// <summary>
        /// Gets or sets the Resend API key. When non-empty, the Resend email service is used.
        /// </summary>
        public string ApiKey { get; [UsedImplicitly] init; } = string.Empty;
    }
}
