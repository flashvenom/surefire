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
        //Removed until we need them
    }
}
