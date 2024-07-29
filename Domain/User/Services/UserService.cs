using Mantis.Data;
using Mantis.Domain.Contacts.Models;
using Mantis.Domain.Shared;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Mantis.Domain.User.Services
{
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;

        public UserService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public ApplicationUser GetLoggedInUser()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            return loggedInUser;
        }
    }
}
