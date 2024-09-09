using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Renewals.Models;


namespace Mantis.Domain.Carriers.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public List<Client> GetClientList()
        {
            return _context.Clients.OrderByDescending(c => c.DateOpened).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClientById(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Address)
                .Include(c => c.PrimaryContact)
                .Include(c => c.Locations)
                .Include(c => c.Contacts)
                .Include(c => c.Policies)
                    .ThenInclude(p => p.Carrier)
                .Include(c => c.Policies)
                    .ThenInclude(p => p.Wholesaler)
                .FirstOrDefaultAsync(c => c.ClientId == id);

            if (client == null)
            {
                return NotFound();
            }
            return client;
        }

        [HttpPost]
        public object Post([FromBody] DataManagerRequest DataManagerRequest)
        {
            IEnumerable<Client> DataSource = GetClientList();

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
            int TotalRecordsCount = DataSource.Cast<Client>().Count();
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


        [HttpPost("Insert")]
        public void Insert([FromBody] CRUDModel<Client> Value)
        {
            _context.Clients.Add(Value.Value);
            _context.SaveChangesAsync();
        }

        [HttpPost("Update")]
        public async void Update([FromBody] CRUDModel<Client> Value)
        {
            var existingRecord = _context.Carriers.Find(Value.Value.ClientId);

            if (existingRecord != null)
            {
                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                existingRecord.CreatedBy = currentUser;
                _context.Entry(existingRecord).CurrentValues.SetValues(Value.Value);
                _context.SaveChanges();
            }
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] CRUDModel<Client> Value)
        {
            int recordId = Convert.ToInt32(Value.Key.ToString());
            var selectedRecord = await _context.Clients.FindAsync(recordId);
            _context.Clients.Remove(selectedRecord);
            _context.SaveChangesAsync();
        }
    }
}
