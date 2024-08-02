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

        public async Task UpdateeClientId(int clientId, string eClientId)
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

                    client.Policies.Add(policy);

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
