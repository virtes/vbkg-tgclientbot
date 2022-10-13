using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

// ReSharper disable once CheckNamespace
namespace VBkg.TgClientBot.Data;

public static class HostExtensions
{
    public static void MigrateAppDbContext(this IHost host)
    {
        try
        {
            Log.Information("Starting to migrate database...");

            using var scope = host.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();

            Log.Information("Database migrated");
        }
        catch (Exception e)
        {
            Log.Error(e, "Exception while migrating database");
            throw;
        }
    }
}