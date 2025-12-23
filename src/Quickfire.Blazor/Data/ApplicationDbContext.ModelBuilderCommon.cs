using Microsoft.EntityFrameworkCore;
using Quickfire.Blazor.Domain.Contacts.Models;
using Quickfire.Blazor.Domain.Renewals.Models;
using Quickfire.Blazor.Domain.Forms.Models;
using Quickfire.Blazor.Domain.Attachments.Models;
using Quickfire.Blazor.Domain.Home.Models;
namespace Quickfire.Blazor.Data;

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

        modelBuilder.Entity<Quickfire.Blazor.Domain.Shared.Models.Location>()
            .HasOne(l => l.Address)
            .WithMany()
            .HasForeignKey("AddressId");

        modelBuilder.Entity<Contact>()
            .HasOne(c => c.PrimaryPhone)
            .WithMany()
            .HasForeignKey(c => c.PrimaryPhoneId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Contact>()
            .HasOne(c => c.PrimaryEmail)
            .WithMany()
            .HasForeignKey(c => c.PrimaryEmailId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Contact>()
            .HasMany(c => c.PhoneNumbers)
            .WithOne(p => p.Contact)
            .HasForeignKey(p => p.ContactId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Contact>()
            .HasMany(c => c.EmailAddresses)
            .WithOne(e => e.Contact)
            .HasForeignKey(e => e.ContactId)
            .OnDelete(DeleteBehavior.Restrict);

        // Tasks          
        //-------------------------------------------------------------------
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

        modelBuilder.Entity<TrackTask>()
            .HasOne(t => t.ParentTask)
            .WithMany(t => t.Subtasks)
            .HasForeignKey("ParentTaskId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DailyTask>()
            .HasOne(d => d.AssignedTo)
            .WithMany()
            .HasForeignKey("AssignedToId")
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<TaskMaster>()
            .HasKey(t => t.TaskMasterId);

        modelBuilder.Entity<TaskGroupTaskMaster>()
            .HasKey(tgtm => new { tgtm.TaskGroupId, tgtm.TaskMasterId });

        modelBuilder.Entity<TaskGroupTaskMaster>()
            .HasOne(tgtm => tgtm.TaskGroup)
            .WithMany(tg => tg.TaskGroupTaskMasters)
            .HasForeignKey(tgtm => tgtm.TaskGroupId);

        modelBuilder.Entity<TaskGroupTaskMaster>()
            .HasOne(tgtm => tgtm.TaskMaster)
            .WithMany(tm => tm.TaskGroupTaskMasters)
            .HasForeignKey(tgtm => tgtm.TaskMasterId);

        modelBuilder.Entity<TaskMasterSubTask>()
            .HasKey(tmst => new { tmst.ParentTaskMasterId, tmst.SubTaskMasterId });

        modelBuilder.Entity<TaskMasterSubTask>()
            .HasOne(tmst => tmst.ParentTaskMaster)
            .WithMany(tm => tm.SubTaskLinks)
            .HasForeignKey(tmst => tmst.ParentTaskMasterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskMasterSubTask>()
            .HasOne(tmst => tmst.SubTaskMaster)
            .WithMany(tm => tm.ParentLinks)
            .HasForeignKey(tmst => tmst.SubTaskMasterId)
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

        modelBuilder.Entity<FormDoc>()
            .HasOne(fd => fd.Policy)
            .WithMany(p => p.FormDocs)  // Policy has a collection of FormDocs
            .HasForeignKey(fd => fd.PolicyId)
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


        // RenewalNote
        //-------------------------------------------------------------------
        modelBuilder.Entity<RenewalNote>()
            .HasOne(rn => rn.TrackTask)
            .WithMany()
            .HasForeignKey(rn => rn.TrackTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RenewalNote>()
            .HasIndex(rn => rn.TrackTaskId);



    }
}
