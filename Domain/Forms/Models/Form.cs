using Mantis.Domain.Clients.Models;
using Mantis.Data;
using System.Net;

namespace Mantis.Domain.Forms.Models
{
    public class FormDoc
    {
        public int FormDocId { get; set; }
        public string? FormName { get; set; }
        public string? FileName { get; set; }
        public string? JSONData { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }
    }

    public class FormDocRevision
    {
        public int FormDocRevisionId { get; set; }
        public string? RevisionName { get; set; }
        public string? JSONData { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        //Dependencies
        public ApplicationUser? CreatedBy { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }
        public FormDoc FormDoc { get; set; }
        public int FormDocId { get; set; }

    }
}
