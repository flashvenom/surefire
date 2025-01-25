using Surefire.Domain.Clients.Models;
using Surefire.Data;
using System.Net;
using Surefire.Domain.Policies.Models;

namespace Surefire.Domain.Forms.Models
{
    public class FormDoc
    {
        public int FormDocId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? JSONData { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        //Navigation Properties
        public ICollection<FormDocRevision> FormDocRevisions { get; set; } = new List<FormDocRevision>();
        public FormPdf FormPdf { get; set; }
        public int FormPdfId { get; set; }
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public string? ModifiedById { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }
        public Client? Client { get; set; }
        public int? ClientId { get; set; }
        public Lead? Lead { get; set; }
        public int? LeadId { get; set; }
    }

    public class FormDocRevision
    {
        public int FormDocRevisionId { get; set; }
        public string? RevisionName { get; set; }
        public string? JSONData { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        //Navigation Properties
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public string? ModifiedById { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }
        public FormDoc FormDoc { get; set; }
        public int FormDocId { get; set; }

    }

    public class FormPdf
    {
        public int FormPdfId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Filepath { get; set; }
        public string? JSONFields { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public string? ModifiedById { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }
    }

    public class FormDocDTO
    {
        public int FormDocId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        // Navigation properties
        public int FormPdfId { get; set; }
        public FormPdf FormPdf { get; set; }
        public ICollection<FormDocRevision> FormDocRevisions { get; set; } = new List<FormDocRevision>();
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public string? ModifiedById { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }
    }
}
