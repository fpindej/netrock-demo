using Microsoft.Extensions.DependencyInjection;
using Netrock.Application.Features.Audit;
using Netrock.Infrastructure.Features.Audit.Services;

namespace Netrock.Infrastructure.Features.Audit.Extensions;

/// <summary>
/// Extension methods for registering audit feature services.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the audit services for event logging and retrieval.
        /// </summary>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddAuditServices()
        {
            services.AddScoped<IAuditService, AuditService>();
            return services;
        }
    }
}
