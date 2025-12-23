using Quickfire.Blazor.Domain.Renewals.ViewModels;
using Quickfire.Blazor.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quickfire.Blazor.Domain.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Quickfire.Blazor.Domain.Shared.Services
{
    public class SharedService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ApplicationDbContext _context;

        public SharedService(UserManager<ApplicationUser> userManager, AuthenticationStateProvider authenticationStateProvider, IDbContextFactory<ApplicationDbContext> contextFactory, ApplicationDbContext context)
        {
            _userManager = userManager;
            _authenticationStateProvider = authenticationStateProvider;
            _contextFactory = contextFactory;
            _context = context;
        }

        public IQueryable<Address> GetAllAddresses()
        {
            return _context.Address.AsQueryable();
        }

        public IQueryable<Product> GetAllProducts()
        {
            using var context = _contextFactory.CreateDbContext();
            var prod = context.Products.AsQueryable();
            return prod;
        }

        public async Task<Product> CreateProductAsync(Product myproduct)
        {
            //Create a new product
            var prod = new Product
            {
                LineName = myproduct.LineName,
                LineNickname = myproduct.LineNickname,
                LineCode = myproduct.LineCode,
                Description = myproduct.Description
            };
            _context.Products.Add(prod);
            await _context.SaveChangesAsync();
            return prod;
        }
        public async Task<Product> UpdateProductAsync(int productid, string linename, string nickname, string linecode, string linedesc)
        {
            using var context = _contextFactory.CreateDbContext();

            // Find the existing product in the database
            var existingProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductId == productid);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {productid} not found.");
            }

            // Update the properties of the existing product
            existingProduct.LineName = linename;
            existingProduct.LineNickname = nickname;
            existingProduct.LineCode = linecode;
            existingProduct.Description = linedesc;

            // Save the changes to the database
            await context.SaveChangesAsync();

            return existingProduct;
        }
        public async Task<List<Product>> GetAllProductsAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Products.AsNoTracking().ToListAsync();
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                using var context = _contextFactory.CreateDbContext();
                return await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }
            return null;
        }

        public async Task<(bool Success, string Message)> SaveCurrentUserProfile(Models.UserPreferences preferences)
        {
            try
            {
                var currentUser = await GetCurrentUserAsync();
                if (currentUser == null)
                {
                    return (false, "User not found");
                }

                using var context = _contextFactory.CreateDbContext();
                
                // Get the user from the context for tracking
                var userToUpdate = await context.Users.FirstOrDefaultAsync(u => u.Id == currentUser.Id);
                if (userToUpdate == null)
                {
                    return (false, "User not found in database");
                }

                // Apply preferences to the user
                preferences.ApplyToApplicationUser(userToUpdate);

                // Save changes
                await context.SaveChangesAsync();

                return (true, "User preferences saved successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user profile: {ex.Message}");
                return (false, $"An error occurred while saving preferences: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message, int? AssociationId)> AddLooseAssociationAsync(
                string entityType1, int entityId1,
                string entityType2, int entityId2,
                string relationshipDescription, string? notes = null)
        {
            if (string.IsNullOrWhiteSpace(relationshipDescription))
            {
                return (false, "Relationship description cannot be empty.", null);
            }

            try
            {
                using var context = _contextFactory.CreateDbContext();

                // Basic validation (could add checks if entities actually exist if needed)
                if (string.IsNullOrWhiteSpace(entityType1) || string.IsNullOrWhiteSpace(entityType2) || entityId1 <= 0 || entityId2 <= 0)
                {
                    return (false, "Invalid entity type or ID provided.", null);
                }

                var newAssociation = new EntityAssociation
                {
                    EntityType1 = entityType1,
                    EntityId1 = entityId1,
                    EntityType2 = entityType2,
                    EntityId2 = entityId2,
                    RelationshipDescription = relationshipDescription.Trim(),
                    Notes = notes,
                    DateCreated = DateTime.UtcNow
                };

                context.EntityAssociations.Add(newAssociation);
                await context.SaveChangesAsync();

                return (true, "Association created successfully.", newAssociation.AssociationId);
            }
            catch (Exception ex)
            {
                // Log exception (ex)
                Console.WriteLine($"Error adding loose association: {ex.Message}");
                return (false, "An error occurred while creating the association.", null);
            }
        }

        /// <summary>
        /// Removes a loose association by its unique ID.
        /// </summary>
        public async Task<(bool Success, string Message)> RemoveLooseAssociationAsync(int associationId)
        {
            if (associationId <= 0)
            {
                return (false, "Invalid Association ID.");
            }

            try
            {
                using var context = _contextFactory.CreateDbContext();
                var association = await context.EntityAssociations.FindAsync(associationId);

                if (association == null)
                {
                    return (false, "Association not found.");
                }

                context.EntityAssociations.Remove(association);
                await context.SaveChangesAsync();

                return (true, "Association removed successfully.");
            }
            catch (Exception ex)
            {
                // Log exception (ex)
                Console.WriteLine($"Error removing loose association {associationId}: {ex.Message}");
                return (false, "An error occurred while removing the association.");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAssociationBetweenEntities(
            string entityType1, int entityId1,
            string entityType2, int entityId2)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(entityType1) || string.IsNullOrWhiteSpace(entityType2) || entityId1 <= 0 || entityId2 <= 0)
            {
                return (false, "Invalid entity type or ID provided.");
            }

            try
            {
                using var context = _contextFactory.CreateDbContext();

                // Find the association in the database by matching entity types and IDs, checking both directions
                var association = await context.EntityAssociations
                    .FirstOrDefaultAsync(a =>
                        (a.EntityType1 == entityType1 && a.EntityId1 == entityId1 &&
                         a.EntityType2 == entityType2 && a.EntityId2 == entityId2) ||
                        (a.EntityType1 == entityType2 && a.EntityId1 == entityId2 &&
                         a.EntityType2 == entityType1 && a.EntityId2 == entityId1));

                if (association == null)
                {
                    // Association doesn't exist
                    return (false, "Association not found between the specified entities.");
                }

                // Remove the found association directly
                context.EntityAssociations.Remove(association);
                await context.SaveChangesAsync();

                return (true, "Association removed successfully.");

                /*
                // --- Alternative using the existing RemoveLooseAssociationAsync ---
                // This involves potentially two lookups (one here, one inside RemoveLooseAssociationAsync)
                // but reuses existing logic if preferred.
                if (association != null)
                {
                    // Call the other service method to perform the actual deletion
                    var result = await RemoveLooseAssociationAsync(association.AssociationId);
                    return result; // Return the result from the called method
                }
                else
                {
                     return (false, "Association not found between the specified entities.");
                }
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting association between entities: {ex.Message}");
                return (false, "An error occurred while deleting the association.");
            }
        }
    }
}