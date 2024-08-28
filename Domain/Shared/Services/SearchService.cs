using Mantis.Domain.Shared;
using Mantis.Domain.Clients.Models;
using System.Net;
using Mantis.Domain.Carriers.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Syncfusion.Blazor.Navigations;
using Mantis.Domain.Renewals.ViewModels;
using Mantis.Data;
using Microsoft.AspNetCore.Identity;
using Mantis.Domain.Policies.Models;
using Microsoft.EntityFrameworkCore;
using Mantis.Domain.Shared.Models;

namespace Mantis.Domain.Shared.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SearchService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = await _context.Products.ToListAsync();
            return products;
        }

        public async Task<List<Client>> FireSearchClients(string str)
        {
            return await _context.Clients
                .Where(c => c.Name.Contains(str) || c.Email.Contains(str) || c.LookupCode.Contains(str))
                .ToListAsync();
        }

        public async Task<List<FireSearchResultViewModel>> SearchAllAsync(string searchTerm)
        {
            var results = new List<FireSearchResultViewModel>();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return results; // Return an empty list if the search term is null or whitespace
            }

            // Search Clients
            var clientResults = await _context.Clients
                .Where(c => c.Name.Contains(searchTerm) || c.Email.Contains(searchTerm) || c.LookupCode.Contains(searchTerm) || c.PhoneNumber.Contains(searchTerm) || c.Website.Contains(searchTerm))
                .Select(c => new FireSearchResultViewModel
                {
                    DataType = "Client",
                    Id = c.ClientId,
                    Primary = c.Name,
                    Parent = "" // Clients have no parent, so this is empty
                })
                .Take(10)
                .ToListAsync();
            results.AddRange(clientResults);

            // Search Carriers
            var carrierResults = await _context.Carriers
                .Where(c => c.CarrierName.Contains(searchTerm) || c.LookupCode.Contains(searchTerm) || c.CarrierNickname.Contains(searchTerm))
                .Select(c => new FireSearchResultViewModel
                {
                    DataType = "Carrier",
                    Id = c.CarrierId,
                    Primary = c.CarrierName,
                    Parent = "" // Carriers have no parent, so this is empty
                })
                .Take(10)
                .ToListAsync();
            results.AddRange(carrierResults);

            // Search Contacts
            var contactResults = await _context.Contacts
                .Where(c => c.FirstName.Contains(searchTerm) || c.LastName.Contains(searchTerm) || c.Email.Contains(searchTerm) || c.Phone.Contains(searchTerm))
                .Select(c => new FireSearchResultViewModel
                {
                    DataType = "Contact",
                    Id = c.ContactId,
                    Primary = $"{c.FirstName} {c.LastName}",
                    Parent = c.Client != null ? c.Client.Name : c.Carrier != null ? c.Carrier.CarrierName : "" // Parent can be either Client or Carrier
                })
                .Take(10)
                .ToListAsync();
            results.AddRange(contactResults);

            // Search Policies
            var policyResults = await _context.Policies
                .Where(p => p.PolicyNumber.Contains(searchTerm))
                .Select(p => new FireSearchResultViewModel
                {
                    DataType = "Policy",
                    Id = p.ClientId,  // Return the ClientId instead of PolicyId
                    Primary = p.PolicyNumber,
                    Parent = p.Client.Name // Policy is linked to a Client
                })
                .Take(10)
                .ToListAsync();
            results.AddRange(policyResults);

            var addressResults = await _context.Address
                .Where(a => a.AddressLine1.Contains(searchTerm) || a.City.Contains(searchTerm) || a.PostalCode.Contains(searchTerm))
                .Select(a => new FireSearchResultViewModel
                {
                    DataType = "Address",
                    Id = _context.Clients
                            .Where(c => c.Address.AddressId == a.AddressId)
                            .Select(c => (int?)c.ClientId) // Return ClientId if found
                            .Union(_context.Carriers
                                    .Where(cr => cr.Address.AddressId == a.AddressId)
                                    .Select(cr => (int?)cr.CarrierId)) // Return CarrierId if found
                            .FirstOrDefault() ?? 0, // Default to 0 if no match is found
                    Primary = $"{a.AddressLine1}, {a.City}, {a.State}",
                    Parent = _context.Clients
                            .Where(c => c.Address.AddressId == a.AddressId)
                            .Select(c => c.Name)
                            .Union(_context.Carriers
                                    .Where(cr => cr.Address.AddressId == a.AddressId)
                                    .Select(cr => cr.CarrierName))
                            .FirstOrDefault() // An address can belong to either a Client or Carrier
                })
                .Take(10)
                .ToListAsync();
            results.AddRange(addressResults);

            return results;
        }

        private string NormalizeString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return new string(input.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToLowerInvariant();
        }

    }
   
}
