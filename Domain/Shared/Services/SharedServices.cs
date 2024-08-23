using Mantis.Domain.Shared;
using Mantis.Domain.Clients.Models;
using System.Net;
using Mantis.Domain.Carriers.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Syncfusion.Blazor.Navigations;
using Mantis.Domain.Renewals.ViewModels;
using Mantis.Domain.Shared;
using Mantis.Data;
using Microsoft.AspNetCore.Identity;
using Mantis.Domain.Policies.Models;
using Microsoft.EntityFrameworkCore;

namespace Mantis.Domain.Shared.Services
{
    public class SharedService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SharedService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = await _context.Products.ToListAsync();
            return products;
        }

        public async Task<List<ProductViewModel>> GetAllProductsViewModelAsync()
        {
            return await _context.Products
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.LineName
                })
                .ToListAsync();
        }

        public ApplicationUser GetApplicationUserById(string userId)
        {
            var user = _userManager.FindByIdAsync(userId).GetAwaiter().GetResult();

            if (user == null)
            {
                // Handle the case where the user is not found
                return null;
            }
            else
            {
                return user;
            }
        }
    }
}
