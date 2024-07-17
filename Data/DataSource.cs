// File: Mantis.Data/DataSource.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mantis.Domain.Clients.Models;

namespace Mantis.Data
{
    public class DataSource
    {
        private readonly ApplicationDbContext _dbContext;

        public DataSource(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            return await _dbContext.Clients
                .Include(c => c.Address)
                .Include(c => c.PrimaryContact)
                .ToListAsync();
        }
        public async Task<Client> GetClientByIdAsync(int clientId)
        {
            return await _dbContext.Clients
                .Include(c => c.Address)
                .Include(c => c.PrimaryContact)
                .FirstOrDefaultAsync(c => c.ClientId == clientId);
        }
    }

    public class CrmApiOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
