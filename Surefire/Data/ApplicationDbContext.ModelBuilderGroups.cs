using Microsoft.EntityFrameworkCore;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Carriers.Models;
namespace Surefire.Data;

public partial class ApplicationDbContext
{
    partial void ConfigureGroupRelationships(ModelBuilder modelBuilder)
    {
        //Client
        modelBuilder.Entity<Client>()
            .HasOne(c => c.Address)
            .WithMany()
            .HasForeignKey("AddressId");

        modelBuilder.Entity<Client>()
            .HasOne(c => c.PrimaryContact)
            .WithMany()
            .HasForeignKey("PrimaryContactId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Client>()
            .HasOne(c => c.Producer)
            .WithMany()
            .HasForeignKey("ProducerId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Client>()
            .HasOne(c => c.CSR)
            .WithMany()
            .HasForeignKey("CSRId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Client>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .HasForeignKey("CreatedById")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Client>()
            .HasMany(s => s.ClientNotes)
            .WithOne(sn => sn.Client)
            .HasForeignKey(sn => sn.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        //Client - Business Details
        modelBuilder.Entity<BusinessDetails>()
            .Property(b => b.LegalEntityType)
            .HasConversion<int>();

        modelBuilder.Entity<BusinessDetails>()
            .Property(b => b.BusinessType)
            .HasConversion<int>();

        modelBuilder.Entity<BusinessDetails>()
            .Property(b => b.LicenseType)
            .HasConversion<int?>();

        //Lead (Potential Client)
        modelBuilder.Entity<Lead>()
            .HasOne(l => l.Product)
            .WithMany()
            .HasForeignKey(l => l.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Lead>()
            .HasOne(l => l.Product)
            .WithMany()
            .HasForeignKey(l => l.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Lead>()
            .HasOne(l => l.Client)
            .WithMany()
            .HasForeignKey(l => l.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Lead>()
            .HasOne(l => l.CreatedBy)
            .WithMany()
            .HasForeignKey("CreatedById")
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Lead>()
            .HasMany(s => s.LeadNotes)
            .WithOne(sn => sn.Lead)
            .HasForeignKey(sn => sn.LeadId)
            .OnDelete(DeleteBehavior.Cascade);

        //Carrier
        modelBuilder.Entity<Carrier>()
            .HasOne(c => c.Address)
            .WithMany()
            .HasForeignKey("AddressId");

        modelBuilder.Entity<Carrier>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .HasForeignKey("CreatedById")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Credential>()
            .HasOne(cred => cred.Carrier)
            .WithMany(carrier => carrier.Credentials)
            .HasForeignKey(cred => cred.CarrierId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
