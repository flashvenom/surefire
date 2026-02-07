using Microsoft.EntityFrameworkCore;
using Quickfire.Blazor.Domain.Clients.Models;
using Quickfire.Blazor.Domain.Carriers.Models;
using Quickfire.Blazor.Domain.Contacts.Models;
using Quickfire.Blazor.Domain.Policies.Models;
using Quickfire.Blazor.Domain.Renewals.Models;
using Quickfire.Blazor.Domain.Shared.Models;
using Quickfire.Blazor.Domain.Forms.Models;
using Quickfire.Blazor.Domain.Home.Models;
using Quickfire.Blazor.Domain.Logs;
using System.ComponentModel.DataAnnotations.Schema;
using Quickfire.Blazor.Domain.Attachments.Models;
namespace Quickfire.Blazor.Data;

public partial class ApplicationDbContext
{
    // Primary entities
    public DbSet<Client> Clients { get; set; }
    public DbSet<ClientNote> ClientNotes { get; set; }
    public DbSet<BusinessDetails> BusinessDetails { get; set; }
    public DbSet<Carrier> Carriers { get; set; }
    public DbSet<Credential> Credentials { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<PhoneNumber> PhoneNumbers { get; set; }
    public DbSet<EmailAddress> EmailAddresses { get; set; }
    public DbSet<Quickfire.Blazor.Domain.Shared.Models.Location> Locations { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Address> Address { get; set; }
    public DbSet<Lead> Leads { get; set; }
    public DbSet<LeadNote> LeadNotes { get; set; }
    public DbSet<GlobalNote> GlobalNotes { get; set; }

    // Renewals and Tasks
    public DbSet<Renewal> Renewals { get; set; }
    public DbSet<RenewalUpdate> RenewalUpdates { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<SubmissionTask> SubmissionTasks { get; set; }
    public DbSet<RenewalNote> RenewalNotes { get; set; }
    public DbSet<TrackTask> TrackTasks { get; set; }
    public DbSet<DailyTask> DailyTasks { get; set; }
    public DbSet<TaskMaster> TaskMasters { get; set; } = default!;
    public DbSet<TaskGroup> TaskGroups { get; set; } = default!;
    public DbSet<TaskGroupTaskMaster> TaskGroupTaskMasters { get; set; } = default!;
    public DbSet<TaskMasterSubTask> TaskMasterSubTasks { get; set; }


    // Policies
    public DbSet<Policy> Policies { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<GeneralLiabilityCoverage> GeneralLiabilityCoverages { get; set; }
    public DbSet<AutoCoverage> AutoCoverages { get; set; }
    public DbSet<WorkCompCoverage> WorkCompCoverages { get; set; }
    public DbSet<UmbrellaCoverage> UmbrellaCoverage { get; set; }
    public DbSet<PropertyCoverage> PropertyCoverage { get; set; }
    public DbSet<BusinessOwnersPolicyCoverage> BusinessOwnersPolicyCoverages { get; set; }
    public DbSet<RatingBasis> RatingBases { get; set; }
    public DbSet<WorkCompRatingBasis> WorkCompRatingBases { get; set; }

    //Forms & Docs
    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<CertificateRequest> CertificateRequests { get; set; }
    public DbSet<FormDoc> FormDocs { get; set; }
    public DbSet<FormDocRevision> FormDocRevisions { get; set; }
    public DbSet<FormPdf> FormPdf { get; set; }
    public DbSet<FormsLibraryEntry> FormsLibrary { get; set; }
    public DbSet<FormsLibraryVersion> FormsLibraryVersions { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<AttachmentGroup> AttachmentGroups { get; set; }
    public DbSet<EmailTemplate> EmailTemplates { get; set; }

    //Other
    public DbSet<Log> Logs { get; set; }
    public DbSet<Settings> Settings { get; set; }
    public DbSet<EntityAssociation> EntityAssociations { get; set; }
    public DbSet<Quickfire.Blazor.Domain.CompanyManual.Models.CompanyManualPage> CompanyManualPages { get; set; }
    public DbSet<Quickfire.Blazor.Domain.CompanyManual.Models.CompanyManualRevision> CompanyManualRevisions { get; set; }
    public DbSet<Quickfire.Blazor.Domain.CompanyManual.Models.CompanyManualSuggestion> CompanyManualSuggestions { get; set; }
    public DbSet<Quickfire.Blazor.Domain.CompanyManual.Models.CompanyManualAuditEntry> CompanyManualAuditEntries { get; set; }

    //Unmapped
    [NotMapped]
    public DbSet<FireSearchResultViewModel> FireSearchResultViewModel { get; set; }
    public DbSet<MasterSubTask> MasterSubTasks { get; set; }
    public DbSet<WholesalerCarrier> WholesalerCarriers { get; set; }
    public DbSet<CarrierProduct> CarrierProducts { get; set; }

}
