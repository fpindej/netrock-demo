using Microsoft.Extensions.DependencyInjection;
using Netrock.Application.Features.Contacts;
using Netrock.Infrastructure.Features.Contacts.Services;

namespace Netrock.Infrastructure.Features.Contacts.Extensions;

/// <summary>
/// Extension methods for registering contact feature services.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the contact services for CRM contact management.
        /// </summary>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddContactServices()
        {
            services.AddScoped<IContactService, ContactService>();
            return services;
        }
    }
}
