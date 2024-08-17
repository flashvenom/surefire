using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Contacts.Models;
using Microsoft.EntityFrameworkCore;
using Mantis.Domain.Policies.Models;
using Syncfusion.Blazor;


namespace Mantis.Domain.Contacts.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContactsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("{ParentType}/{ParentId}")]
        public List<Contact> GetContactsList(string ParentType, int ParentId)
        {
            // Filter contacts based on ParentType and ParentId
            if (ParentType == "Carrier")
            {
                return _context.Contacts.Where(x => x.CarrierId == ParentId).ToList();
            }
            else if(ParentType == "Client")
            {
                return _context.Contacts.Where(x => x.ClientId == ParentId).ToList();
            }
            else
            {
                return _context.Contacts.Take(5).ToList();
            }

        }
        [HttpGet("List")]
        public List<ContactDto> GetContactsDto()
        {
            return _context.Contacts
                .Include(c => c.Client)
                .Include(c => c.Carrier)
                .Select(c => new ContactDto
                {
                    ContactId = c.ContactId,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Title = c.Title,
                    Email = c.Email,
                    Phone = c.Phone,
                    Fax = c.Fax,
                    Mobile = c.Mobile,
                    Notes = c.Notes,
                    DateCreated = c.DateCreated,
                    AssociatedWith = c.Client != null ? c.Client.Name : c.Carrier != null ? c.Carrier.CarrierName : "N/A"
                })
                .ToList();
        }

        [HttpPost("List")]
        public object Post([FromBody] DataManagerRequest DataManagerRequest)
        {
            IEnumerable<ContactDto> DataSource = GetContactsDto();

            if (DataManagerRequest.Search != null && DataManagerRequest.Search.Count > 0)
            {
                DataSource = DataOperations.PerformSearching(DataSource, DataManagerRequest.Search);
            }

            if (DataManagerRequest.Where != null && DataManagerRequest.Where.Count > 0)
            {
                DataSource = DataOperations.PerformFiltering(DataSource, DataManagerRequest.Where, DataManagerRequest.Where[0].Operator);
            }

            if (DataManagerRequest.Sorted != null && DataManagerRequest.Sorted.Count > 0)
            {
                DataSource = DataOperations.PerformSorting(DataSource, DataManagerRequest.Sorted);
            }

            int TotalRecordsCount = DataSource.Cast<ContactDto>().Count();

            if (DataManagerRequest.Skip != 0)
            {
                DataSource = DataOperations.PerformSkip(DataSource, DataManagerRequest.Skip);
            }

            if (DataManagerRequest.Take != 0)
            {
                DataSource = DataOperations.PerformTake(DataSource, DataManagerRequest.Take);
            }

            return new { result = DataSource, count = TotalRecordsCount };
        }

    }
}
