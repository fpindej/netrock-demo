using Microsoft.Extensions.DependencyInjection;
using Netrock.Application.Features.Contacts;
using Netrock.Infrastructure.Features.Contacts.Services;

namespace Netrock.Infrastructure.Features.Contacts.Extensions;

/// <summary>
/// Extension methods for registering contacts feature services.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the contacts services for CRUD and pipeline analytics operations.
        /// </summary>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddContactsServices()
        {
            services.AddScoped<IContactsService, ContactsService>();
            return services;
        }
    }
}
