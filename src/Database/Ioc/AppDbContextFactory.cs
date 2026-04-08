using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Database.Ioc;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Data Source=app.db";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite(connectionString,
            sqliteOptions => sqliteOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name));

        var ret = new AppDbContext(optionsBuilder.Options);
        ret.Database.EnsureCreated();
        return ret;
    }
}
