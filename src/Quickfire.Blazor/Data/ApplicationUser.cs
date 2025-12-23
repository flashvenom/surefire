using Quickfire.Blazor.Domain.Clients.Models;
using Quickfire.Blazor.Domain.Contacts.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Quickfire.Blazor.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();

        public string? PictureUrl { get; set; }
        public string? LastLookupClient { get; set; }
        public string? DesktopUsername { get; set; }
        public bool? EnableAudio { get; set; }
        public bool? EnableAnimations { get; set; }
        public bool? EnableSimpleMode { get; set; }
        public string? HomepageLayoutJSON { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    public class CallInfo
    {
        public string CallerId { get; set; }
        public string CallerName { get; set; }
        public string? SessionId { get; set; }
    }
    public class CallInfoMatchResult
    {
        public Client MatchedClient { get; set; }
        public Contact MatchedContact { get; set; }
    }
    
}
