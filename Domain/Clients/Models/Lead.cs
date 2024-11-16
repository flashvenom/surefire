using Mantis.Data;
using Mantis.Domain.Forms.Models;
using Mantis.Domain.Shared;
using Mantis.Domain.Renewals.Models;

namespace Mantis.Domain.Clients.Models
{
    public class Lead
    {
        public int LeadId { get; set; }

        //Form Data
        public string? CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? Title { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Operations { get; set; }
        public string? Notes { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public int? Stage { get; set; }
        public int? Source { get; set; }
        public DateTime? BindDate { get; set; }

        //Application Use Only
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastOpened { get; set; }
        public ApplicationUser? CreatedBy { get; set; }

        //Navigation
        public ICollection<FormDoc> FormDocs { get; set; } = new List<FormDoc>();
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public List<LeadNote> LeadNotes { get; set; } = new List<LeadNote>();
        public Client? Client { get; set; }
        public int? ClientId { get; set; }
        public Product? Product { get; set; }
        public int? ProductId { get; set; }
    }

    public class LeadNote
    {
        public int LeadNoteId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public string Note { get; set; }
        public Lead Lead { get; set; }
        public int LeadId { get; set; }
        public bool Deleted { get; set; }
    }
    public enum LeadStage
    {
        New,
        Active,
        Holding,
        Tickler,
        Archive,
        Trash,
        Converted
    }

    public enum Source
    {
        Internal,
        Referral,
        Website,
        Google,
        SocialMedia,
        Walkin,
        Other
    }
}
