using Surefire.Domain.Contacts.Models;
using System.ComponentModel.DataAnnotations;

namespace Surefire.Domain.Plugins
{
    public class Plugin
    {
        [Key]
        public int Id { get; set; }
        public string HashId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ShortDescription { get; set; }
        public string PluginWebsite { get; set; }
        public string DeveloperName { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? Jwt { get; set; }
        public string? TokenUri { get; set; }
        public string? BaseUri { get; set; }
        public string? RedirectUri { get; set; }
        public string? Scope { get; set; }
        public string? GrantType { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class PluginMethodResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public bool cleanup { get; set; }
        public string jsonresponse { get; set; }
        public List<Contact> contacts { get; set; } = new();
    }

}
