using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Database.Ioc;

public static class DatabaseServiceRegistrar
{
    public static void RegisterDatabaseServices(
        this IServiceCollection services,
        string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        ConfigureDbContext(services, connectionString);
    }

    private static void ConfigureDbContext(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(connectionString,
                sqliteOptions => sqliteOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name));
        });
    }
}
