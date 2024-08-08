using System.Collections.Generic;
using System.Threading.Tasks;
using Mantis.Data;
using Mantis.Domain.Carriers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace Mantis.Domain.Carriers.Services
{
    public class CarrierService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CarrierService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Carrier>> GetCarriersAsync()
        {
            return await _context.Carriers.ToListAsync();
        }

        public async Task NewCarrierQuick(Carrier carrier)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            carrier.CreatedBy = currentUser;
            _context.Carriers.Add(carrier);
            await _context.SaveChangesAsync();
        }
    }
}
