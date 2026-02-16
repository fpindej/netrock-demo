using Microsoft.Extensions.DependencyInjection;
using Netrock.Application.Features.Captcha;
using Netrock.Infrastructure.Features.Captcha.Options;
using Netrock.Infrastructure.Features.Captcha.Services;

namespace Netrock.Infrastructure.Features.Captcha.Extensions;

/// <summary>
/// Extension methods for registering CAPTCHA verification services.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers Cloudflare Turnstile CAPTCHA services and configuration.
        /// </summary>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddCaptchaServices()
        {
            services.AddOptions<CaptchaOptions>()
                .BindConfiguration(CaptchaOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddHttpClient<ICaptchaService, TurnstileCaptchaService>();
            return services;
        }
    }
}
