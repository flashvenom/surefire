using Microsoft.EntityFrameworkCore;
using Surefire.Domain.Contacts.Models;
using Surefire.Domain.Policies.Models;
using Surefire.Domain.Renewals.Models;
using Surefire.Domain.Shared.Models;
using Surefire.Domain.Forms.Models;
using Surefire.Domain.Attachments.Models;
using Surefire.Domain.Accounting.Models;
namespace Surefire.Data;

public partial class ApplicationDbContext
{
    partial void ConfigureCommonRelationships(ModelBuilder modelBuilder)
    {
        // Contact
        //-------------------------------------------------------------------
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

        modelBuilder.Entity<Location>()
            .HasOne(l => l.Address)
            .WithMany()
            .HasForeignKey("AddressId");

        // Tasks          
        //-------------------------------------------------------------------

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


        // Loss
        //-------------------------------------------------------------------
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


        // Forms   | Certificate 
        //---------|----------------------------------------------------
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


        // Forms   | FormDocs 
        //---------|----------------------------------------------------
        modelBuilder.Entity<FormDoc>()
            .HasOne(fd => fd.FormPdf)
            .WithMany()
            .HasForeignKey(fd => fd.FormPdfId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FormDoc>()
            .HasOne(fd => fd.Client)
            .WithMany(c => c.FormDocs) // Assume the Client has ICollection<FormDoc>
            .HasForeignKey(fd => fd.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FormDoc>()
            .HasOne(fd => fd.CreatedBy)
            .WithMany()
            .HasForeignKey(fd => fd.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FormDoc>()
            .HasOne(fd => fd.ModifiedBy)
            .WithMany()
            .HasForeignKey(fd => fd.ModifiedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FormDoc>()
            .HasOne(fd => fd.Lead)
            .WithMany(l => l.FormDocs)  // Assuming Lead has a collection of FormDocs
            .HasForeignKey(fd => fd.LeadId)
            .OnDelete(DeleteBehavior.Restrict);

        // Forms   | FormDoc Revisions 
        //---------|----------------------------------------------------
        modelBuilder.Entity<FormDocRevision>()
            .HasOne(fdr => fdr.FormDoc)
            .WithMany(fd => fd.FormDocRevisions)
            .HasForeignKey(fdr => fdr.FormDocId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FormDocRevision>()
            .HasOne(fdr => fdr.CreatedBy)
            .WithMany()
            .HasForeignKey(fdr => fdr.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FormDocRevision>()
            .HasOne(fdr => fdr.ModifiedBy)
            .WithMany()
            .HasForeignKey(fdr => fdr.ModifiedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Forms   | FormPdfs 
        //---------|----------------------------------------------------
        modelBuilder.Entity<FormPdf>()
            .HasOne(ff => ff.CreatedBy)
            .WithMany()
            .HasForeignKey(ff => ff.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FormPdf>()
            .HasOne(ff => ff.ModifiedBy)
            .WithMany()
            .HasForeignKey(ff => ff.ModifiedById)
            .OnDelete(DeleteBehavior.Restrict);


        // Attachments   | (Main)
        //---------------|----------------------------------------------------
        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.Client)
            .WithMany(c => c.Attachments)
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.Policy)
            .WithMany(p => p.Attachments)
            .HasForeignKey(a => a.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.Renewal)
            .WithMany(r => r.Attachments)
            .HasForeignKey(a => a.RenewalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.Submission)
            .WithMany(r => r.Attachments) 
            .HasForeignKey(a => a.SubmissionId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.Carrier)
            .WithMany(c => c.Attachments)
            .HasForeignKey(a => a.CarrierId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.AttachmentGroup)
            .WithMany(ag => ag.Attachments) 
            .HasForeignKey(a => a.AttachmentGroupId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.Folder)
            .WithMany(f => f.Attachments) 
            .HasForeignKey(a => a.FolderId)
            .OnDelete(DeleteBehavior.SetNull); 

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.UploadedBy)
            .WithMany()
            .HasForeignKey("UploadedById")
            .OnDelete(DeleteBehavior.Restrict); 

        // Attachments   | Groups
        //---------------|----------------------------------------------------
        modelBuilder.Entity<AttachmentGroup>()
            .HasMany(ag => ag.Attachments)
            .WithOne(a => a.AttachmentGroup)
            .HasForeignKey(a => a.AttachmentGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Folder>()
            .HasMany(f => f.Attachments)
            .WithOne(a => a.Folder)
            .HasForeignKey(a => a.FolderId)
            .OnDelete(DeleteBehavior.Cascade);


        // Accounting    | (Main)
        //---------------|----------------------------------------------------
        modelBuilder.Entity<Settlement>()
            .HasMany(s => s.SettlementItems)
            .WithOne(si => si.Settlement)
            .HasForeignKey(si => si.SettlementId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Settlement>()
            .HasOne(s => s.Renewal)
            .WithMany(r => r.Settlements)
            .HasForeignKey(s => s.RenewalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Settlement>()
            .HasOne(s => s.Policy)
            .WithMany(p => p.Settlements)
            .HasForeignKey(s => s.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Settlement>()
            .HasOne(s => s.CreatedBy)
            .WithMany() 
            .HasForeignKey(s => s.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SettlementItem>()
            .Property(si => si.ItemCode)
            .HasConversion<string>();

        modelBuilder.Entity<Settlement>()
            .Property(s => s.BillType)
            .HasConversion<string>();

        modelBuilder.Entity<Settlement>()
            .Property(s => s.PayType)
            .HasConversion<string>();
    }
}