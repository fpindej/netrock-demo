using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netrock.Infrastructure.Features.Email.Options;
using Resend;
using AppEmailMessage = Netrock.Application.Features.Email.EmailMessage;
using IAppEmailService = Netrock.Application.Features.Email.IEmailService;

namespace Netrock.Infrastructure.Features.Email.Services;

/// <summary>
/// Sends emails via the Resend transactional email API.
/// Registered when <see cref="EmailOptions.ResendOptions.ApiKey"/> is configured.
/// </summary>
internal class ResendEmailService(
    IResend resendClient,
    IOptions<EmailOptions> emailOptions,
    ILogger<ResendEmailService> logger) : IAppEmailService
{
    /// <inheritdoc />
    public async Task SendEmailAsync(AppEmailMessage message, CancellationToken cancellationToken = default)
    {
        var options = emailOptions.Value;
        var from = new EmailAddress { Email = options.FromAddress, DisplayName = options.FromName };

        var resendMessage = new Resend.EmailMessage
        {
            From = from,
            To = message.To,
            Subject = message.Subject,
            HtmlBody = message.HtmlBody,
            TextBody = message.PlainTextBody
        };

        logger.LogInformation("Sending email via Resend to {To} | Subject: {Subject}", message.To, message.Subject);

        var emailId = await resendClient.EmailSendAsync(resendMessage, cancellationToken);

        logger.LogInformation("Email sent via Resend (ID: {EmailId}) to {To}", emailId, message.To);
    }
}
