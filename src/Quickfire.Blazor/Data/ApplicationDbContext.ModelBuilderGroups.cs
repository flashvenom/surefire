using Microsoft.EntityFrameworkCore;
using Quickfire.Blazor.Domain.Clients.Models;
using Quickfire.Blazor.Domain.Carriers.Models;
using Quickfire.Blazor.Domain.Forms.Models;
namespace Quickfire.Blazor.Data;

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

        modelBuilder.Entity<Client>()
            .HasOne(client => client.BusinessDetails)
            .WithOne(details => details.Client)
            .HasForeignKey<BusinessDetails>(details => details.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

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

        modelBuilder.Entity<CertificateRequest>(entity =>
        {
            entity.HasKey(e => e.CertificateRequestId);

            entity.HasOne(e => e.Client)
                .WithMany()
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AssignedTo)
                .WithMany()
                .HasForeignKey(e => e.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Certificate)
                .WithMany()
                .HasForeignKey(e => e.CertificateId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // WholesalerCarrier many-to-many (junction) relationship
        modelBuilder.Entity<WholesalerCarrier>()
            .HasOne(wc => wc.Wholesaler)
            .WithMany(c => c.WholesalerAccess)
            .HasForeignKey(wc => wc.WholesalerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WholesalerCarrier>()
            .HasOne(wc => wc.Carrier)
            .WithMany()
            .HasForeignKey(wc => wc.CarrierId)
            .OnDelete(DeleteBehavior.Restrict);

        // CarrierProduct many-to-many (junction) relationship
        modelBuilder.Entity<CarrierProduct>()
            .HasOne(cp => cp.Carrier)
            .WithMany(c => c.CarrierProducts)
            .HasForeignKey(cp => cp.CarrierId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CarrierProduct>()
            .HasOne(cp => cp.Product)
            .WithMany(p => p.CarrierProducts)
            .HasForeignKey(cp => cp.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
