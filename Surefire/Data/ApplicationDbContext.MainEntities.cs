using Microsoft.EntityFrameworkCore;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Carriers.Models;
using Surefire.Domain.Contacts.Models;
using Surefire.Domain.Policies.Models;
using Surefire.Domain.Renewals.Models;
using Surefire.Domain.Shared.Models;
using Surefire.Domain.Forms.Models;
using Surefire.Domain.Plugins;
using Surefire.Domain.OpenAI;
using Surefire.Domain.Logs;
using System.ComponentModel.DataAnnotations.Schema;
using Surefire.Domain.Attachments.Models;
using Surefire.Domain.Accounting.Models;
namespace Surefire.Data;

public partial class ApplicationDbContext
{
    // Primary entities
    public DbSet<Client> Clients { get; set; }
    public DbSet<ClientNote> ClientNotes { get; set; }
    public DbSet<BusinessDetails> BusinessDetails { get; set; }
    public DbSet<Carrier> Carriers { get; set; }
    public DbSet<Credential> Credentials { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Address> Address { get; set; }
    public DbSet<Lead> Leads { get; set; }
    public DbSet<LeadNote> LeadNotes { get; set; }


    // Renewals and Tasks
    public DbSet<Renewal> Renewals { get; set; }
    public DbSet<TaskMaster> TaskMasters { get; set; }
    public DbSet<TrackTask> TrackTasks { get; set; }
    public DbSet<DailyTask> DailyTasks { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<SubmissionNote> SubmissionNotes { get; set; }


    // Policies
    public DbSet<Policy> Policies { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<GeneralLiabilityCoverage> GeneralLiabilityCoverages { get; set; }
    public DbSet<AutoCoverage> AutoCoverages { get; set; }
    public DbSet<WorkCompCoverage> WorkCompCoverages { get; set; }
    public DbSet<UmbrellaCoverage> UmbrellaCoverage { get; set; }
    public DbSet<PropertyCoverage> PropertyCoverage { get; set; }
    public DbSet<RatingBasis> RatingBases { get; set; }
    public DbSet<Loss> Losses { get; set; }
    public DbSet<Claim> Claims { get; set; }

    //Forms & Docs
    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<FormDoc> FormDocs { get; set; }
    public DbSet<FormDocRevision> FormDocRevisions { get; set; }
    public DbSet<FormPdf> FormPdf { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<AttachmentGroup> AttachmentGroups { get; set; }

    //Accounting
    public DbSet<Settlement> Settlements { get; set; }
    public DbSet<SettlementItem> SettlementItems { get; set; }


    //Other
    public DbSet<Log> Logs { get; set; }
    public DbSet<Plugin> Plugins { get; set; }
    public DbSet<OpenAIPrompt> OpenAIPrompt { get; set; }
    public DbSet<OpenAIResponse> OpenAIResponse { get; set; }
    public DbSet<Settings> Settings { get; set; }


    //Unmapped
    [NotMapped]
    public DbSet<FireSearchResultViewModel> FireSearchResultViewModel { get; set; }
}