using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Policies.Models;
using Quickfire.Blazor.Domain.Contacts.Models;
using Quickfire.Blazor.Domain.Forms.Models;
using Quickfire.Blazor.Domain.Shared;
using Quickfire.Blazor.Domain.Renewals.Models;
using System.ComponentModel.DataAnnotations;

namespace Quickfire.Blazor.Domain.Clients.Models
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

    public enum EntityType
    {
        Client,
        Policy,
        Carrier,
        Contact,
        Claim,
        Renewal,
        Task
    }

    public class GlobalNote
    {
        public int Id { get; set; }
        public EntityType EntityType { get; set; }
        public int EntityId { get; set; }
        public string Text { get; set; }
        public string Tags { get; set; }
        public bool Pinned { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReminderDate { get; set; }
        public bool Deleted { get; set; }
        public bool HasMarkdown { get; set; }
    }

}
