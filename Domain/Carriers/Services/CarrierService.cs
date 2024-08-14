using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
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

        public async Task<List<Carrier>> GetAllCarriersAsync()
        {
            return await _context.Carriers.ToListAsync();
        }

        public async Task<List<Carrier>> GetAllWholesalersAsync()
        {
            return await _context.Carriers.Where(r => r.Wholesaler == true).ToListAsync();
        }

        public async Task NewCarrierQuick(Carrier carrier)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            carrier.CreatedBy = currentUser;
            _context.Carriers.Add(carrier);
            await _context.SaveChangesAsync();
        }


        // Get Carrier by ID
        public async Task<Carrier> GetCarrierByIdAsync(int carrierId)
        {
            return await _context.Carriers
                .Include(c => c.Contacts) // Include related entities as needed
                .FirstOrDefaultAsync(c => c.CarrierId == carrierId);
        }

        // Create a new Carrier
        public async Task CreateCarrierAsync(Carrier carrier)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            carrier.CreatedBy = currentUser;

            ValidateCarrier(carrier);
            
            _context.Carriers.Add(carrier);
            await _context.SaveChangesAsync();
        }

        // Update an existing Carrier
        public async Task UpdateCarrierAsync(Carrier carrier)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var existingCarrier = await _context.Carriers.FirstOrDefaultAsync(c => c.CarrierId == carrier.CarrierId);

            if (existingCarrier != null)
            {
                existingCarrier.CarrierName = carrier.CarrierName;
                existingCarrier.LookupCode = carrier.LookupCode;
                existingCarrier.CarrierNickname = carrier.CarrierNickname;
                existingCarrier.StreetAddress = carrier.StreetAddress;
                existingCarrier.City = carrier.City;
                existingCarrier.State = carrier.State;
                existingCarrier.Zip = carrier.Zip;
                existingCarrier.Phone = carrier.Phone;
                existingCarrier.Website = carrier.Website;
                existingCarrier.QuotingWebsite = carrier.QuotingWebsite;
                existingCarrier.ServicingWebsite = carrier.ServicingWebsite;
                existingCarrier.NewSubmissionEmail = carrier.NewSubmissionEmail;
                existingCarrier.ServicingEmail = carrier.ServicingEmail;
                existingCarrier.IssuingCarrier = carrier.IssuingCarrier;
                existingCarrier.Wholesaler = carrier.Wholesaler;
                existingCarrier.AppetiteJson = carrier.AppetiteJson;
                existingCarrier.QuotelinesJson = carrier.QuotelinesJson;
                existingCarrier.Notes = carrier.Notes;
                existingCarrier.CreatedBy = currentUser; // Update the user who made the changes, if necessary

                await _context.SaveChangesAsync();
            }
        }

        private void ValidateCarrier(Carrier carrier)
        {
            var validationContext = new ValidationContext(carrier);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(carrier, validationContext, validationResults, true);

            if (!isValid)
            {
                var errors = string.Join("; ", validationResults.Select(r => r.ErrorMessage));
                throw new ValidationException($"Carrier model is not valid: {errors}");
            }
        }
    }
}
