using Microsoft.EntityFrameworkCore;
using Surefire.Domain.Policies.Models;
using Surefire.Domain.Renewals.Models;
namespace Surefire.Data;

public partial class ApplicationDbContext
{
    partial void ConfigurePolicyRelationships(ModelBuilder modelBuilder)
    {
        // Policy        | 
        //---------------|----------------------------------------------------
        modelBuilder.Entity<Policy>()
            .HasOne(p => p.Application)
            .WithMany()
            .HasForeignKey("ApplicationId");

        modelBuilder.Entity<Policy>()
            .HasOne(p => p.Carrier)
            .WithMany()
            .HasForeignKey("CarrierId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Policy>()
            .HasOne(p => p.Wholesaler)
            .WithMany()
            .HasForeignKey("WholesalerId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Policy>()
            .HasOne(p => p.Product)
            .WithMany()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Policy>()
            .HasOne(p => p.CreatedBy)
            .WithMany()
            .HasForeignKey(p => p.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Policy>()
            .HasOne(c => c.CSR)
            .WithMany()
            .HasForeignKey(p => p.CSRId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Policy>()
            .HasOne(p => p.Producer)
            .WithMany()
            .HasForeignKey(p => p.ProducerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Policy>()
            .Property(p => p.Premium)
            .HasColumnType("decimal(18,2)");


        // Policy        | Coverage Details - General Liability
        //---------------|----------------------------------------------------
        modelBuilder.Entity<Policy>()
            .HasOne(p => p.GeneralLiabilityCoverage)
            .WithOne(g => g.Policy)
            .HasForeignKey<GeneralLiabilityCoverage>(g => g.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<GeneralLiabilityCoverage>()
            .HasOne(g => g.Client)
            .WithMany()
            .HasForeignKey(g => g.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<GeneralLiabilityCoverage>()
            .HasOne(g => g.CreatedBy)
            .WithMany()
            .HasForeignKey("CreatedById")
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<GeneralLiabilityCoverage>()
            .HasOne(g => g.ModifiedBy)
            .WithMany()
            .HasForeignKey("ModifiedById")
            .OnDelete(DeleteBehavior.Restrict);

        // Policy        | Coverage Details - Commercial Auto
        //---------------|----------------------------------------------------
        modelBuilder.Entity<Policy>()
            .HasOne(p => p.AutoCoverage)
            .WithOne(g => g.Policy)
            .HasForeignKey<AutoCoverage>(g => g.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AutoCoverage>()
            .HasOne(a => a.Client)
            .WithMany()
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AutoCoverage>()
            .HasOne(a => a.CreatedBy)
            .WithMany()
            .HasForeignKey("CreatedById")
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AutoCoverage>()
            .HasOne(a => a.ModifiedBy)
            .WithMany()
            .HasForeignKey("ModifiedById")
            .OnDelete(DeleteBehavior.Restrict);

        // Policy        | Coverage Details - Worker's Compensation
        //---------------|----------------------------------------------------
        modelBuilder.Entity<Policy>()
            .HasOne(p => p.WorkCompCoverage)
            .WithOne(g => g.Policy)
            .HasForeignKey<WorkCompCoverage>(g => g.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<WorkCompCoverage>()
            .HasOne(wc => wc.Policy)
            .WithOne(p => p.WorkCompCoverage)
            .HasForeignKey<WorkCompCoverage>(wc => wc.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<WorkCompCoverage>()
            .HasOne(w => w.Client)
            .WithMany()
            .HasForeignKey(w => w.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<WorkCompCoverage>()
            .HasOne(w => w.CreatedBy)
            .WithMany()
            .HasForeignKey("CreatedById")
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<WorkCompCoverage>()
            .HasOne(w => w.ModifiedBy)
            .WithMany()
            .HasForeignKey("ModifiedById")
            .OnDelete(DeleteBehavior.Restrict);

        // Policy        | Coverage Details - Umbrella
        //---------------|----------------------------------------------------
        modelBuilder.Entity<Policy>()
            .HasOne(p => p.UmbrellaCoverage)
            .WithOne(g => g.Policy)
            .HasForeignKey<UmbrellaCoverage>(g => g.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UmbrellaCoverage>()
            .HasOne(w => w.Client)
            .WithMany()
            .HasForeignKey(w => w.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UmbrellaCoverage>()
            .HasOne(w => w.CreatedBy)
            .WithMany()
            .HasForeignKey("CreatedById")
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UmbrellaCoverage>()
            .HasOne(w => w.ModifiedBy)
            .WithMany()
            .HasForeignKey("ModifiedById")
            .OnDelete(DeleteBehavior.Restrict);

        // Policy        | Coverage Details - Property
        //---------------|----------------------------------------------------
        modelBuilder.Entity<Policy>()
            .HasOne(p => p.PropertyCoverage)
            .WithOne(g => g.Policy)
            .HasForeignKey<PropertyCoverage>(g => g.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PropertyCoverage>()
            .HasOne(w => w.Client)
            .WithMany()
            .HasForeignKey(w => w.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PropertyCoverage>()
            .HasOne(w => w.CreatedBy)
            .WithMany()
            .HasForeignKey("CreatedById")
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PropertyCoverage>()
            .HasOne(w => w.ModifiedBy)
            .WithMany()
            .HasForeignKey("ModifiedById")
            .OnDelete(DeleteBehavior.Restrict);

        // Policy        | Rating Basis
        //---------------|----------------------------------------------------
        modelBuilder.Entity<RatingBasis>()
            .HasOne(r => r.Policy)
            .WithMany(p => p.RatingBases)
            .HasForeignKey(r => r.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RatingBasis>()
            .HasOne(r => r.UserModified)
            .WithMany()
            .HasForeignKey("UserModifiedId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RatingBasis>()
            .HasOne(r => r.Product)
            .WithMany()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RatingBasis>()
            .HasOne(r => r.Location)
            .WithMany()
            .HasForeignKey("LocationId")
            .OnDelete(DeleteBehavior.Restrict);


        //Renewal

        modelBuilder.Entity<Renewal>()
            .HasOne(r => r.Carrier)
            .WithMany()
            .HasForeignKey("CarrierId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Renewal>()
            .HasOne(r => r.Wholesaler)
            .WithMany()
            .HasForeignKey("WholesalerId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Renewal>()
            .HasOne(r => r.AssignedTo)
            .WithMany()
            .HasForeignKey("AssignedToId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Renewal>()
            .HasOne(r => r.Product)
            .WithMany()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Renewal>()
            .HasOne(r => r.Policy)
            .WithMany(p => p.Renewals)
            .HasForeignKey("PolicyId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Renewal>()
            .Property(c => c.ExpiringPremium)
            .HasColumnType("decimal(18,2)");

        //Submission

        modelBuilder.Entity<Submission>()
            .HasOne(s => s.Product)
            .WithMany()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Submission>()
            .HasOne(s => s.Carrier)
            .WithMany()
            .HasForeignKey("CarrierId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Submission>()
            .HasOne(s => s.Wholesaler)
            .WithMany()
            .HasForeignKey("WholesalerId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Submission>()
            .HasOne(s => s.Renewal)
            .WithMany(r => r.Submissions)
            .HasForeignKey("RenewalId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Submission>()
            .HasOne(s => s.Lead)
            .WithMany(l => l.Submissions)  // Assuming Lead has a collection of Submissions
            .HasForeignKey(s => s.LeadId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Submission>()
            .HasMany(s => s.SubmissionNotes)
            .WithOne(sn => sn.Submission)
            .HasForeignKey(sn => sn.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}