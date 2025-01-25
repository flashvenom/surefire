using Surefire.Domain.Clients.Models;
using Surefire.Domain.Contacts.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Surefire.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        public string? PictureUrl { get; set; }
        public string? LastLookupClient { get; set; }
        public string? LastLookupPerson { get; set; }
        public int? LastRenewalId { get; set; }
        public int? LastRenewalMonth { get; set; }
        public int? LastRenewalYear { get; set; }
        public string? LastRenewalPerson { get; set; }
        public string? LastRenewalScreen { get; set; }
        public string? DesktopUsername { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    public class CallInfo
    {
        public string CallerId { get; set; }
        public string CallerName { get; set; }
    }
    public class CallInfoMatchResult
    {
        public Client MatchedClient { get; set; }
        public Contact MatchedContact { get; set; }
        public string? ToastParam { get; set; }
    }
    
}
