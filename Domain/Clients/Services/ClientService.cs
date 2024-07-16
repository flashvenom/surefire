// File: Domain/Clients/Services/ClientService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mantis.Domain.Clients.Models;
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

        public async Task<List<Client>> GetClientsAsync()
        {
            return await _context.Clients.ToListAsync();
        }

        // Add methods for filtering, sorting, and pagination if needed
    }
}
