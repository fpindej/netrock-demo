using System.Net;

namespace Netrock.Infrastructure.Features.Email;

/// <summary>
/// Provides shared HTML and plain-text layout rendering for transactional emails.
/// All styles are inline for maximum email-client compatibility.
/// </summary>
internal static class EmailLayout
{
    /// <summary>
    /// Wraps inner HTML content in a full HTML5 email document with header, content area, and footer.
    /// When <paramref name="supportEmail"/> is provided, a "Questions?" contact line is included in the footer.
    /// </summary>
    internal static string RenderHtml(string innerHtml, string appName, string? supportEmail = null)
    {
        var safeAppName = WebUtility.HtmlEncode(appName);

        var contactLine = string.IsNullOrWhiteSpace(supportEmail)
            ? string.Empty
            : $"""<br/>Questions? Contact us at <a href="mailto:{WebUtility.HtmlEncode(supportEmail)}" style="color:#71717a;">{WebUtility.HtmlEncode(supportEmail)}</a>""";

        return $$"""
            <!DOCTYPE html>
            <html lang="en" xmlns="http://www.w3.org/1999/xhtml" xmlns:v="urn:schemas-microsoft-com:vml" xmlns:o="urn:schemas-microsoft-com:office:office">
            <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <meta http-equiv="X-UA-Compatible" content="IE=edge" />
                <meta name="x-apple-disable-message-reformatting" />
                <title>{{safeAppName}}</title>
                <!--[if mso]>
                <noscript>
                    <xml>
                        <o:OfficeDocumentSettings>
                            <o:AllowPNG/>
                            <o:PixelsPerInch>96</o:PixelsPerInch>
                        </o:OfficeDocumentSettings>
                    </xml>
                </noscript>
                <![endif]-->
                <style>
                    body, table, td, a { -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }
                    table, td { mso-table-lspace: 0pt; mso-table-rspace: 0pt; }
                    body { margin: 0; padding: 0; width: 100% !important; height: 100% !important; }
                </style>
            </head>
            <body style="margin:0; padding:0; background-color:#f4f4f5; font-family:-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;">
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background-color:#f4f4f5;">
                    <tr>
                        <td align="center" style="padding:24px 16px;">
                            <table role="presentation" width="600" cellpadding="0" cellspacing="0" style="max-width:600px; width:100%;">
                                <!-- Header -->
                                <tr>
                                    <td style="background-color:#18181b; padding:24px 32px; border-radius:8px 8px 0 0; text-align:center;">
                                        <h1 style="margin:0; color:#ffffff; font-size:20px; font-weight:600; letter-spacing:0.5px;">{{safeAppName}}</h1>
                                    </td>
                                </tr>
                                <!-- Content -->
                                <tr>
                                    <td style="background-color:#ffffff; padding:32px; font-size:16px; line-height:1.6; color:#27272a;">
                                        {{innerHtml}}
                                    </td>
                                </tr>
                                <!-- Footer -->
                                <tr>
                                    <td style="background-color:#ffffff; padding:0 32px;">
                                        <hr style="border:none; border-top:1px solid #e4e4e7; margin:0;" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="background-color:#ffffff; padding:20px 32px 24px; border-radius:0 0 8px 8px; text-align:center; font-size:13px; color:#71717a; line-height:1.5;">
                                        Sent by {{safeAppName}}{{contactLine}}
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;
    }

    /// <summary>
    /// Wraps inner plain-text content with an app-name header, separator lines, and footer.
    /// When <paramref name="supportEmail"/> is provided, a contact line is included in the footer.
    /// </summary>
    internal static string RenderPlainText(string innerText, string appName, string? supportEmail = null)
    {
        var contactLine = string.IsNullOrWhiteSpace(supportEmail)
            ? string.Empty
            : $"\nQuestions? Contact us at {supportEmail}";

        return $"""
            {appName}
            ────────────────────────────────

            {innerText}

            ────────────────────────────────
            Sent by {appName}{contactLine}
            """;
    }

    /// <summary>
    /// Returns the standard HTML note shown below a CTA button: link expiration and safety disclaimer.
    /// </summary>
    internal static string ExpiryAndSafetyNote(int expiresInHours, string safetyMessage)
    {
        return $"""
            <p style="margin:0 0 4px; font-size:13px; color:#71717a;">{WebUtility.HtmlEncode(safetyMessage)}</p>
            <p style="margin:0; font-size:13px; color:#71717a;">This link will expire in {expiresInHours} hours.</p>
            """;
    }

    /// <summary>
    /// Returns a bulletproof CTA button using the table+VML pattern for Outlook compatibility.
    /// </summary>
    internal static string Button(string url, string label)
    {
        var safeUrl = WebUtility.HtmlEncode(url);
        var safeLabel = WebUtility.HtmlEncode(label);

        return $"""
            <table role="presentation" cellpadding="0" cellspacing="0" style="margin:24px 0;">
                <tr>
                    <td align="center" style="border-radius:6px; background-color:#18181b;">
                        <!--[if mso]>
                        <v:roundrect xmlns:v="urn:schemas-microsoft-com:vml" xmlns:w="urn:schemas-microsoft-com:office:word"
                            href="{safeUrl}" style="height:44px; v-text-anchor:middle; width:220px;" arcsize="14%" strokecolor="#18181b" fillcolor="#18181b">
                            <w:anchorlock/>
                            <center style="color:#ffffff; font-family:sans-serif; font-size:15px; font-weight:600;">{safeLabel}</center>
                        </v:roundrect>
                        <![endif]-->
                        <!--[if !mso]><!-->
                        <a href="{safeUrl}" target="_blank"
                           style="display:inline-block; padding:12px 32px; background-color:#18181b; color:#ffffff; text-decoration:none; font-size:15px; font-weight:600; border-radius:6px; line-height:20px;">
                            {safeLabel}
                        </a>
                        <!--<![endif]-->
                    </td>
                </tr>
            </table>
            """;
    }
}
