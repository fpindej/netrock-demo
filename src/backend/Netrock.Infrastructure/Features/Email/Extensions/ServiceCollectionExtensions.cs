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
        /// Registers email options and the no-op email service implementation.
        /// Replace the service registration with a real SMTP or API-based implementation for production use.
        /// </summary>
        /// <param name="configuration">The application configuration for reading email options.</param>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddEmailServices(IConfiguration configuration)
        {
            services.AddOptions<EmailOptions>()
                .BindConfiguration(EmailOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddScoped<IEmailService, NoOpEmailService>();

            return services;
        }
    }
}
