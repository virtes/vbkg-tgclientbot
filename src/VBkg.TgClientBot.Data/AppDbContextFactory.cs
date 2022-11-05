using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VBkg.TgClientBot.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        if (args.Length == 0)
            args = new[] { "appsettings.json" };

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(args[0], false)
            .Build();

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseNpgsql(configuration.GetConnectionString(nameof(AppDbContext)), o => o.CommandTimeout(7200));

        return new AppDbContext(builder.Options);
    }
}