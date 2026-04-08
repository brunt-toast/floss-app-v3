using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Database.Ioc;

/// <summary>
/// Factory for creating an AppDbContext at design-time (e.g., for EF migrations).
/// Uses environment-based configuration to discover the connection string.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? "Data Source=app.db";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite(connectionString,
            sqliteOptions => sqliteOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name));

        return new AppDbContext(optionsBuilder.Options);
    }
}
