// File: Domain/Clients/Services/ClientService.cs
//using Mantis.Domain.Clients.Models;
//using Mantis.Data;
using Mantis.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Policies.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;


namespace Mantis.Domain.Clients.Services
{
    public class ClientService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CrmApiService _crmApiService;

        public ClientService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, CrmApiService crmApiService)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _crmApiService = crmApiService;
        }

        public async Task<List<Client>> GetCarriersAsync()
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task NewClientQuick(Client client)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            client.CreatedBy = currentUser;
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        public async Task<Client> GetClientById(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Address)
                .Include(c => c.PrimaryContact)
                .Include(c => c.Locations)
                .Include(c => c.Contacts)
                .Include(c => c.Policies)
                    .ThenInclude(p => p.Carrier)
                .Include(c => c.Policies)
                    .ThenInclude(p => p.Wholesaler)
                .FirstOrDefaultAsync(c => c.ClientId == id);

            return client;
        }

        public async Task UpdateClientAsync(Client client)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var existingClient = await _context.Clients
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.ClientId == client.ClientId);

            if (existingClient != null)
            {
                // Update the basic client fields
                existingClient.Name = client.Name;
                existingClient.LookupCode = client.LookupCode;
                existingClient.PhoneNumber = client.PhoneNumber;
                existingClient.Email = client.Email;
                existingClient.Website = client.Website;
                existingClient.Comments = client.Comments;

                // Update the associated Address
                if (existingClient.Address != null && client.Address != null)
                {
                    existingClient.Address.AddressLine1 = client.Address.AddressLine1;
                    existingClient.Address.AddressLine2 = client.Address.AddressLine2;
                    existingClient.Address.City = client.Address.City;
                    existingClient.Address.State = client.Address.State;
                    existingClient.Address.PostalCode = client.Address.PostalCode;
                }

                // Update any other navigation properties if necessary
                //existingClient.PrimaryContactId = client.PrimaryContactId;

                // Track who made the changes, if necessary
                existingClient.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }

        // Create a new client
        public async Task CreateClientAsync(Client client)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            client.CreatedBy = currentUser;
            client.eClientId = Guid.NewGuid().ToString();
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClientId(int clientId, string eClientId)
        {
            var client = await _context.Clients.FindAsync(clientId);

            if (client != null && !string.IsNullOrEmpty(eClientId))
            {
                client.eClientId = eClientId;
                _context.Clients.Update(client);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Invalid clientId or eClientId");
            }
        }

        public async Task UpdateClientIdAndPolicies(int clientId, string eClientId, List<Policy> policies, string accesstoken)
        {
            var client = await _context.Clients
                .Include(c => c.Policies)
                .FirstOrDefaultAsync(c => c.ClientId == clientId);

            if (client != null && !string.IsNullOrEmpty(eClientId))
            {
                client.eClientId = eClientId;

                foreach (var policy in policies)
                {
                    policy.ClientId = clientId;

                    var policyLinesJson = await _crmApiService.GetPolicyLinesAsync(policy.ePolicyId, accesstoken);
                    var policyLinesData = JObject.Parse(policyLinesJson);

                    if (policyLinesData["_embedded"]?["lines"] is JArray linesList && linesList.Count > 0)
                    {
                        var firstLine = linesList[0] as JObject;
                        policy.ePolicyLineId = firstLine["id"].ToString();

                        var issuingCompanyCode = firstLine["issuingCompany"]?["lookupCode"]?.ToString();
                        var premiumPayableCode = firstLine["premiumPayable"]?["lookupCode"]?.ToString();

                        if (!string.IsNullOrEmpty(issuingCompanyCode))
                        {
                            var carrier = await _context.Carriers.FirstOrDefaultAsync(c => c.LookupCode == issuingCompanyCode);
                            policy.Carrier = carrier;
                        }

                        if (!string.IsNullOrEmpty(premiumPayableCode))
                        {
                            var wholesaler = await _context.Carriers.FirstOrDefaultAsync(c => c.LookupCode == premiumPayableCode);
                            policy.Wholesaler = wholesaler;
                        }

                        // Store additional lines in the notes field
                        if (linesList.Count > 1)
                        {
                            policy.Notes = string.Join("; ", linesList.Skip(1).Select(line => line.ToString()));
                        }
                    }

                    switch (policy.eTypeCode)
                    {
                        case "EQFL":
                            policy.ProductId = 17;
                            break;
                        case "EPLX":
                            policy.ProductId = 8;
                            break;
                        case "EART":
                            policy.ProductId = 18;
                            break;
                        case "ACCD":
                            policy.ProductId = 11;
                            break;
                        case "GDEN":
                        case "DENT":
                            policy.ProductId = 15;
                            break;
                        case "CUMB":
                            policy.ProductId = 7;
                            break;
                        case "INLM":
                            policy.ProductId = 16;
                            break;
                        case "EPLI":
                            policy.ProductId = 8;
                            break;
                        case "BOP":
                            policy.ProductId = 6;
                            break;
                        case "GLIA":
                            policy.ProductId = 3;
                            break;
                        case "WCOM":
                            policy.ProductId = 2;
                            break;
                        case "PROP":
                            policy.ProductId = 14;
                            break;
                        case "BAUT":
                            policy.ProductId = 4;
                            break;
                        case "PROF":
                            policy.ProductId = 5;
                            break;
                        case "GHEA":
                            policy.ProductId = 9;
                            break;
                        case "BOND":
                            policy.ProductId = 12;
                            break;
                        case "CPKG":
                            policy.ProductId = 13;
                            break;
                        default:
                            policy.ProductId = 10;
                            break;
                    }

                    client.Policies.Add(policy);
                }

                _context.Clients.Update(client);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Invalid clientId or eClientId");
            }
        }
    }
}
