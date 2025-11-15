using Surefire.Data;
using Surefire.Domain.Accounting.Models;
using Surefire.Domain.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace Surefire.Domain.Accounting.Services
{
    public class AccountingService
    {

        private readonly StateService _stateService;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public AccountingService(StateService stateService, IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _stateService = stateService;
            _contextFactory = contextFactory;
        }

        public async Task<Settlement> GetOrCreateSettlementByRenewalIdAsync(int renewalId)
        {
            using var context = _contextFactory.CreateDbContext();
            var settlement = await context.Settlements
                .Include(s => s.SettlementItems)
                .Include(s => s.Renewal)
                    .ThenInclude(r => r.Client)
                .FirstOrDefaultAsync(s => s.RenewalId == renewalId);

            if (settlement == null)
            {
                settlement = new Settlement { RenewalId = renewalId, BillType = BillType.Direct, SettlementItems = new List<SettlementItem>() };
                context.Settlements.Add(settlement);
                await context.SaveChangesAsync();
            }
            return settlement;
        }
        public async Task SaveSettlementAsync(Settlement settlement)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Settlements.Update(settlement);
            await context.SaveChangesAsync();
        }
        public async Task DeleteSettlementItemAsync(int settlementItemId)
        {
            using var context = _contextFactory.CreateDbContext();

            // Find the SettlementItem by ID
            var settlementItem = await context.SettlementItems.FirstOrDefaultAsync(item => item.SettlementItemId == settlementItemId);

            if (settlementItem == null)
            {
                throw new ArgumentException($"No SettlementItem found with ID {settlementItemId}");
            }

            // Remove the SettlementItem from the database
            context.SettlementItems.Remove(settlementItem);

            // Save changes
            await context.SaveChangesAsync();
        }

        public async Task SavePurpleSheetJson(int renewalId, string jsonData)
        {
            try
            {
                var filePath = Path.Combine("wwwroot", "uploads", "temp", $"{renewalId}.json");
                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException());

                // Write JSON data to file
                await File.WriteAllTextAsync(filePath, jsonData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving Purple Sheet JSON: {ex.Message}", ex);
            }
        }

    }
}