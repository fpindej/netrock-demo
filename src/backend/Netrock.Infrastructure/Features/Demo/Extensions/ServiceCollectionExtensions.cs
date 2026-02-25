using Microsoft.Extensions.DependencyInjection;
using Netrock.Application.Features.Demo;
using Netrock.Infrastructure.Features.Demo.Services;

namespace Netrock.Infrastructure.Features.Demo.Extensions;

/// <summary>
/// Extension methods for registering demo feature services.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the demo services for role elevation.
        /// </summary>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddDemoServices()
        {
            services.AddScoped<IDemoService, DemoService>();
            return services;
        }
    }
}
