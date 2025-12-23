using Microsoft.EntityFrameworkCore;
using Quickfire.Blazor.Domain.Policies.Models;
using Quickfire.Blazor.Domain.Renewals.Models;
namespace Quickfire.Blazor.Data;

public partial class ApplicationDbContext
{
    partial void ConfigurePolicyRelationships(ModelBuilder modelBuilder)
    {
        // Policy        | 
        //---------------|----------------------------------------------------
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

        // Policy        | Coverage Details - Business Owners Policy
        //---------------|----------------------------------------------------
        modelBuilder.Entity<Policy>()
            .HasOne(p => p.BusinessOwnersPolicyCoverage)
            .WithOne(b => b.Policy)
            .HasForeignKey<BusinessOwnersPolicyCoverage>(b => b.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<BusinessOwnersPolicyCoverage>()
            .HasOne(b => b.Client)
            .WithMany()
            .HasForeignKey(b => b.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<BusinessOwnersPolicyCoverage>()
            .HasOne(b => b.CreatedBy)
            .WithMany()
            .HasForeignKey("CreatedById")
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<BusinessOwnersPolicyCoverage>()
            .HasOne(b => b.ModifiedBy)
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

        modelBuilder.Entity<SubmissionTask>()
            .HasOne(t => t.Submission)
            .WithMany(s => s.SubmissionTasks)
            .HasForeignKey(t => t.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade); // or Restrict/SetNull depending on logic

        // RenewalUpdate
        modelBuilder.Entity<RenewalUpdate>()
            .HasIndex(r => r.RenewalHashId)
            .IsUnique(false);

        modelBuilder.Entity<RenewalUpdate>()
            .HasOne(ru => ru.Client)
            .WithMany()
            .HasForeignKey(ru => ru.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RenewalUpdate>()
            .HasOne(ru => ru.Renewal)
            .WithMany()
            .HasForeignKey(ru => ru.RenewalId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RenewalUpdate>()
            .HasOne(ru => ru.Policy)
            .WithMany()
            .HasForeignKey(ru => ru.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RenewalUpdate>()
            .HasOne(ru => ru.Product)
            .WithMany()
            .HasForeignKey(ru => ru.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Defaults for include flags
        modelBuilder.Entity<RenewalUpdate>()
            .Property(r => r.IncludeGeneralLiability)
            .HasDefaultValue(false);
        modelBuilder.Entity<RenewalUpdate>()
            .Property(r => r.IncludeWorkComp)
            .HasDefaultValue(false);
        modelBuilder.Entity<RenewalUpdate>()
            .Property(r => r.IncludeCommercialAuto)
            .HasDefaultValue(false);
    }
}
