// File: Domain/Clients/Services/ClientService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mantis.Domain.Clients.Models;
using Radzen;
using Mantis.Data;

namespace Mantis.Domain.Clients.Services
{
    public class ClientService
    {
        private readonly ApplicationDbContext _context;

        public ClientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Client> Data, int Count)> GetClientsAsync(int skip, int take, string orderBy, IEnumerable<FilterDescriptor> filters)
        {
            var query = _context.Clients.AsQueryable();

            // Apply filters
            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    if (filter.FilterOperator == FilterOperator.Equals)
                    {
                        query = query.Where($"{filter.Property} == @0", filter.FilterValue);
                    }
                    else if (filter.FilterOperator == FilterOperator.Contains)
                    {
                        query = query.Where($"{filter.Property}.Contains(@0)", filter.FilterValue);
                    }
                    // Add additional filter operator cases as needed
                }
            }

            // Get the count before paging
            var count = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrEmpty(orderBy))
            {
                query = query.OrderBy(orderBy);
            }

            // Apply paging
            query = query.Skip(skip).Take(take);

            var data = await query.ToListAsync();

            return (data, count);
        }
    }
}
