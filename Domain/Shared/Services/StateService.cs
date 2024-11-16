using Mantis.Data;
using Mantis.Domain.Carriers.Models;
using Mantis.Domain.Policies.Models;
using Mantis.Domain.Renewals.Models;
using Mantis.Domain.Shared.Models;
using Mantis.Domain.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Mantis.Domain.Shared.Services
{
    public class StateService
    {
        // Database context and service provider
        private readonly IServiceProvider _serviceProvider;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        

        public StateService(IServiceProvider serviceProvider, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _serviceProvider = serviceProvider;
            _dbContextFactory = dbContextFactory;
          
        }

        //=============================================
        //             ** STATIC DATA **              //
        //=============================================
        private bool _isInitialized = false;
        public bool IsInitialized => _isInitialized;
        
        // Static lists Props ----------------------------------------------------------------//
        private Task<List<Carrier>>? _allCarriersTask;
        private Task<List<Carrier>>? _allWholesalersTask;
        private Task<List<Product>>? _allProductsTask;
        private Task<List<ApplicationUser>>? _allUsersTask;
        public Task<List<Carrier>> AllCarriers => _allCarriersTask ??= LoadCarriersAsync();
        public Task<List<Carrier>> AllWholesalers => _allWholesalersTask ??= LoadWholesalersAsync();
        public Task<List<Product>> AllProducts => _allProductsTask ??= LoadProductsAsync();
        public Task<List<ApplicationUser>> AllUsers => _allUsersTask ??= LoadUsersAsync();
        // Static lists Methods---------------------------------------------------------------//
        public async Task InitializeStateAsync(Task<AuthenticationState> authStateTask)
        {
            if (_isInitialized) return;

            // User initialization
            var authState = await authStateTask;
            var user = authState.User;
            using var context = _dbContextFactory.CreateDbContext();
            if (user.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                CurrentUser = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            }

            // Data initialization
            _allCarriersTask ??= LoadCarriersAsync();
            _allWholesalersTask ??= LoadWholesalersAsync();
            _allProductsTask ??= LoadProductsAsync();
            _allUsersTask ??= LoadUsersAsync();

            await Task.WhenAll(_allCarriersTask, _allWholesalersTask, _allProductsTask, _allUsersTask);
            _isInitialized = true;
        }
        private async Task<List<Carrier>> LoadCarriersAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Carriers.ToListAsync();
        }
        private async Task<List<Carrier>> LoadWholesalersAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Carriers.Where(c => c.Wholesaler).ToListAsync();
        }
        private async Task<List<Product>> LoadProductsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Products.ToListAsync();
        }
        private async Task<List<ApplicationUser>> LoadUsersAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Users.ToListAsync();
        }
        // CurrentUser Props -----------------------------------------------------------------//
        public ApplicationUser? CurrentUser { get; private set; }
        


        //=============================================
        //               RENEWAL STATE               //
        //=============================================
        public int HtmlRenewalId { get; set; } = 0;
        public int HtmlMonth { get; set; } = DateTime.Now.Month;
        public int HtmlYear { get; set; } = DateTime.Now.Year;
        public string HtmlTab { get; set; } = "tab-1";
        public string HtmlUser { get; set; } = "Everyone";
        public string HtmlView { get; set; } = "list";
        public bool IsLoading { get; set; } = false;
        public List<RenewalListItemViewModel> RenewalList { get; set; } = new();
        public List<Policy> PolicyOrphanList { get; set; } = new();

    }
}
