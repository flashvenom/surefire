using Mantis.Data;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Policies.Models;

namespace Mantis.Domain.Forms.Models
{
    public class Certificate
    {
        public int CertificateId { get; set; }
        public string? JSONData { get; set; }
        public string? JSONDataTemp { get; set; }
        public string? HolderName { get; set; }
        public string? ProjectName { get; set; }
        public bool? AttachGLAI { get; set; }
        public string? AttachGLAIfilename { get; set; }
        public bool? AttachGLWOS { get; set; }
        public string? AttachGLWOSfilename { get; set; }
        public bool? AttachWCWOS { get; set; }
        public string? AttachWCWOSfilename { get; set; }
        public bool? AttachPNC { get; set; }
        public bool? BlockAttachments { get; set; }

        //Record Info
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public ApplicationUser? CreatedBy { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }
    }
}
