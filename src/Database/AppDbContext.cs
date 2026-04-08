using Microsoft.EntityFrameworkCore;
using Database.Entities;

namespace Database;

/// <summary>
/// Main database context for the floss application.
/// Uses SQLite as the storage provider.
/// </summary>
public sealed class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The DbContext options configured via DI.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the DbSet for ColorSet entities.
    /// </summary>
    public DbSet<ColorSet> ColorSets { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for ColorSetRow entities.
    /// </summary>
    public DbSet<ColorSetRow> ColorSetRows { get; set; } = null!;

    public DbSet<HiddenBuiltinColorSet> HiddenBuiltinColorSets { get; set; } = null!;

    /// <summary>
    /// Configures the EF Core model when it is created.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure entity types.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure indexes
        modelBuilder.Entity<ColorSet>()
            .HasIndex(cs => cs.Name)
            .HasDatabaseName("IX_ColorSets_Name");

        modelBuilder.Entity<ColorSet>()
            .HasIndex(cs => cs.IsDeleted)
            .HasDatabaseName("IX_ColorSets_IsDeleted");

        modelBuilder.Entity<ColorSetRow>()
            .HasIndex(csr => csr.ColorSetId)
            .HasDatabaseName("IX_ColorSetRows_ColorSetId");

        modelBuilder.Entity<ColorSetRow>()
            .HasIndex(csr => csr.Floss)
            .HasDatabaseName("IX_ColorSetRows_Floss");

        // Configure composite unique constraint on ColorSetRow (ColorSetId, Floss)
        modelBuilder.Entity<ColorSetRow>()
            .HasIndex(csr => new { csr.ColorSetId, csr.Floss })
            .IsUnique()
            .HasDatabaseName("IX_ColorSetRows_ColorSetId_Floss_Unique");

        // Configure cascade delete relationship
        modelBuilder.Entity<ColorSet>()
            .HasMany(cs => cs.Colors)
            .WithOne(csr => csr.ColorSet)
            .HasForeignKey(csr => csr.ColorSetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
