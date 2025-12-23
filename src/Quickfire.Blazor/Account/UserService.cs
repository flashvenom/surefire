using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Cryptography;

namespace Quickfire.Blazor.Domain.Users.Services
{
    public class UserService
    {
        private readonly StateService _stateService;
        private readonly UserManager<ApplicationUser> _userManager;
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

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
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
            if (userup == null)
            {
                throw new InvalidOperationException($"Unable to find user with id {user.Id}");
            }

            userup.FirstName = user.FirstName;
            userup.LastName = user.LastName;
            userup.Email = user.Email;
            userup.PhoneNumber = user.PhoneNumber;
            userup.DesktopUsername = user.DesktopUsername;
            userup.PictureUrl = user.PictureUrl;

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

        public async Task<string?> GetDesktopUsernameForUserAsync(string userIdentifier)
        {
            if (string.IsNullOrWhiteSpace(userIdentifier))
            {
                return null;
            }

            using var dbContext = _dbContextFactory.CreateDbContext();
            var match = await dbContext.Users
                .Where(u => u.Id == userIdentifier || u.UserName == userIdentifier || u.DesktopUsername == userIdentifier)
                .Select(u => new { u.DesktopUsername, u.UserName })
                .FirstOrDefaultAsync();

            if (match == null)
            {
                return null;
            }

            return string.IsNullOrWhiteSpace(match.DesktopUsername)
                ? match.UserName
                : match.DesktopUsername;
        }

        public async Task<(bool Success, string? TemporaryPassword, IEnumerable<string>? Errors)> ResetPasswordAsync(string userId, string? temporaryPassword = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return (false, null, new[] { "Invalid user identifier." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, null, new[] { "User not found." });
            }

            var password = string.IsNullOrWhiteSpace(temporaryPassword)
                ? GenerateTemporaryPassword()
                : temporaryPassword.Trim();

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, password);
            if (!resetResult.Succeeded)
            {
                return (false, null, resetResult.Errors.Select(e => e.Description));
            }

            return (true, password, null);
        }

        private static string GenerateTemporaryPassword(int length = 12)
        {
            const string allowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@$?#";
            Span<char> buffer = length <= 32 ? stackalloc char[length] : new char[length];

            using var rng = RandomNumberGenerator.Create();
            var randomBytes = new byte[length];
            rng.GetBytes(randomBytes);

            for (int i = 0; i < length; i++)
            {
                var index = randomBytes[i] % allowedChars.Length;
                buffer[i] = allowedChars[index];
            }

            return new string(buffer);
        }
    }
}
