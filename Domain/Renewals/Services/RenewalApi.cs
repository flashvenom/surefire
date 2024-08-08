using Microsoft.AspNetCore.Mvc;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Renewals.Models;
using Microsoft.EntityFrameworkCore;


namespace Mantis.Domain.Renewals.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class RenewalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RenewalController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public List<Renewal> GetList()
        {
            var renewals = _context.Renewals
                   .Include(r => r.Product)
                   .Include(r => r.Client)
                   .Include(r => r.AssignedTo)
                   .Include(r => r.Carrier)
                   .Include(r => r.Wholesaler)
                   .ToList();
            return renewals;
        }

        [HttpPost]
        public object Post([FromBody] DataManagerRequest DataManagerRequest)
        {
            IEnumerable<Renewal> DataSource = GetList();

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
            int TotalRecordsCount = DataSource.Cast<Renewal>().Count();
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
        public void Insert([FromBody] CRUDModel<Renewal> Value)
        {
            _context.Renewals.Add(Value.Value);
            _context.SaveChangesAsync();
        }

        [HttpPost("Update")]
        public async void Update([FromBody] CRUDModel<Renewal> Value)
        {
            var existingOrder = _context.Carriers.Find(Value.Value.RenewalId);
            
            //existingOrder.CreatedBy = currentUser;
            if (existingOrder != null)
            {
                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                existingOrder.CreatedBy = currentUser;
                _context.Entry(existingOrder).CurrentValues.SetValues(Value.Value);
                _context.SaveChanges();
            }
        }

        [HttpPost("Delete")]
        public void Delete([FromBody] CRUDModel<Renewal> Value)
        {
            //BRTEAKPOINT DOES NOT HIT HERE
            var existingOrder = _context.Renewals.Find(Value.Value.RenewalId);
            //Delete Code
        }
    }
}
