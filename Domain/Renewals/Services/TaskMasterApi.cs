using Microsoft.AspNetCore.Mvc;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Carriers.Models;
using Mantis.Domain.Renewals.Models;
using Mantis.Components.Pages.Renewals;


namespace Mantis.Domain.Carriers.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskMasterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaskMasterController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public List<TaskMaster> GetList()
        {
            return _context.TaskMasters.ToList();
        }

        [HttpPost]
        public object Post([FromBody] DataManagerRequest DataManagerRequest)
        {
            IEnumerable<TaskMaster> DataSource = GetList();

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
            int TotalRecordsCount = DataSource.Cast<TaskMaster>().Count();
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
        public void Insert([FromBody] CRUDModel<TaskMaster> Value)
        {
            _context.TaskMasters.Add(Value.Value);
            _context.SaveChangesAsync();
        }

        [HttpPost("Update")]
        public async void Update([FromBody] CRUDModel<TaskMaster> Value)
        {
            var existingOrder = _context.TaskMasters.Find(Value.Value.Id);
            
            //existingOrder.CreatedBy = currentUser;
            if (existingOrder != null)
            {
                //var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                //existingOrder.CreatedBy = currentUser;
                _context.Entry(existingOrder).CurrentValues.SetValues(Value.Value);
                _context.SaveChanges();
            }
        }

        [HttpPost("Delete")]
        public void Delete([FromBody] CRUDModel<TaskMaster> Value)
        {
            //BRTEAKPOINT DOES NOT HIT HERE
            var existingOrder = _context.Renewals.Find(Value.Value.Id);
            //Delete Code
        }
    }
}
