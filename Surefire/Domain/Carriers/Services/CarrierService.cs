using Surefire.Data;
using Surefire.Domain.Carriers.Models;
using Surefire.Domain.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Surefire.Domain.Carriers.Services
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
                return await context.Carriers.ToListAsync();
            });

            return carriers;
        }

        // GET a carrier by ID
        public async Task<Carrier?> GetCarrierByIdAsync(int carrierId)
        {
            return await _context.Carriers
                .Include(c => c.Address)
                .Include(c => c.Contacts)
                .Include(c => c.Credentials)
                    .ThenInclude(d => d.CreatedBy)
                .FirstOrDefaultAsync(c => c.CarrierId == carrierId);
        }

        // CREATE a new Carrier
        public async Task CreateCarrierAsync(Carrier carrier)
        {
            //carrier.CreatedBy = _stateService.CurrentUser;
            _context.Carriers.Add(carrier);
            await _context.SaveChangesAsync();
        }

        // UPDATE an existing Carrier
        public async Task UpdateCarrierAsync(Carrier carrier)
        {
            //var existingCarrier = await _context.Carriers.FirstOrDefaultAsync(c => c.CarrierId == carrier.CarrierId);

            if (carrier != null)
            {
                carrier.CarrierName = carrier.CarrierName;
                carrier.LookupCode = carrier.LookupCode;
                carrier.CarrierNickname = carrier.CarrierNickname;
                carrier.StreetAddress = carrier.StreetAddress;
                carrier.City = carrier.City;
                carrier.State = carrier.State;
                carrier.Zip = carrier.Zip;
                carrier.Phone = carrier.Phone;
                carrier.Website = carrier.Website;
                carrier.QuotingWebsite = carrier.QuotingWebsite;
                carrier.ServicingWebsite = carrier.ServicingWebsite;
                carrier.NewSubmissionEmail = carrier.NewSubmissionEmail;
                carrier.ServicingEmail = carrier.ServicingEmail;
                carrier.IssuingCarrier = carrier.IssuingCarrier;
                carrier.Wholesaler = carrier.Wholesaler;
                carrier.AppetiteJson = carrier.AppetiteJson;
                carrier.QuotelinesJson = carrier.QuotelinesJson;
                carrier.Notes = carrier.Notes;
                _context.Carriers.Update(carrier);
                await _context.SaveChangesAsync();
            }
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
            var existingCredential = await _context.Credentials.FirstOrDefaultAsync(c => c.CredentialId == credential.CredentialId);
            if (existingCredential != null)
            {
                existingCredential.Username = credential.Username;
                existingCredential.Password = credential.Password;
                existingCredential.Website = credential.Website;
                existingCredential.Notes = credential.Notes;
                existingCredential.DateModified = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
            else
            {
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

        // OTHER ---------------------------------------------------------------------- //
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
    }
}
