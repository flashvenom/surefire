using Microsoft.EntityFrameworkCore;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Carriers.Models;
using Mantis.Domain.Contacts.Models;
using Mantis.Domain.Policies.Models;
using Mantis.Domain.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Mantis.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Carrier> Carriers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Renewal> Renewals { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
                .Property(p => p.Premium)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Claim>()
                .Property(c => c.AmountPaid)
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

            modelBuilder.Entity<Location>()
                .HasOne(l => l.Address)
                .WithMany()
                .HasForeignKey("AddressId");
        }
    }
}
