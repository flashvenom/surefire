using Microsoft.EntityFrameworkCore;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Carriers.Models;
using Mantis.Domain.Contacts.Models;
using Mantis.Domain.Policies.Models;
using Mantis.Domain.Renewals.Models;
using Mantis.Domain.Forms.Models;
using Mantis.Domain.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Mantis.Domain.Shared.Models;

namespace Mantis.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Primary entities
        public DbSet<Client> Clients { get; set; }
        public DbSet<Carrier> Carriers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Certificate> Certificates { get; set; }

        // Renewals and Tasks
        public DbSet<TaskMaster> TaskMasters { get; set; }
        public DbSet<TrackTask> TrackTasks { get; set; }
        public DbSet<DailyTask> DailyTasks { get; set; }
        public DbSet<Renewal> Renewals { get; set; }
        public DbSet<Submission> Submissions { get; set; }

        // Policies
        public DbSet<Policy> Policies { get; set; }
        public DbSet<GeneralLiabilityCoverage> GeneralLiabilityCoverages { get; set; }
        public DbSet<AutoCoverage> AutoCoverages { get; set; }
        public DbSet<WorkCompCoverage> WorkCompCoverages { get; set; }
        public DbSet<RatingBasis> RatingBases { get; set; }
        public DbSet<Loss> Losses { get; set; }
        public DbSet<Claim> Claims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for specific entities
            modelBuilder.Entity<GeneralLiabilityCoverage>()
                .Property(g => g.Premium)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Loss>()
                .Property(l => l.AmountPaid)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Loss>()
                .Property(l => l.AmountReserved)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<RatingBasis>()
                .Property(r => r.BaseRate)
                .HasColumnType("decimal(7,4)");

            modelBuilder.Entity<RatingBasis>()
                .Property(r => r.NetRate)
                .HasColumnType("decimal(7,4)");

            modelBuilder.Entity<RatingBasis>()
                .Property(r => r.Payroll)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<RatingBasis>()
                .Property(r => r.Premium)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Location>()
                .Property(l => l.GrossSales)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Claim>()
                .Property(c => c.AmountPaid)
                .HasColumnType("decimal(18,2)");

            //Relationships
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

            modelBuilder.Entity<Carrier>()
                .HasOne(c => c.Address)
                .WithMany()
                .HasForeignKey("AddressId");

            modelBuilder.Entity<Carrier>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey("CreatedById")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Address)
                .WithMany()
                .HasForeignKey("AddressId");

            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Client)
                .WithMany(cl => cl.Contacts)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Carrier)
                .WithMany(cr => cr.Contacts)
                .HasForeignKey(c => c.CarrierId)
                .OnDelete(DeleteBehavior.SetNull);

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
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey("CreatedById")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Policy>()
                .HasOne(c => c.CSR)
                .WithMany()
                .HasForeignKey("CSRId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Policy>()
                .HasOne(c => c.Producer)
                .WithMany()
                .HasForeignKey("ProducerId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Policy>()
                .Property(p => p.Premium)
                .HasColumnType("decimal(18,2)");

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

            modelBuilder.Entity<Location>()
                .HasOne(l => l.Address)
                .WithMany()
                .HasForeignKey("AddressId");

            modelBuilder.Entity<Location>()
                .Property(l => l.GrossSales)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<TaskMaster>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<TaskMaster>()
                .HasOne<TaskMaster>()
                .WithMany()
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrackTask>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<TrackTask>()
                .HasOne(t => t.Renewal)
                .WithMany(r => r.TrackTasks)
                .HasForeignKey("RenewalId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrackTask>()
                .HasOne(t => t.AssignedTo)
                .WithMany()
                .HasForeignKey("AssignedToId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DailyTask>()
                .HasOne(d => d.AssignedTo)
                .WithMany()
                .HasForeignKey("AssignedToId")
                .OnDelete(DeleteBehavior.SetNull);

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

            // RatingBasis configuration
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

            // Loss configuration
            modelBuilder.Entity<Loss>()
                .HasOne(l => l.Policy)
                .WithMany(p => p.Losses)
                .HasForeignKey(l => l.PolicyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Loss>()
                .HasOne(l => l.UserModified)
                .WithMany()
                .HasForeignKey("UserModifiedId")
                .OnDelete(DeleteBehavior.Restrict);

            // AutoCoverage configuration
            modelBuilder.Entity<AutoCoverage>()
                .HasOne(a => a.Policy)
                .WithOne(p => p.AutoCoverage)
                .HasForeignKey<AutoCoverage>(a => a.PolicyId)
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

            // WorkCompCoverage configuration
            modelBuilder.Entity<WorkCompCoverage>()
                .HasOne(w => w.Policy)
                .WithOne(p => p.WorkCompCoverage)
                .HasForeignKey<WorkCompCoverage>(w => w.PolicyId)
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

            //Certificate
            

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey("CreatedById")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey("ModifiedById")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
