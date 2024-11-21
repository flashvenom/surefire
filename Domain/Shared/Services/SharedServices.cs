using Mantis.Domain.Renewals.ViewModels;
using Mantis.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mantis.Domain.Carriers.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Mantis.Domain.Shared.Services
{
    public class SharedService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SharedService(ApplicationDbContext context, IDbContextFactory<ApplicationDbContext> contextFactory, UserManager<ApplicationUser> userManager, AuthenticationStateProvider authenticationStateProvider, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _contextFactory = contextFactory;
            _userManager = userManager;
            _authenticationStateProvider = authenticationStateProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<Address> GetAllAddresses()
        {
            return _context.Address.AsQueryable();
        }

        public IQueryable<Product> GetAllProducts()
        {
            return _context.Products.AsQueryable();
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
        public async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                using var context = _contextFactory.CreateDbContext();
                return await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }
            return null;
        }

    }
}
