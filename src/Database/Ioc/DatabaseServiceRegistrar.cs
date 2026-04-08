using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Database.Ioc;

public static class DatabaseServiceRegistrar
{
    public static void RegisterDatabaseServices(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringKey = "ConnectionStrings:DefaultConnection")
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   $"Connection string '{connectionStringKey}' not found in configuration.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(connectionString,
                sqliteOptions => sqliteOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name));
        });
    }

    public static void RegisterDatabaseServices(
        this IServiceCollection services,
        string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(connectionString,
                sqliteOptions => sqliteOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name));
        });
    }
}
