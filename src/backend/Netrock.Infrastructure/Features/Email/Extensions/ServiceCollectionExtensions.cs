using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Netrock.Application.Features.Email;
using Netrock.Infrastructure.Features.Email.Options;
using Netrock.Infrastructure.Features.Email.Services;

namespace Netrock.Infrastructure.Features.Email.Extensions;

/// <summary>
/// Extension methods for registering email services and configuration.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers email options and the appropriate email service implementation.
        /// When a Resend API key is configured, emails are sent via the Resend API;
        /// otherwise, a no-op service logs email details to Serilog.
        /// </summary>
        /// <param name="configuration">The application configuration for reading email options.</param>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddEmailServices(IConfiguration configuration)
        {
            services.AddOptions<EmailOptions>()
                .BindConfiguration(EmailOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var apiKey = configuration[$"{EmailOptions.SectionName}:Resend:ApiKey"];

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                services.AddHttpClient<IEmailService, ResendEmailService>(client =>
                {
                    client.BaseAddress = new Uri("https://api.resend.com/");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                });
            }
            else
            {
                services.AddScoped<IEmailService, NoOpEmailService>();
            }

            return services;
        }
    }
}
