using Microsoft.Extensions.DependencyInjection;
using Netrock.Application.Features.Notes;
using Netrock.Infrastructure.Features.Notes.Services;

namespace Netrock.Infrastructure.Features.Notes.Extensions;

/// <summary>
/// Extension methods for registering notes feature services.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the notes services for CRUD and analytics operations.
        /// </summary>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddNotesServices()
        {
            services.AddScoped<INotesService, NotesService>();
            return services;
        }
    }
}
