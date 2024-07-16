using Mantis.Domain.Shared;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Policies.Models;
using Mantis.Domain.Renewals.Models;

using System.Net;

namespace Mantis.Domain.Shared
{
    public class Attachment
    {
        public int AttachmentId { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public string StoreType { get; set; }
        public string ContentType { get; set; }
        
    }
}
