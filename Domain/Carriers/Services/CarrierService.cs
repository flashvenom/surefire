using System.Collections.Generic;
using System.Threading.Tasks;
using Mantis.Data;
using Mantis.Domain.Carriers.Models;
using Microsoft.EntityFrameworkCore;

namespace Mantis.Domain.Carriers.Services
{
    public class CarrierService
    {
        private readonly ApplicationDbContext _context;

        public CarrierService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Carrier>> GetCarriersAsync()
        {
            return await _context.Carriers.ToListAsync();
        }
    }
}
