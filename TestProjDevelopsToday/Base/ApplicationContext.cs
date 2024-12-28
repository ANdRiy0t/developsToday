using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestProjDevelopsToday.Models;

namespace TestProjDevelopsToday.Base;

public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        
        optionsBuilder.UseNpgsql(AppConfiguration.ConnectionString);

        return new ApplicationContext(optionsBuilder.Options);
    }
}

public class ApplicationContext : DbContext
{
     public DbSet<TripRecord> TripRecords { get; set; }
    
     public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
     {
     }
    
     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     {
         if (!optionsBuilder.IsConfigured)
         {
             optionsBuilder.UseNpgsql(AppConfiguration.ConnectionString);
         }
     }

    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(), 
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType.GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(UtcDateTimeAttribute)));

            foreach (var property in properties)
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(property.Name)
                    .HasConversion(dateTimeConverter);
            }
        }
        
        modelBuilder.Entity<TripRecord>(entity =>
        {
            entity.HasIndex(e => e.PULocationID).HasDatabaseName("IX_PULocationId");
            entity.HasIndex(e => e.TipAmount).HasDatabaseName("IX_TipAmount");
            entity.HasIndex(e => e.TripDistance).HasDatabaseName("IX_TripDistance");
            entity.HasIndex(e => new { e.PickupDate, e.DropoffDate }).HasDatabaseName("IX_TravelTime");
        });
    }

}