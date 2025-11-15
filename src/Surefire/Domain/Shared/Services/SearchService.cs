using Surefire.Data;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace Surefire.Domain.Shared.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly StateService _stateService;

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

        public async Task<List<Client>> FireSearchClients(string str)
        {
            return await _context.Clients
                .Where(c => c.Name.Contains(str) || c.Email.Contains(str) || c.LookupCode.Contains(str))
                .ToListAsync();
        }
       
        public async Task<List<FireSearchResultViewModel>> SearchAllWaitAsync(string searchTerm, CancellationToken cancellationToken)
        {
            var results = new List<FireSearchResultViewModel>();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return results; // Return an empty list if the search term is null or whitespace
            }

            // Start queries in parallel with separate DbContext instances
            var clientTask = Task.Run(async () =>
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.Clients.AsNoTracking()
                    .Where(c => c.Name.Contains(searchTerm) || c.Email.Contains(searchTerm) || c.LookupCode.Contains(searchTerm)
                                || c.PhoneNumber.Contains(searchTerm) || c.Website.Contains(searchTerm))
                    .Select(c => new FireSearchResultViewModel
                    {
                        DataType = "Client",
                        Id = c.ClientId,
                        Primary = c.Name,
                        Parent = "" // Clients have no parent, so this is empty
                    })
                    .Take(10)
                    .ToListAsync(cancellationToken);
            });

            var carrierTask = Task.Run(async () =>
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.Carriers.AsNoTracking()
                    .Where(c => c.CarrierName.Contains(searchTerm) || c.LookupCode.Contains(searchTerm) || c.CarrierNickname.Contains(searchTerm))
                    .Select(c => new FireSearchResultViewModel
                    {
                        DataType = "Carrier",
                        Id = c.CarrierId,
                        Primary = c.CarrierName,
                        Parent = "" // Carriers have no parent, so this is empty
                    })
                    .Take(10)
                    .ToListAsync(cancellationToken);
            });

            var contactTask = Task.Run(async () =>
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.Contacts.AsNoTracking()
                    .Where(c => c.FirstName.Contains(searchTerm) || c.LastName.Contains(searchTerm)
                                || c.Email.Contains(searchTerm) || c.Phone.Contains(searchTerm))
                    .Select(c => new FireSearchResultViewModel
                    {
                        DataType = "Contact",
                        Id = c.ContactId,
                        Primary = $"{c.FirstName} {c.LastName}",
                        Parent = c.Client != null ? c.Client.Name : c.Carrier != null ? c.Carrier.CarrierName : "" // Parent can be either Client or Carrier
                    })
                    .Take(10)
                    .ToListAsync(cancellationToken);
            });

            var policyTask = Task.Run(async () =>
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.Policies.AsNoTracking()
                    .Where(p => p.PolicyNumber.Contains(searchTerm))
                    .Select(p => new FireSearchResultViewModel
                    {
                        DataType = "Policy",
                        Id = p.ClientId,  // Return the ClientId instead of PolicyId
                        Primary = p.PolicyNumber,
                        Parent = p.Client.Name // Policy is linked to a Client
                    })
                    .Take(10)
                    .ToListAsync(cancellationToken);
            });

            var addressTask = Task.Run(async () =>
            {
                using var context = _contextFactory.CreateDbContext();
                return await (from a in context.Address.AsNoTracking()
                              join c in context.Clients.AsNoTracking() on a.AddressId equals c.Address.AddressId into clientGroup
                              from client in clientGroup.DefaultIfEmpty()
                              join cr in context.Carriers.AsNoTracking() on a.AddressId equals cr.Address.AddressId into carrierGroup
                              from carrier in carrierGroup.DefaultIfEmpty()
                              where a.AddressLine1.Contains(searchTerm) || a.City.Contains(searchTerm) || a.PostalCode.Contains(searchTerm)
                              select new FireSearchResultViewModel
                              {
                                  DataType = "Address",
                                  Id = client != null ? client.ClientId : carrier != null ? carrier.CarrierId : 0,
                                  Primary = $"{a.AddressLine1}, {a.City}, {a.State}",
                                  Parent = client != null ? client.Name : carrier != null ? carrier.CarrierName : ""
                              })
                             .Take(10)
                             .ToListAsync(cancellationToken);
            });

            // Await all tasks
            await Task.WhenAll(clientTask, carrierTask, contactTask, policyTask, addressTask);

            // Collect results
            results.AddRange(clientTask.Result);
            results.AddRange(carrierTask.Result);
            results.AddRange(contactTask.Result);
            results.AddRange(policyTask.Result);
            results.AddRange(addressTask.Result);

            return results;
        }

        public async Task<List<FireSearchResultViewModel>> SearchAllUsingSPAsync(string searchTerm, CancellationToken cancellationToken)
        {
            // Use the stored database provider name from StateService
            if (_stateService.DatabaseProvider.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                // Fallback to SearchAllWaitAsync for SQLite
                return await SearchAllWaitAsync(searchTerm, cancellationToken);
            }
            try
            {
                using var context = _contextFactory.CreateDbContext();

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return new List<FireSearchResultViewModel>();
                }

                searchTerm = searchTerm.ToLower();

                // Execute the stored procedure
                var results = await context.FireSearchResultViewModel
                    .FromSqlRaw("EXEC dbo.SearchAllWithRenewals @SearchTerm = {0}", searchTerm)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return results;
            }
            catch (Exception ex) when (
                ex is SqlException ||
                ex is OperationCanceledException ||
                ex is TaskCanceledException ||
                ex is InvalidOperationException)
            {
                // Log the error if needed (e.g., using a logging framework)
                Console.WriteLine($"Error in search: {ex.Message}");

                // Return an indicator to keep typing
                return new List<FireSearchResultViewModel>
                {
                    new FireSearchResultViewModel
                    {
                        DataType = "Info",
                        Primary = "[Keep typing...]", // Message to display in UI
                        Parent = ""
                    }
                };
            }
        }
    }
}
