using Surefire.Data;
using Surefire.Domain.Contacts.Models;
using Surefire.Domain.Shared;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Surefire.Domain.Policies.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Surefire.Domain.Shared.Services;

namespace Surefire.Domain.Users.Services
{
    public class UserService
    {
        private readonly StateService _stateService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public UserService(StateService stateService, UserManager<ApplicationUser> userManager, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _stateService = stateService;
            _userManager = userManager;
            _dbContextFactory = dbContextFactory;

        }
        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Users
                .OrderBy(u => u.UserName) // Optional: Order by username
                .ToListAsync();
        }
        public async Task<bool> UpdateUserDetailsAsync(string userId, string fieldName, string newValue)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return false; // User not found
            }

            // Update the appropriate field
            switch (fieldName)
            {
                case "FirstName":
                    user.FirstName = newValue;
                    break;
                case "LastName":
                    user.LastName = newValue;
                    break;
                case "Email":
                    user.Email = newValue;
                    break;
                case "PhoneNumber":
                    user.PhoneNumber = newValue;
                    break;
                case "DesktopUsername":
                    user.DesktopUsername = newValue;
                    break;
                default:
                    throw new ArgumentException("Invalid field name", nameof(fieldName));
            }

            // Save changes
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();
            return true;
        }
        public async Task AddUserAsync(ApplicationUser user)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(ApplicationUser user)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var userup = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            userup.FirstName = user.FirstName;
            userup.LastName = user.LastName;
            userup.Email = user.Email;
            userup.PhoneNumber = user.PhoneNumber;
            userup.DesktopUsername = user.DesktopUsername;

            dbContext.Users.Update(userup);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string userId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                dbContext.Users.Remove(user);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateLastLoginAsync(ApplicationUser user)
        {
            using var context = _dbContextFactory.CreateDbContext();
            user.LastLogin = DateTime.UtcNow;
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task<string?> GetCurrentUserDesktopUsernameAsync()
        {
            var currentUser = _stateService.CurrentUser;
            return currentUser?.DesktopUsername;
        }
    }
}
