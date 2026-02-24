using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Netrock.Infrastructure.Persistence;

namespace Netrock.Component.Tests.Fixtures;

internal static class TestDbContextFactory
{
    public static NetrockDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<NetrockDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new NetrockDbContext(options);
    }
}
