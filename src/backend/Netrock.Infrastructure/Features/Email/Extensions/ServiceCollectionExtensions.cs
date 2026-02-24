using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Netrock.Application.Features.Email;
using Netrock.Infrastructure.Features.Email.Options;
using Netrock.Infrastructure.Features.Email.Services;
using Resend;

namespace Netrock.Infrastructure.Features.Email.Extensions;

/// <summary>
/// Extension methods for registering email services and configuration.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers email options, the template rendering pipeline, and the appropriate email service.
        /// When a Resend API key is configured, <see cref="ResendEmailService"/> is used;
        /// otherwise, <see cref="NoOpEmailService"/> is registered as a development/test fallback.
        /// </summary>
        /// <param name="configuration">The application configuration for reading email options.</param>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddEmailServices(IConfiguration configuration)
        {
            services.AddOptions<EmailOptions>()
                .BindConfiguration(EmailOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var resendApiKey = configuration[$"{EmailOptions.SectionName}:Resend:ApiKey"];

            if (!string.IsNullOrWhiteSpace(resendApiKey))
            {
                services.AddOptions<ResendClientOptions>()
                    .Configure(opts => opts.ApiToken = resendApiKey);

                services.AddHttpClient<IResend, ResendClient>();
                services.AddScoped<IEmailService, ResendEmailService>();
            }
            else
            {
                services.AddScoped<IEmailService, NoOpEmailService>();
            }

            services.AddSingleton<IEmailTemplateRenderer, FluidEmailTemplateRenderer>();
            services.AddScoped<ITemplatedEmailSender, TemplatedEmailSender>();

            return services;
        }
    }
}
