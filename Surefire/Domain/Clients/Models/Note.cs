using Surefire.Data;
using Surefire.Domain.Policies.Models;
using Surefire.Domain.Contacts.Models;
using Surefire.Domain.Forms.Models;
using Surefire.Domain.Shared;
using Surefire.Domain.Renewals.Models;
using System.ComponentModel.DataAnnotations;

namespace Surefire.Domain.Clients.Models
{
    public class ClientNote
    {
        public int ClientNoteId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public string Note { get; set; }
        public Client Client { get; set; }
        public int ClientId { get; set; }
        public bool Deleted { get; set; }
    }
   
}
