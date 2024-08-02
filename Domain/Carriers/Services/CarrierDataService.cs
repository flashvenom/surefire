using System.Collections.Generic;
using System.Threading.Tasks;
using Mantis.Domain.Carriers.Models;
using Microsoft.EntityFrameworkCore;

namespace Mantis.Shared.DataAccess
{
    public class OrderService
    {
        private readonly OrderContext _context;

        public OrderService(OrderContext context)
        {
            _context = context;
        }

        public async Task<List<Carrier>> GetOrdersAsync()
        {
            return await _context.Carriers.ToListAsync();
        }
        public async Task AddOrder(Carrier carrier)
        {
            _context.Carriers.Add(carrier);
            await _context.SaveChangesAsync();
        }
        // Other CRUD operations
    }
}
