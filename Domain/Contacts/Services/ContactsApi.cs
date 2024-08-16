using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Contacts.Models;


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
            if(ParentType == "Carrier")
            {
                return _context.Contacts.Where(x => x.CarrierId == ParentId).ToList();
            }
            else
            {
                return _context.Contacts.Take(5).ToList();
            }
            
        }
    }
}
