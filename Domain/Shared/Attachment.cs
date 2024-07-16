using Folient.Domain.Shared;
using Folient.Domain.Clients.Models;
using Folient.Domain.Policies.Models;
using Folient.Domain.Renewals.Models;

using System.Net;

namespace Folient.Domain.Shared
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
