using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Quickfire.Blazor.Data;

public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureMainEntities(modelBuilder);          // For DbSet declarations
        ConfigureColumnProperties(modelBuilder);      // For property/decimal precision configurations
        ConfigurePolicyRelationships(modelBuilder);   // For Policy-specific navigation relationships
        ConfigureGroupRelationships(modelBuilder);    // For Client, Lead, Carrier, etc., relationships
        ConfigureCommonRelationships(modelBuilder);   // For all other relationships

        if (Database.IsSqlite())
        {
            NormalizeSqliteColumnTypes(modelBuilder);
        }
    }

    // Partial method declarations for modular OnModelCreating logic
    partial void ConfigureMainEntities(ModelBuilder modelBuilder);
    partial void ConfigureColumnProperties(ModelBuilder modelBuilder);
    partial void ConfigurePolicyRelationships(ModelBuilder modelBuilder);
    partial void ConfigureGroupRelationships(ModelBuilder modelBuilder);
    partial void ConfigureCommonRelationships(ModelBuilder modelBuilder);

    private static void NormalizeSqliteColumnTypes(ModelBuilder modelBuilder)
    {
        var dateTimeOffsetConverter = new ValueConverter<DateTimeOffset, long>(
            value => value.UtcDateTime.Ticks,
            ticks => new DateTimeOffset(new DateTime(ticks, DateTimeKind.Utc)));

        var nullableDateTimeOffsetConverter = new ValueConverter<DateTimeOffset?, long?>(
            value => value.HasValue ? value.Value.UtcDateTime.Ticks : null,
            ticks => ticks.HasValue ? new DateTimeOffset(new DateTime(ticks.Value, DateTimeKind.Utc)) : null);

        var dateTimeOffsetComparer = new ValueComparer<DateTimeOffset>(
            (left, right) => left.UtcDateTime == right.UtcDateTime,
            value => value.UtcDateTime.GetHashCode(),
            value => new DateTimeOffset(value.UtcDateTime));

        var nullableDateTimeOffsetComparer = new ValueComparer<DateTimeOffset?>(
            (left, right) => left.GetValueOrDefault().UtcDateTime == right.GetValueOrDefault().UtcDateTime,
            value => value.HasValue ? value.Value.UtcDateTime.GetHashCode() : 0,
            value => value.HasValue ? new DateTimeOffset(value.Value.UtcDateTime) : (DateTimeOffset?)null);

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                var columnType = property.GetColumnType();
                if (string.IsNullOrWhiteSpace(columnType))
                {
                    continue;
                }

                if (columnType.Equals("nvarchar(max)", StringComparison.OrdinalIgnoreCase) ||
                    columnType.StartsWith("nvarchar(", StringComparison.OrdinalIgnoreCase))
                {
                    property.SetColumnType("TEXT");
                }

                if (property.ClrType == typeof(DateTimeOffset))
                {
                    property.SetValueConverter(dateTimeOffsetConverter);
                    property.SetValueComparer(dateTimeOffsetComparer);
                    property.SetColumnType("INTEGER");
                }
                else if (property.ClrType == typeof(DateTimeOffset?))
                {
                    property.SetValueConverter(nullableDateTimeOffsetConverter);
                    property.SetValueComparer(nullableDateTimeOffsetComparer);
                    property.SetColumnType("INTEGER");
                }
            }
        }
    }
}
