using EFDataAccessLib.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace EFDataAccessLib.DataAccess;

public class ProgDbContext : DbContext
{
    public DbConnection DbConnection { get; set; }
    //public ProgDbContext() { }


    public ProgDbContext(DbContextOptions<ProgDbContext> options) : base(options)
    {
        if (Database.IsRelational())
        {
            DbConnection = Database.GetDbConnection();
        }
        //bool isExists = Database.EnsureCreated();
        //if (!isExists) Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>()
            .HasIndex(a => a.Name)
            .IsUnique();

        modelBuilder.Entity<AppVersion>()
            .HasIndex(av => new { av.ApplicationId, av.Major, av.Minor, av.Patch })
            .IsUnique();

        modelBuilder.Entity<AppCompatibility>()
            .HasIndex(ac => new { ac.SourceVersionId, ac.TargetVersionId })
            .IsUnique();

        // Ensure AppVersion deletion cascades on Application delete
        modelBuilder.Entity<AppVersion>()
            .HasOne(av => av.Application)
            .WithMany(a => a.AppVersions)
            .HasForeignKey(av => av.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Explicitly define FK relationships for AppCompatibility
        modelBuilder.Entity<AppCompatibility>()
            .HasOne(ac => ac.SourceVersion)
            .WithMany(av => av.CompatibilitySourceVersions)
            .HasForeignKey(ac => ac.SourceVersionId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent multiple cascade paths issue

        modelBuilder.Entity<AppCompatibility>()
            .HasOne(ac => ac.TargetVersion)
            .WithMany(av => av.CompatibilityTargetVersions)
            .HasForeignKey(ac => ac.TargetVersionId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent multiple cascade paths issue
    }


    public DbSet<AppCompatibility> AppCompatibilities { get; set; }

    public DbSet<AppVersion> AppVersions { get; set; }

    public DbSet<Application> Applications { get; set; }

}
