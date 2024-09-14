using Mantis.Domain.Shared;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Carriers.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Mantis.Data;
using System;
using System.Threading.Tasks;

namespace Mantis.Domain.Shared.Services
{
    public class NavigationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NavigationService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SetLastClientPageAsync(string url)
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);

            if (currentUser == null)
            {
                Console.WriteLine("User not found. Unable to save last client page.");
                return;
            }

            // Set the LastLookupClient to the URL
            currentUser.LastLookupClient = url;

            // Save the changes
            var result = await _userManager.UpdateAsync(currentUser);

            if (result.Succeeded)
            {
                Console.WriteLine("Saved last url to: " + url);
            }
            else
            {
                Console.WriteLine("Failed to save last url.");
            }
        }

        public async Task<string> GetLastClientPageAsync()
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);

            if (currentUser == null)
            {
                Console.WriteLine("User not found. Returning default client page.");
                return "/Clients/List/211";
            }

            // Get the LastLookupClient from the current user
            var url = currentUser.LastLookupClient;

            // If the URL is null, set it to the default client page
            if (string.IsNullOrEmpty(url))
            {
                url = "/Clients/List/211";
            }

            Console.WriteLine("Returning last url: " + url);
            return url;
        }
    }
}
