using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netrock.Application.Features.Email;
using Netrock.Infrastructure.Features.Email.Options;

namespace Netrock.Infrastructure.Features.Email.Services;

/// <summary>
/// Email service implementation that sends emails via the Resend HTTP API.
/// </summary>
internal class ResendEmailService(
    HttpClient httpClient,
    IOptions<EmailOptions> emailOptions,
    ILogger<ResendEmailService> logger) : IEmailService
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    /// <inheritdoc />
    public async Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var from = $"{_emailOptions.FromName} <{_emailOptions.FromAddress}>";

        var payload = new ResendRequest(
            From: from,
            To: [message.To],
            Subject: message.Subject,
            Html: message.HtmlBody,
            Text: message.PlainTextBody);

        logger.LogDebug("Sending email via Resend to {To} with subject {Subject}", message.To, message.Subject);

        var response = await httpClient.PostAsJsonAsync("emails", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError(
                "Resend API returned {StatusCode}: {Body}",
                (int)response.StatusCode,
                body);

            response.EnsureSuccessStatusCode();
        }

        logger.LogInformation("Email sent via Resend to {To}", message.To);
    }

    /// <summary>
    /// Request payload for the Resend <c>POST /emails</c> endpoint.
    /// </summary>
    private sealed record ResendRequest(
        [property: JsonPropertyName("from")] string From,
        [property: JsonPropertyName("to")] string[] To,
        [property: JsonPropertyName("subject")] string Subject,
        [property: JsonPropertyName("html")] string Html,
        [property: JsonPropertyName("text")] string? Text);
}
