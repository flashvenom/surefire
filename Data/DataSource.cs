// File: Mantis.Data/DataSource.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Carriers.Models;

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

    public class CarrierData
    {
        private readonly ApplicationDbContext _dbContext;

        public CarrierData(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Carrier>> GetCarriersAsync()
        {
            return await _dbContext.Carriers
                .Include(c => c.Address)
                .ToListAsync();
        }
        public async Task<Carrier> SaveCarrierAsync(Carrier carrier)
        {
            carrier = _dbContext.Carriers.Add(carrier).Entity;
            await _dbContext.SaveChangesAsync();
            return carrier;
        }
    }

    public class CrmApiOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
