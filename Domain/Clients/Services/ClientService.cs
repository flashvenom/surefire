// File: Domain/Clients/Services/ClientService.cs
using Mantis.Domain.Clients.Models;
using Mantis.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mantis.Domain.Clients.Models;

namespace Mantis.Domain.Clients.Services
{
    public class ClientService
    {
        private readonly ApplicationDbContext _context;

        public ClientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> GetClientsAsync()
        {
            return await _context.Clients.ToListAsync();
        }
    }
}
