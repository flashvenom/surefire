using Surefire.Domain.Renewals.ViewModels;
using Surefire.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Surefire.Domain.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Surefire.Domain.Shared.Services
{
    public class SharedService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ApplicationDbContext _context;

        public SharedService(UserManager<ApplicationUser> userManager, AuthenticationStateProvider authenticationStateProvider, IDbContextFactory<ApplicationDbContext> contextFactory, ApplicationDbContext context)
        {
            _userManager = userManager;
            _authenticationStateProvider = authenticationStateProvider;
            _contextFactory = contextFactory;
            _context = context;
        }

        public IQueryable<Address> GetAllAddresses()
        {
            return _context.Address.AsQueryable();
        }

        public IQueryable<Product> GetAllProducts()
        {
            using var context = _contextFactory.CreateDbContext();
            var prod = context.Products.AsQueryable();
            return prod;
        }

        public async Task<Product> CreateProductAsync(Product myproduct)
        {
            //Create a new product
            var prod = new Product
            {
                LineName = myproduct.LineName,
                LineNickname = myproduct.LineNickname,
                LineCode = myproduct.LineCode,
                Description = myproduct.Description
            };
            _context.Products.Add(prod);
            await _context.SaveChangesAsync();
            return prod;
        }
        public async Task<Product> UpdateProductAsync(int productid, string linename, string nickname, string linecode, string linedesc)
        {
            using var context = _contextFactory.CreateDbContext();

            // Find the existing product in the database
            var existingProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductId == productid);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {productid} not found.");
            }

            // Update the properties of the existing product
            existingProduct.LineName = linename;
            existingProduct.LineNickname = nickname;
            existingProduct.LineCode = linecode;
            existingProduct.Description = linedesc;

            // Save the changes to the database
            await context.SaveChangesAsync();

            return existingProduct;
        }
        public async Task<List<Product>> GetAllProductsAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Products.AsNoTracking().ToListAsync();
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
