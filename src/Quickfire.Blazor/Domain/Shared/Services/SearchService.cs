using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Clients.Models;
using Quickfire.Blazor.Domain.Shared.Models;
using System.Data.SqlClient;
using System.Globalization;

namespace Quickfire.Blazor.Domain.Shared.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly StateService _stateService;

        private static readonly Dictionary<string, int> _searchResultOrdering = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Client"] = 1,
            ["Contact"] = 2,
            ["Carrier"] = 3,
            ["Renewal"] = 4,
            ["Policy"] = 5,
            ["Address"] = 6,
            ["Lead"] = 7,
            ["Info"] = 8
        };

        public SearchService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IDbContextFactory<ApplicationDbContext> contextFactory, StateService stateService)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _contextFactory = contextFactory;
            _stateService = stateService;
        }


        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = await _context.Products.ToListAsync();
            return products;
        }

        public async Task<bool> HasAnyClientsAsync(CancellationToken cancellationToken = default)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Clients.AsNoTracking().AnyAsync(cancellationToken);
        }

        public async Task<List<Client>> FireSearchClients(string str)
        {
            return await _context.Clients
                .Where(c => c.Name.Contains(str) || c.Email.Contains(str) || c.LookupCode.Contains(str))
                .ToListAsync();
        }
       
        public async Task<List<FireSearchResultViewModel>> SearchAllWaitAsync(string searchTerm, CancellationToken cancellationToken, bool runSequentially = false)
        {
            var results = new List<FireSearchResultViewModel>();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return results; // Return an empty list if the search term is null or whitespace
            }

            if (runSequentially)
            {
                using var context = _contextFactory.CreateDbContext();
                var clientResults = await BuildClientSearchQuery(context, searchTerm)
                    .Take(10)
                    .ToListAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                var carrierResults = await BuildCarrierSearchQuery(context, searchTerm)
                    .Take(10)
                    .ToListAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                var contactResults = await BuildContactSearchQuery(context, searchTerm)
                    .Take(10)
                    .ToListAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                var policyResults = await BuildPolicySearchQuery(context, searchTerm)
                    .Take(10)
                    .ToListAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                var addressResults = await BuildAddressSearchQuery(context, searchTerm)
                    .Take(10)
                    .ToListAsync(cancellationToken);

                results.AddRange(clientResults);
                results.AddRange(carrierResults);
                results.AddRange(contactResults);
                results.AddRange(policyResults);
                results.AddRange(addressResults);
            }
            else
            {
                // Start queries in parallel with separate DbContext instances
                var clientTask = Task.Run(async () =>
                {
                    using var context = _contextFactory.CreateDbContext();
                    return await BuildClientSearchQuery(context, searchTerm)
                        .Take(10)
                        .ToListAsync(cancellationToken);
                }, cancellationToken);

                var carrierTask = Task.Run(async () =>
                {
                    using var context = _contextFactory.CreateDbContext();
                    return await BuildCarrierSearchQuery(context, searchTerm)
                        .Take(10)
                        .ToListAsync(cancellationToken);
                }, cancellationToken);

                var contactTask = Task.Run(async () =>
                {
                    using var context = _contextFactory.CreateDbContext();
                    return await BuildContactSearchQuery(context, searchTerm)
                        .Take(10)
                        .ToListAsync(cancellationToken);
                }, cancellationToken);

                var policyTask = Task.Run(async () =>
                {
                    using var context = _contextFactory.CreateDbContext();
                    return await BuildPolicySearchQuery(context, searchTerm)
                        .Take(10)
                        .ToListAsync(cancellationToken);
                }, cancellationToken);

                var addressTask = Task.Run(async () =>
                {
                    using var context = _contextFactory.CreateDbContext();
                    return await BuildAddressSearchQuery(context, searchTerm)
                        .Take(10)
                        .ToListAsync(cancellationToken);
                }, cancellationToken);

                await Task.WhenAll(clientTask, carrierTask, contactTask, policyTask, addressTask);

                results.AddRange(clientTask.Result);
                results.AddRange(carrierTask.Result);
                results.AddRange(contactTask.Result);
                results.AddRange(policyTask.Result);
                results.AddRange(addressTask.Result);
            }

            return results;
        }

        private IQueryable<FireSearchResultViewModel> BuildClientSearchQuery(ApplicationDbContext context, string searchTerm)
        {
            return context.Clients.AsNoTracking()
                .Where(c => c.Name.Contains(searchTerm) || c.Email.Contains(searchTerm) || c.LookupCode.Contains(searchTerm)
                            || c.PhoneNumber.Contains(searchTerm) || c.Website.Contains(searchTerm))
                .OrderBy(c => c.Name)
                .Select(c => new FireSearchResultViewModel
                {
                    DataType = "Client",
                    Id = c.ClientId,
                    Primary = c.Name,
                    Parent = ""
                });
        }

        private IQueryable<FireSearchResultViewModel> BuildCarrierSearchQuery(ApplicationDbContext context, string searchTerm)
        {
            return context.Carriers.AsNoTracking()
                .Where(c => c.CarrierName.Contains(searchTerm) || c.LookupCode.Contains(searchTerm) || c.CarrierNickname.Contains(searchTerm))
                .OrderBy(c => c.CarrierName)
                .Select(c => new FireSearchResultViewModel
                {
                    DataType = "Carrier",
                    Id = c.CarrierId,
                    Primary = c.CarrierName,
                    Parent = ""
                });
        }

        private IQueryable<FireSearchResultViewModel> BuildContactSearchQuery(ApplicationDbContext context, string searchTerm)
        {
            return context.Contacts.AsNoTracking()
                .Include(c => c.PhoneNumbers)
                .Include(c => c.EmailAddresses)
                .Include(c => c.Client)
                .Include(c => c.Carrier)
                .Where(c => c.FirstName.Contains(searchTerm) || c.LastName.Contains(searchTerm)
                            || c.EmailAddresses.Any(e => e.Email.Contains(searchTerm))
                            || c.PhoneNumbers.Any(p => p.Number.Contains(searchTerm)))
                .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
                .Select(c => new FireSearchResultViewModel
                {
                    DataType = "Contact",
                    Id = c.ContactId,
                    Primary = (c.FirstName ?? string.Empty) + " " + (c.LastName ?? string.Empty),
                    Parent = c.Client != null ? c.Client.Name : c.Carrier != null ? c.Carrier.CarrierName : ""
                });
        }

        private IQueryable<FireSearchResultViewModel> BuildPolicySearchQuery(ApplicationDbContext context, string searchTerm)
        {
            return context.Policies.AsNoTracking()
                .Where(p => p.PolicyNumber.Contains(searchTerm))
                .OrderBy(p => p.PolicyNumber)
                .Select(p => new FireSearchResultViewModel
                {
                    DataType = "Policy",
                    Id = p.ClientId,
                    Primary = p.PolicyNumber,
                    Parent = p.Client.Name
                });
        }

        private IQueryable<FireSearchResultViewModel> BuildAddressSearchQuery(ApplicationDbContext context, string searchTerm)
        {
            return (from a in context.Address.AsNoTracking()
                    join c in context.Clients.AsNoTracking() on a.AddressId equals c.Address.AddressId into clientGroup
                    from client in clientGroup.DefaultIfEmpty()
                    join cr in context.Carriers.AsNoTracking() on a.AddressId equals cr.Address.AddressId into carrierGroup
                    from carrier in carrierGroup.DefaultIfEmpty()
                    where a.AddressLine1.Contains(searchTerm) || a.City.Contains(searchTerm) || a.PostalCode.Contains(searchTerm)
                    select new FireSearchResultViewModel
                    {
                        DataType = "Address",
                        Id = client != null ? client.ClientId : carrier != null ? carrier.CarrierId : 0,
                        Primary = (a.AddressLine1 ?? string.Empty) + ", " + (a.City ?? string.Empty) + ", " + (a.State ?? string.Empty),
                        Parent = client != null ? client.Name : carrier != null ? carrier.CarrierName : ""
                    })
                   .OrderBy(x => x.Primary);
        }

        public async Task<List<FireSearchResultViewModel>> SearchAllUsingSPAsync(string searchTerm, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<FireSearchResultViewModel>();
            }

            var sanitizedTerm = searchTerm.Trim();
            var providerName = _stateService.DatabaseProvider ?? string.Empty;
            var isSqlite = providerName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase);

            if (!isSqlite && string.IsNullOrWhiteSpace(providerName))
            {
                using (var providerContext = _contextFactory.CreateDbContext())
                {
                    providerName = providerContext.Database.ProviderName ?? string.Empty;
                    isSqlite = providerName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase);
                }
            }

            try
            {
                if (isSqlite)
                {
                    return await SearchAllSqliteAsync(sanitizedTerm, cancellationToken);
                }

                using var context = _contextFactory.CreateDbContext();
                var loweredTerm = sanitizedTerm.ToLowerInvariant();

                var results = await context.FireSearchResultViewModel
                    .FromSqlRaw("EXEC dbo.SearchAllWithRenewals @SearchTerm = {0}", loweredTerm)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return results;
            }
            catch (Exception ex) when (
                ex is SqlException ||
                ex is SqliteException ||
                ex is OperationCanceledException ||
                ex is TaskCanceledException ||
                ex is InvalidOperationException)
            {
                Console.WriteLine($"Error in search: {ex.Message}");

                return new List<FireSearchResultViewModel>
                {
                    new FireSearchResultViewModel
                    {
                        DataType = "Info",
                        Primary = "[Keep typing...]",
                        Parent = string.Empty
                    }
                };
            }
        }

        private async Task<List<FireSearchResultViewModel>> SearchAllSqliteAsync(string searchTerm, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<FireSearchResultViewModel>();
            }

            cancellationToken.ThrowIfCancellationRequested();

            var sanitizedTerm = searchTerm.Trim();
            var aggregatedResults = await SearchAllWaitAsync(sanitizedTerm, cancellationToken, runSequentially: true);
            var loweredTerm = sanitizedTerm.ToLowerInvariant();

            cancellationToken.ThrowIfCancellationRequested();
            var renewalResults = await GetRenewalSearchResultsAsync(loweredTerm, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            var leadResults = await GetLeadSearchResultsAsync(loweredTerm, cancellationToken);

            var combined = new List<FireSearchResultViewModel>(aggregatedResults.Count + renewalResults.Count + leadResults.Count);
            combined.AddRange(aggregatedResults);
            combined.AddRange(renewalResults);
            combined.AddRange(leadResults);

            return SortSearchResults(combined);
        }

        private async Task<List<FireSearchResultViewModel>> GetLeadSearchResultsAsync(string loweredTerm, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();

            var leads = await context.Leads.AsNoTracking()
                .Where(l =>
                    (l.CompanyName != null && l.CompanyName.ToLower().Contains(loweredTerm)) ||
                    (l.ContactName != null && l.ContactName.ToLower().Contains(loweredTerm)) ||
                    (l.Email != null && l.Email.ToLower().Contains(loweredTerm)) ||
                    (l.PhoneNumber != null && l.PhoneNumber.ToLower().Contains(loweredTerm)))
                .Select(l => new
                {
                    l.LeadId,
                    l.CompanyName,
                    l.ContactName,
                    l.Email,
                    l.PhoneNumber
                })
                .OrderBy(l => l.CompanyName ?? l.ContactName ?? l.Email ?? l.PhoneNumber ?? string.Empty)
                .Take(10)
                .ToListAsync(cancellationToken);

            var results = new List<FireSearchResultViewModel>(leads.Count);
            foreach (var lead in leads)
            {
                var primary = !string.IsNullOrWhiteSpace(lead.CompanyName)
                    ? lead.CompanyName
                    : !string.IsNullOrWhiteSpace(lead.ContactName)
                        ? lead.ContactName
                        : lead.Email ?? lead.PhoneNumber ?? $"Lead #{lead.LeadId}";

                results.Add(new FireSearchResultViewModel
                {
                    DataType = "Lead",
                    Id = lead.LeadId,
                    Primary = primary,
                    Parent = lead.ContactName ?? string.Empty
                });
            }

            return results;
        }

        private async Task<List<FireSearchResultViewModel>> GetRenewalSearchResultsAsync(string loweredTerm, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();

            var today = DateTime.UtcNow.Date;
            var startDate = today.AddDays(-30);
            var endDate = today.AddDays(120);

            var renewals = await context.Renewals.AsNoTracking()
                .Where(r => r.RenewalDate >= startDate && r.RenewalDate <= endDate)
                .Where(r => r.Client != null && r.Client.Name != null && r.Client.Name.ToLower().Contains(loweredTerm))
                .Select(r => new
                {
                    r.RenewalId,
                    r.RenewalDate,
                    ClientName = r.Client != null ? r.Client.Name : null,
                    ProductNickname = r.Product != null ? r.Product.LineNickname : null
                })
                .OrderBy(r => r.RenewalDate)
                .ThenBy(r => r.ClientName)
                .Take(10)
                .ToListAsync(cancellationToken);

            var results = new List<FireSearchResultViewModel>(renewals.Count);
            foreach (var renewal in renewals)
            {
                results.Add(new FireSearchResultViewModel
                {
                    DataType = "Renewal",
                    Id = renewal.RenewalId,
                    Primary = renewal.ClientName ?? $"Renewal #{renewal.RenewalId}",
                    Parent = BuildRenewalParent(renewal.RenewalDate, renewal.ProductNickname)
                });
            }

            return results;
        }

        private static string BuildRenewalParent(DateTime renewalDate, string? productNickname)
        {
            var datePart = renewalDate.ToString("MM/dd", CultureInfo.InvariantCulture);
            return string.IsNullOrWhiteSpace(productNickname)
                ? datePart
                : $"{datePart} {productNickname}".Trim();
        }

        private static List<FireSearchResultViewModel> SortSearchResults(IEnumerable<FireSearchResultViewModel> results)
        {
            return results
                .Where(r => r != null)
                .OrderBy(r => GetDataTypeSortOrder(r.DataType))
                .ThenBy(r => r.Primary ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static int GetDataTypeSortOrder(string? dataType)
        {
            if (string.IsNullOrWhiteSpace(dataType))
            {
                return int.MaxValue;
            }

            return _searchResultOrdering.TryGetValue(dataType, out var order)
                ? order
                : int.MaxValue;
        }

        /// <summary>
        /// Retrieves associations for a given entity, focusing on Contact relationships.
        /// Includes the primary hard-coded client for a contact, and any loose associations
        /// stored in the EntityAssociations table (primarily Contact->Client and Contact->Contact).
        /// </summary>
        /// <param name="entityType">Currently optimized for "Contact".</param>
        /// <param name="entityId">The ID of the entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of associated entities formatted as FireSearchResultViewModel.</returns>
        public async Task<List<FireSearchResultViewModel>> GetAssociationsAsync(string entityType, int entityId, CancellationToken cancellationToken)
        {
            var results = new List<FireSearchResultViewModel>();
            using var context = _contextFactory.CreateDbContext();
            string entityTypeLower = entityType.ToLowerInvariant();

            // --- Step 1: Handle Hard-coded Links (Primarily for Contacts) ---
            if (entityTypeLower == "contact")
            {
                var contact = await context.Contacts.AsNoTracking()
                    .Include(co => co.Client) // Include the primary Client via ClientId FK
                    .FirstOrDefaultAsync(co => co.ContactId == entityId, cancellationToken);

                if (contact?.Client != null)
                {
                    results.Add(new FireSearchResultViewModel
                    {
                        DataType = "Client",
                        Id = contact.ClientId.Value, // Assuming ClientId is nullable int
                        Primary = contact.Client.Name,
                        Parent = "Primary Client (Direct)" // Clearly mark the hard-coded link
                    });
                }
            }
            // Add other hard-coded lookups here if needed for other entity types later

            // --- Step 2: Get Loose Associations from EntityAssociations Table ---
            var looseAssociations = await context.EntityAssociations.AsNoTracking()
                .Where(assoc => (assoc.EntityType1.ToLower() == entityTypeLower && assoc.EntityId1 == entityId) ||
                               (assoc.EntityType2.ToLower() == entityTypeLower && assoc.EntityId2 == entityId))
                .ToListAsync(cancellationToken);

            if (looseAssociations.Any())
            {
                // Identify the 'other' entities we need to look up
                var relatedEntitiesToLookup = looseAssociations
                    .Select(a =>
                    {
                        bool isEntity1Input = a.EntityType1.Equals(entityType, StringComparison.OrdinalIgnoreCase) && a.EntityId1 == entityId;
                        return new
                        {
                            OtherEntityType = (isEntity1Input ? a.EntityType2 : a.EntityType1).ToLowerInvariant(),
                            OtherEntityId = isEntity1Input ? a.EntityId2 : a.EntityId1,
                            Association = a // Keep original association for relationship description
                        };
                    })
                    .GroupBy(x => x.OtherEntityType)
                    .ToList(); // Now have groups by type, e.g., all "client" ids, all "contact" ids

                var nameLookup = new Dictionary<string, Dictionary<int, string>>();

                // Fetch names/display strings in batches
                foreach (var typeGroup in relatedEntitiesToLookup)
                {
                    var otherEntityType = typeGroup.Key;
                    var idsToFetch = typeGroup.Select(x => x.OtherEntityId).Distinct().ToList();
                    var names = new Dictionary<int, string>();

                    switch (otherEntityType)
                    {
                        case "client":
                            var clientNames = await context.Clients.AsNoTracking()
                                .Where(c => idsToFetch.Contains(c.ClientId))
                                .Select(c => new { Id = c.ClientId, Name = c.Name })
                                .ToListAsync(cancellationToken);
                            names = clientNames.ToDictionary(x => x.Id, x => x.Name);
                            break;
                        case "contact":
                            var contactNames = await context.Contacts.AsNoTracking()
                                .Where(c => idsToFetch.Contains(c.ContactId))
                                .Select(c => new { Id = c.ContactId, Name = (c.FirstName + " " + c.LastName).Trim() })
                                .ToListAsync(cancellationToken);
                            names = contactNames.ToDictionary(x => x.Id, x => x.Name);
                            break;
                        // Add other types here if you associate them later
                        default:
                            names = idsToFetch.ToDictionary(id => id, id => $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(otherEntityType)} #{id}");
                            break;

                    }
                    nameLookup[otherEntityType] = names;
                }

                // Create ViewModels using the fetched names
                foreach (var typeGroup in relatedEntitiesToLookup)
                {
                    var otherEntityType = typeGroup.Key;
                    if (nameLookup.TryGetValue(otherEntityType, out var namesDict))
                    {
                        foreach (var relatedEntityInfo in typeGroup)
                        {
                            string primaryName = namesDict.TryGetValue(relatedEntityInfo.OtherEntityId, out var name)
                                                   ? name
                                                   : $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(otherEntityType)} #{relatedEntityInfo.OtherEntityId}"; // Fallback

                            results.Add(new FireSearchResultViewModel
                            {
                                // Use TitleCase for display consistency
                                DataType = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(otherEntityType),
                                Id = relatedEntityInfo.OtherEntityId,
                                Primary = primaryName,
                                Parent = $"{relatedEntityInfo.Association.RelationshipDescription} (Loose)" // Mark as loose & show description
                            });
                        }
                    }
                }
            }

            // --- Step 3: Combine and Order Results ---
            return results
                .OrderBy(r => r.Parent.Contains("(Direct)") ? 0 : 1) // Show direct links first
                .ThenBy(r => r.Parent)
                .ThenBy(r => r.DataType)
                .ThenBy(r => r.Primary)
                .ToList();
        }
    }
}
