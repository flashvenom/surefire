using Microsoft.EntityFrameworkCore;
using Quickfire.Blazor.Domain.Contacts.Models;
using Quickfire.Blazor.Domain.CompanyManual.Models;
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

        // Company Manual
        //-------------------------------------------------------------------
        modelBuilder.Entity<CompanyManualPage>()
            .HasOne(p => p.ParentPage)
            .WithMany(p => p.ChildPages)
            .HasForeignKey(p => p.ParentPageId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualPage>()
            .HasOne(p => p.PublishedRevision)
            .WithMany()
            .HasForeignKey(p => p.PublishedRevisionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualPage>()
            .HasOne(p => p.OwnerUser)
            .WithMany()
            .HasForeignKey(p => p.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualPage>()
            .HasOne(p => p.CreatedBy)
            .WithMany()
            .HasForeignKey(p => p.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualPage>()
            .HasOne(p => p.UpdatedBy)
            .WithMany()
            .HasForeignKey(p => p.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualPage>()
            .HasOne(p => p.ArchivedBy)
            .WithMany()
            .HasForeignKey(p => p.ArchivedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualPage>()
            .HasOne(p => p.TaskGroup)
            .WithMany()
            .HasForeignKey(p => p.TaskGroupId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<CompanyManualRevision>()
            .HasOne(r => r.Page)
            .WithMany(p => p.Revisions)
            .HasForeignKey(r => r.CompanyManualPageId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CompanyManualRevision>()
            .HasOne(r => r.CreatedBy)
            .WithMany()
            .HasForeignKey(r => r.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualRevision>()
            .HasOne(r => r.PublishedBy)
            .WithMany()
            .HasForeignKey(r => r.PublishedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualRevision>()
            .HasOne(r => r.SourceSuggestion)
            .WithMany()
            .HasForeignKey(r => r.SourceSuggestionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualSuggestion>()
            .HasOne(s => s.Page)
            .WithMany(p => p.Suggestions)
            .HasForeignKey(s => s.CompanyManualPageId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CompanyManualSuggestion>()
            .HasOne(s => s.SubmittedBy)
            .WithMany()
            .HasForeignKey(s => s.SubmittedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualSuggestion>()
            .HasOne(s => s.ReviewedBy)
            .WithMany()
            .HasForeignKey(s => s.ReviewedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualSuggestion>()
            .HasOne(s => s.BasedOnRevision)
            .WithMany()
            .HasForeignKey(s => s.BasedOnRevisionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualSuggestion>()
            .HasOne(s => s.OutcomeRevision)
            .WithMany()
            .HasForeignKey(s => s.OutcomeRevisionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualAuditEntry>()
            .HasOne(a => a.Page)
            .WithMany()
            .HasForeignKey(a => a.CompanyManualPageId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualAuditEntry>()
            .HasOne(a => a.Revision)
            .WithMany()
            .HasForeignKey(a => a.CompanyManualRevisionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualAuditEntry>()
            .HasOne(a => a.Suggestion)
            .WithMany()
            .HasForeignKey(a => a.CompanyManualSuggestionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualAuditEntry>()
            .HasOne(a => a.ActorUser)
            .WithMany()
            .HasForeignKey(a => a.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyManualPage>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        modelBuilder.Entity<CompanyManualPage>()
            .HasIndex(p => p.ParentPageId);

        modelBuilder.Entity<CompanyManualSuggestion>()
            .Property(s => s.Status)
            .HasConversion<string>();

        modelBuilder.Entity<CompanyManualSuggestion>()
            .HasIndex(s => s.Status);

        modelBuilder.Entity<CompanyManualAuditEntry>()
            .HasIndex(a => a.CompanyManualPageId);


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
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        modelBuilder.Entity<FormDoc>()
            .HasOne(fd => fd.FormsLibraryVersion)
            .WithMany()
            .HasForeignKey(fd => fd.FormsLibraryVersionId)
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

        // Forms | Forms Library
        //---------|----------------------------------------------------
        modelBuilder.Entity<FormsLibraryEntry>()
            .ToTable("FormsLibrary");

        modelBuilder.Entity<FormsLibraryEntry>()
            .HasOne(e => e.ActiveVersion)
            .WithMany()
            .HasForeignKey(e => e.ActiveVersionId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<FormsLibraryEntry>()
            .HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FormsLibraryEntry>()
            .HasOne(e => e.ModifiedBy)
            .WithMany()
            .HasForeignKey(e => e.ModifiedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FormsLibraryVersion>()
            .ToTable("FormsLibraryVersions");

        modelBuilder.Entity<FormsLibraryVersion>()
            .HasOne(v => v.Entry)
            .WithMany(e => e.Versions)
            .HasForeignKey(v => v.FormsLibraryEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FormsLibraryVersion>()
            .HasOne(v => v.UploadedBy)
            .WithMany()
            .HasForeignKey(v => v.UploadedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FormsLibraryEntry>()
            .HasIndex(e => e.Title);

        modelBuilder.Entity<FormsLibraryEntry>()
            .HasIndex(e => e.CarrierName);

        modelBuilder.Entity<FormsLibraryEntry>()
            .HasIndex(e => e.WholesalerName);

        modelBuilder.Entity<FormsLibraryEntry>()
            .HasIndex(e => e.MarketTag);

        modelBuilder.Entity<FormsLibraryEntry>()
            .HasIndex(e => e.IsBookmarked);

        modelBuilder.Entity<FormsLibraryVersion>()
            .HasIndex(v => new { v.FormsLibraryEntryId, v.VersionNumber })
            .IsUnique();

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
