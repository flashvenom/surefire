using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace Surefire.Data;

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
    }

    // Partial method declarations for modular OnModelCreating logic
    partial void ConfigureMainEntities(ModelBuilder modelBuilder);
    partial void ConfigureColumnProperties(ModelBuilder modelBuilder);
    partial void ConfigurePolicyRelationships(ModelBuilder modelBuilder);
    partial void ConfigureGroupRelationships(ModelBuilder modelBuilder);
    partial void ConfigureCommonRelationships(ModelBuilder modelBuilder);
}
