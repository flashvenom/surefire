using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Carriers.Models;
using Quickfire.Blazor.Domain.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Quickfire.Blazor.Domain.Carriers.Services
{
    public class CarrierService
    {
        private readonly ApplicationDbContext _context;
        private readonly StateService _stateService;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMemoryCache _cache;

        public CarrierService(StateService stateService, ApplicationDbContext context, IMemoryCache cache, IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _stateService = stateService;
            _context = context;
            _contextFactory = contextFactory;
            _cache = cache;
        }
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//
        // GET all carriers <IQueryable>
        public IQueryable<Carrier> GetAllCarriers()
        {
            return _context.Carriers.AsQueryable();
        }

        // GET all carriers <List> [wholesalers only]
        public async Task<List<Carrier>> GetAllWholesalersAsync()
        {
            var wholesalers = await _cache.GetOrCreateAsync("AllWholesalers", async entry =>
            {
                using var context = _contextFactory.CreateDbContext();
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                return await context.Carriers.Where(r => r.Wholesaler == true).ToListAsync();
            });

            return wholesalers;
        }

        // GET all carriers <List>
        public async Task<List<Carrier>> GetAllCarriersAsync()
        {
            var carriers = await _cache.GetOrCreateAsync("AllCarriers", async entry =>
            {
                using var context = _contextFactory.CreateDbContext();
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                return await context.Carriers
                    .Include(c => c.CarrierProducts)
                        .ThenInclude(cp => cp.Product)
                    .Include(c => c.WholesalerAccess)
                        .ThenInclude(wa => wa.Carrier)
                    .Include(c => c.Contacts)
                        .ThenInclude(contact => contact.EmailAddresses)
                    .Include(c => c.Contacts)
                        .ThenInclude(contact => contact.PhoneNumbers)
                    .ToListAsync();
            });

            return carriers;
        }

        // GET a carrier by ID
        public async Task<Carrier?> GetCarrierByIdAsync(int carrierId)
        {
            // Use the factory to get a fresh context for this read operation
            using var context = await _contextFactory.CreateDbContextAsync();

            // Fetch using the fresh context
            return await context.Carriers
                .Include(c => c.Address)
                .Include(c => c.Contacts)
                    .ThenInclude(contact => contact.PhoneNumbers)
                .Include(c => c.Contacts)
                    .ThenInclude(contact => contact.EmailAddresses)
                .Include(c => c.Credentials)
                    .ThenInclude(d => d.CreatedBy)
                .Include(c => c.CarrierProducts)
                    .ThenInclude(cp => cp.Product)
                // Consider adding AsNoTracking() if this data is read-only in the component
                // .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CarrierId == carrierId);
        }

        // CREATE a new Carrier
        public async Task CreateCarrierAsync(Carrier carrier)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Carriers.Add(carrier);
            await _context.SaveChangesAsync();
        }
        public async Task<int> CreateCarrierReturnIdAsync(Carrier carrier)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Carriers.Add(carrier);
            await context.SaveChangesAsync();
            return carrier.CarrierId;
        }

        // UPDATE an existing Carrier
        public async Task UpdateCarrierAsync(Carrier carrierUpdates)
        {
            // Use the factory to get a fresh context for this specific update operation
            using var context = await _contextFactory.CreateDbContextAsync();

            // Fetch the current entity from the database within this context
            var existingCarrier = await context.Carriers.FirstOrDefaultAsync(c => c.CarrierId == carrierUpdates.CarrierId);

            if (existingCarrier != null)
            {
                existingCarrier.CarrierName = carrierUpdates.CarrierName;
                existingCarrier.LookupCode = carrierUpdates.LookupCode;
                existingCarrier.CarrierNickname = carrierUpdates.CarrierNickname;
                existingCarrier.StreetAddress = carrierUpdates.StreetAddress;
                existingCarrier.City = carrierUpdates.City;
                existingCarrier.State = carrierUpdates.State;
                existingCarrier.Zip = carrierUpdates.Zip;
                existingCarrier.Phone = carrierUpdates.Phone;
                existingCarrier.Website = carrierUpdates.Website;
                existingCarrier.QuotingWebsite = carrierUpdates.QuotingWebsite;
                existingCarrier.ServicingWebsite = carrierUpdates.ServicingWebsite;
                existingCarrier.NewSubmissionEmail = carrierUpdates.NewSubmissionEmail;
                existingCarrier.ServicingEmail = carrierUpdates.ServicingEmail;
                existingCarrier.QuickLink = carrierUpdates.QuickLink;
                existingCarrier.IssuingCarrier = carrierUpdates.IssuingCarrier;
                existingCarrier.Wholesaler = carrierUpdates.Wholesaler;
                existingCarrier.Admitted = carrierUpdates.Admitted;
                existingCarrier.IsPEO = carrierUpdates.IsPEO;
                existingCarrier.LossRunsEmail = carrierUpdates.LossRunsEmail;
                existingCarrier.LossRunsURL = carrierUpdates.LossRunsURL;
                existingCarrier.LossRunsNote = carrierUpdates.LossRunsNote;
                existingCarrier.IsFavorite = carrierUpdates.IsFavorite;
                existingCarrier.AppetiteJson = carrierUpdates.AppetiteJson;
                existingCarrier.QuotelinesJson = carrierUpdates.QuotelinesJson;
                existingCarrier.BrokerPaymentPortalUrl = carrierUpdates.BrokerPaymentPortalUrl;
                existingCarrier.Notes = carrierUpdates.Notes;
                existingCarrier.DateModified = DateTime.UtcNow;

                context.Entry(existingCarrier).State = EntityState.Modified;

                await context.SaveChangesAsync();

                _cache.Remove("AllCarriers");
                _cache.Remove("CarrierListItems");
            }
            else
            {
                throw new InvalidOperationException($"Carrier with ID {carrierUpdates.CarrierId} not found for update.");
            }
        }

        public async Task UpdateCarrierFavoriteAsync(int carrierId, bool isFavorite)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var carrier = await context.Carriers.FirstOrDefaultAsync(c => c.CarrierId == carrierId);
            if (carrier == null)
            {
                throw new InvalidOperationException($"Carrier with ID {carrierId} not found for favorite update.");
            }
            carrier.IsFavorite = isFavorite;
            carrier.DateModified = DateTime.UtcNow;
            await context.SaveChangesAsync();
            _cache.Remove("AllCarriers");
            _cache.Remove("CarrierListItems");
        }

        public async Task UpdateCarrierNotesAsync(int carrierId, string notes)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var carrier = await context.Carriers.FirstOrDefaultAsync(c => c.CarrierId == carrierId);
            if (carrier == null)
            {
                throw new InvalidOperationException($"Carrier with ID {carrierId} not found for notes update.");
            }
            carrier.Notes = notes;
            carrier.DateModified = DateTime.UtcNow;
            await context.SaveChangesAsync();
            _cache.Remove("AllCarriers");
            _cache.Remove("CarrierListItems");
        }
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//


        // CREDENTIALS ---------------------------------------------------------------- //
        public async Task<List<Credential>> GetCredentialsByCarrierIdAsync(int carrierId)
        {
            return await _context.Credentials
                .Where(c => c.CarrierId == carrierId)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task AddCredentialAsync(int carrierId, Credential credential)
        {
            using var context = _contextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);
            var cred = new Credential();
            cred.Username = credential.Username;
            cred.Password = credential.Password;
            cred.Website = credential.Website;
            cred.Notes = credential.Notes;
            cred.DateCreated = DateTime.Now;
            cred.DateModified = DateTime.Now;
            cred.CreatedBy = currentUser;
            cred.CarrierId = carrierId;

            context.Credentials.Add(cred);
            await context.SaveChangesAsync();
        }
        public async Task UpdateCredentialAsync(Credential credential)
        {
            // Use the factory to get a fresh context for this specific operation
            using var context = await _contextFactory.CreateDbContextAsync();

            // Fetch the current entity from the database within this context
            var existingCredential = await context.Credentials.FirstOrDefaultAsync(c => c.CredentialId == credential.CredentialId);

            if (existingCredential != null)
            {
                // Update the properties of the entity tracked by *this* context
                existingCredential.Username = credential.Username;
                existingCredential.Password = credential.Password;
                existingCredential.Website = credential.Website;
                existingCredential.Notes = credential.Notes;
                existingCredential.DateModified = DateTime.UtcNow;

                // No need to call context.Update(), just save changes to the tracked entity
                await context.SaveChangesAsync();
            }
            else
            {
                // Consider logging this instead of throwing if it's possible the credential
                // was deleted between the UI rendering and the user clicking save.
                // For now, keeping the exception to match original behaviour.
                throw new InvalidOperationException("Credential not found");
            }
        }
        public async Task DeleteCredentialAsync(int credentialId)
        {
            using var context = _contextFactory.CreateDbContext();
            var credential = await context.Credentials.FindAsync(credentialId);

            if (credential != null)
            {
                context.Credentials.Remove(credential);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Credential not found.");
            }
        }

        public async Task<List<CarrierListItem>> GetCarrierListAsync()
        {
            var carriers = await _cache.GetOrCreateAsync("CarrierListItems", async entry =>
            {
                using var context = _contextFactory.CreateDbContext();
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                return await context.Carriers
                    .Select(c => new CarrierListItem
                    {
                        CarrierId = c.CarrierId,
                        Name = c.CarrierName,
                        DateCreated = c.DateCreated ?? DateTime.UtcNow
                    })
                    .ToListAsync();
            });

            return carriers;
        }

        // OTHER ---------------------------------------------------------------------- //
        public async Task UpdateCarrierLogoAsync(int carrierId, string logoFilename)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var carrier = await context.Carriers.FirstOrDefaultAsync(c => c.CarrierId == carrierId);

            if (carrier != null)
            {
                carrier.LogoFilename = logoFilename;
                await context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException($"Carrier with ID {carrierId} not found for logo update.");
            }
        }

        public async Task<Carrier> RemoveLogo(int carrierId)
        {
            var carrier = await _context.Carriers.FirstOrDefaultAsync(c => c.CarrierId == carrierId);

            if (carrier == null || string.IsNullOrEmpty(carrier.LogoFilename))
            {
                return null; // No client found or no logo to remove
            }

            // Construct the file path for the logo
            string filePath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot/uploads/logos/carrier", carrier.LogoFilename);

            // Check if the file exists, and if it does, delete it
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Set the LogoFilename to null and update the client in the database
            carrier.LogoFilename = null;
            _context.Carriers.Update(carrier);
            await _context.SaveChangesAsync();

            return carrier; // Return the updated client
        }

        // GET carriers filtered by product line
        public async Task<List<Carrier>> GetCarriersByProductAsync(int productId)
        {
            var cacheKey = $"CarriersByProduct_{productId}";
            var carriers = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                using var context = _contextFactory.CreateDbContext();
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                return await context.Carriers
                    .Include(c => c.CarrierProducts)
                        .ThenInclude(cp => cp.Product)
                    .Where(c => c.CarrierProducts.Any(cp => cp.ProductId == productId && cp.IsActive))
                    .ToListAsync();
            });

            return carriers;
        }

        // WHOLESALER CARRIER ACCESS METHODS ----------------------------------------- //

        // GET carriers that a wholesaler has access to
        public async Task<List<WholesalerCarrier>> GetWholesalerCarrierAssociationsAsync(int wholesalerId, bool includeInactive = true)
        {
            using var context = _contextFactory.CreateDbContext();

            var query = context.WholesalerCarriers
                .Where(wc => wc.WholesalerId == wholesalerId)
                .Include(wc => wc.Carrier)
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(wc => wc.IsActive);
            }

            return await query
                .OrderByDescending(wc => wc.IsActive)
                .ThenBy(wc => wc.Carrier != null ? wc.Carrier.CarrierName : string.Empty)
                .ToListAsync();
        }

        public async Task<List<Carrier>> GetWholesalerAccessCarriersAsync(int wholesalerId)
        {
            var associations = await GetWholesalerCarrierAssociationsAsync(wholesalerId, includeInactive: false);
            return associations
                .Where(wc => wc.Carrier != null)
                .Select(wc => wc.Carrier!)
                .ToList();
        }

        // ADD carrier access for a wholesaler
        public async Task AddWholesalerCarrierAccessAsync(int wholesalerId, int carrierId, string? notes = null)
        {
            using var context = _contextFactory.CreateDbContext();

            // Check if the relationship already exists
            var existingAccess = await context.WholesalerCarriers
                .FirstOrDefaultAsync(wc => wc.WholesalerId == wholesalerId && wc.CarrierId == carrierId);

            if (existingAccess != null)
            {
                // If it exists but is inactive, reactivate it
                if (!existingAccess.IsActive)
                {
                    existingAccess.IsActive = true;
                    existingAccess.DateModified = DateTime.UtcNow;
                    existingAccess.Notes = notes;
                    await context.SaveChangesAsync();
                }
                return; // Already exists and active
            }

            // Create new relationship
            var wholesalerCarrier = new WholesalerCarrier
            {
                WholesalerId = wholesalerId,
                CarrierId = carrierId,
                Notes = notes,
                IsActive = true,
                DateCreated = DateTime.UtcNow
            };

            context.WholesalerCarriers.Add(wholesalerCarrier);
            await context.SaveChangesAsync();
        }

        // REMOVE carrier access for a wholesaler
        public async Task RemoveWholesalerCarrierAccessAsync(int wholesalerId, int carrierId)
        {
            using var context = _contextFactory.CreateDbContext();

            var wholesalerCarrier = await context.WholesalerCarriers
                .FirstOrDefaultAsync(wc => wc.WholesalerId == wholesalerId && wc.CarrierId == carrierId);

            if (wholesalerCarrier != null)
            {
                // Soft delete by setting IsActive to false
                wholesalerCarrier.IsActive = false;
                wholesalerCarrier.DateModified = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }
        }

        public async Task SetWholesalerCarrierAccessStatusAsync(int wholesalerId, int carrierId, bool isActive)
        {
            using var context = _contextFactory.CreateDbContext();

            var association = await context.WholesalerCarriers
                .FirstOrDefaultAsync(wc => wc.WholesalerId == wholesalerId && wc.CarrierId == carrierId);

            if (association == null)
            {
                return;
            }

            if (association.IsActive == isActive)
            {
                return;
            }

            association.IsActive = isActive;
            association.DateModified = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }

        // GET available carriers that can be added to wholesaler (excluding those already associated)
        public async Task<List<Carrier>> GetAvailableCarriersForWholesalerAsync(int wholesalerId, string searchTerm = "")
        {
            using var context = _contextFactory.CreateDbContext();

            // Get carriers that are already associated with this wholesaler
            var existingCarrierIds = await context.WholesalerCarriers
                .Where(wc => wc.WholesalerId == wholesalerId)
                .Select(wc => wc.CarrierId)
                .ToListAsync();

            var query = context.Carriers
                .Where(c => c.CarrierId != wholesalerId && !existingCarrierIds.Contains(c.CarrierId));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.CarrierName.Contains(searchTerm) ||
                    (c.CarrierNickname != null && c.CarrierNickname.Contains(searchTerm)) ||
                    (c.LookupCode != null && c.LookupCode.Contains(searchTerm)));
            }

            return await query
                .OrderBy(c => c.CarrierName)
                .Take(20) // Limit results for performance
                .ToListAsync();
        }

        // GET wholesalers that have access to a specific issuing carrier
        public async Task<List<Carrier>> GetWholesalersForCarrierAsync(int carrierId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.WholesalerCarriers
                .Where(wc => wc.CarrierId == carrierId && wc.IsActive)
                .Include(wc => wc.Wholesaler)
                .Select(wc => wc.Wholesaler)
                .ToListAsync();
        }

        // CARRIER PRODUCT METHODS --------------------------------------------------- //

        // ADD a product to a carrier
        public async Task AddCarrierProductAsync(int carrierId, int productId, bool isSpecialty = false, string? notes = null)
        {
            using var context = _contextFactory.CreateDbContext();

            // Check if the relationship already exists
            var existingCarrierProduct = await context.CarrierProducts
                .FirstOrDefaultAsync(cp => cp.CarrierId == carrierId && cp.ProductId == productId);

            if (existingCarrierProduct != null)
            {
                // If it exists but is inactive, reactivate it
                if (!existingCarrierProduct.IsActive)
                {
                    existingCarrierProduct.IsActive = true;
                    existingCarrierProduct.DateModified = DateTime.UtcNow;
                    existingCarrierProduct.ProductSpecialty = isSpecialty;
                    existingCarrierProduct.Notes = notes;
                    await context.SaveChangesAsync();
                }
                return; // Already exists and active
            }

            // Create new relationship
            var carrierProduct = new CarrierProduct
            {
                CarrierId = carrierId,
                ProductId = productId,
                ProductSpecialty = isSpecialty,
                Notes = notes,
                IsActive = true,
                DateCreated = DateTime.UtcNow
            };

            context.CarrierProducts.Add(carrierProduct);
            await context.SaveChangesAsync();

            // Invalidate relevant caches
            _cache.Remove("AllCarriers");
            _cache.Remove($"CarriersByProduct_{productId}");
        }

        // REMOVE a product from a carrier
        public async Task RemoveCarrierProductAsync(int carrierId, int productId)
        {
            using var context = _contextFactory.CreateDbContext();

            var carrierProduct = await context.CarrierProducts
                .FirstOrDefaultAsync(cp => cp.CarrierId == carrierId && cp.ProductId == productId);

            if (carrierProduct != null)
            {
                // Soft delete by setting IsActive to false
                carrierProduct.IsActive = false;
                carrierProduct.DateModified = DateTime.UtcNow;
                await context.SaveChangesAsync();

                // Invalidate relevant caches
                _cache.Remove("AllCarriers");
                _cache.Remove($"CarriersByProduct_{productId}");
            }
        }

        // CHECK if a carrier offers a specific product
        public async Task<bool> CarrierOffersProductAsync(int carrierId, int productId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.CarrierProducts
                .AnyAsync(cp => cp.CarrierId == carrierId && cp.ProductId == productId && cp.IsActive);
        }
    }
}
