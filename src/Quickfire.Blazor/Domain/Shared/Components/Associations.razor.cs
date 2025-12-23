using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Inputs;
using Quickfire.Blazor.Domain.Shared.Models;
using Quickfire.Blazor.Domain.Shared.Services;
using System.Timers;
using System.Text.RegularExpressions;

namespace Quickfire.Blazor.Domain.Shared.Components
{
    public enum RelationshipCategory
    {
        Business,
        Personal,
        Generic
    }

    public class RelationshipTypeItem
    {
        public RelationshipType Value { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public RelationshipCategory Category { get; set; }
        public List<string> ValidSourceTypes { get; set; } = new();
        public List<string> ValidTargetTypes { get; set; } = new();
    }

    public enum RelationshipType
    {
        // Business relationships
        PrimaryBusiness,
        SecondaryBusiness,
        SideHustle,
        ParentCompany,
        Subsidiary,
        Employer,
        Employee,
        Client,
        ServiceProvider,
        Partner,
        Vendor,
        Customer,
        Agent,
        Principal,
        
        // Personal relationships
        Spouse,
        Parent,
        Child,
        Sibling,
        
        // Generic relationships
        AssociatedEntity,
        RelatedContact
    }

    public partial class Associations
    {
        [Parameter]
        public string EntityType { get; set; } = "Contact";

        [Parameter]
        public int EntityId { get; set; }

        [Parameter]
        public FireSearchResultViewModel? PrimaryAssociation { get; set; }

        [Parameter]
        public List<FireSearchResultViewModel> AssociationList { get; set; } = new();

        [Parameter]
        public EventCallback OnAssociationsChanged { get; set; }

        // Dialog references and states
        private FluentDialog addAssociationDialogRef;
        private bool addAssociationDialogHidden = true;
        private bool deleteAssociationDialogHidden = true;
        private FireSearchResultViewModel? associationToDelete;

        // For smart association search
        private string searchTerm = "";
        private System.Timers.Timer searchDebounceTimer;
        private List<FireSearchResultViewModel> searchResults = new();
        private FireSearchResultViewModel? selectedResult;
        private bool isSearching = false;
        private CancellationTokenSource searchCancellationTokenSource = new();

        // For adding associations
        private List<string> entityTypes = new() { "All", "Client", "Contact", "Carrier" };
        private string selectedEntityType = "All";
        private string relationshipDescription = "";
        private string associationNotes = "";

        private RelationshipType selectedRelationshipType;
        private List<RelationshipTypeItem> relationshipTypeItems = new();
        private List<RelationshipTypeItem> filteredRelationshipTypes = new();
        private readonly Dictionary<RelationshipType, RelationshipType> relationshipMappings = new()
        {
            // Business relationships
            { RelationshipType.PrimaryBusiness, RelationshipType.SecondaryBusiness },
            { RelationshipType.ParentCompany, RelationshipType.Subsidiary },
            { RelationshipType.Employer, RelationshipType.Employee },
            { RelationshipType.Client, RelationshipType.ServiceProvider },
            { RelationshipType.Partner, RelationshipType.Partner }, // Symmetric
            { RelationshipType.Vendor, RelationshipType.Customer },
            { RelationshipType.Agent, RelationshipType.Principal },
            
            // Personal relationships
            { RelationshipType.Spouse, RelationshipType.Spouse }, // Symmetric
            { RelationshipType.Parent, RelationshipType.Child },
            { RelationshipType.Sibling, RelationshipType.Sibling }, // Symmetric
            
            // Generic relationships
            { RelationshipType.AssociatedEntity, RelationshipType.AssociatedEntity }, // Symmetric
            { RelationshipType.RelatedContact, RelationshipType.RelatedContact } // Symmetric
        };

        protected override void OnInitialized()
        {
            base.OnInitialized();
            
            // Initialize the debounce timer
            searchDebounceTimer = new System.Timers.Timer(300); // 300ms debounce
            searchDebounceTimer.Elapsed += SearchTimerElapsed;
            searchDebounceTimer.AutoReset = false;

            // Initialize relationship type items
            relationshipTypeItems = new List<RelationshipTypeItem>
            {
                // Business relationships
                new() { Value = RelationshipType.PrimaryBusiness, DisplayName = "Primary Business", Category = RelationshipCategory.Business, 
                    ValidSourceTypes = new List<string> { "Contact" }, ValidTargetTypes = new List<string> { "Client" } },
                new() { Value = RelationshipType.SecondaryBusiness, DisplayName = "Secondary Business", Category = RelationshipCategory.Business,
                    ValidSourceTypes = new List<string> { "Contact" }, ValidTargetTypes = new List<string> { "Client" } },
                new() { Value = RelationshipType.SideHustle, DisplayName = "Side Hustle", Category = RelationshipCategory.Business,
                    ValidSourceTypes = new List<string> { "Contact" }, ValidTargetTypes = new List<string> { "Client" } },
                new() { Value = RelationshipType.ParentCompany, DisplayName = "Parent Company", Category = RelationshipCategory.Business,
                    ValidSourceTypes = new List<string> { "Client" }, ValidTargetTypes = new List<string> { "Client" } },
                new() { Value = RelationshipType.Subsidiary, DisplayName = "Subsidiary", Category = RelationshipCategory.Business,
                    ValidSourceTypes = new List<string> { "Client" }, ValidTargetTypes = new List<string> { "Client" } },
                new() { Value = RelationshipType.Partner, DisplayName = "Business Partner", Category = RelationshipCategory.Business,
                    ValidSourceTypes = new List<string> { "Client" }, ValidTargetTypes = new List<string> { "Client" } },
                new() { Value = RelationshipType.Employer, DisplayName = "Employer", Category = RelationshipCategory.Business,
                    ValidSourceTypes = new List<string> { "Contact" }, ValidTargetTypes = new List<string> { "Client" } },
                new() { Value = RelationshipType.Employee, DisplayName = "Employee", Category = RelationshipCategory.Business,
                    ValidSourceTypes = new List<string> { "Contact" }, ValidTargetTypes = new List<string> { "Client" } },
                new() { Value = RelationshipType.Client, DisplayName = "Client", Category = RelationshipCategory.Business,
                    ValidSourceTypes = new List<string> { "Contact" }, ValidTargetTypes = new List<string> { "Client" } },
                new() { Value = RelationshipType.ServiceProvider, DisplayName = "Service Provider", Category = RelationshipCategory.Business,
                    ValidSourceTypes = new List<string> { "Contact" }, ValidTargetTypes = new List<string> { "Client" } },
                new() { Value = RelationshipType.Vendor, DisplayName = "Vendor", Category = RelationshipCategory.Business,
                    ValidSourceTypes = new List<string> { "Client" }, ValidTargetTypes = new List<string> { "Client" } },
                new() { Value = RelationshipType.Customer, DisplayName = "Customer", Category = RelationshipCategory.Business,
                    ValidSourceTypes = new List<string> { "Client" }, ValidTargetTypes = new List<string> { "Client" } },
                new() { Value = RelationshipType.Agent, DisplayName = "Agent", Category = RelationshipCategory.Business,
                    ValidSourceTypes = new List<string> { "Contact" }, ValidTargetTypes = new List<string> { "Client" } },
                new() { Value = RelationshipType.Principal, DisplayName = "Principal", Category = RelationshipCategory.Business,
                    ValidSourceTypes = new List<string> { "Client" }, ValidTargetTypes = new List<string> { "Contact" } },
                
                // Personal relationships
                new() { Value = RelationshipType.Spouse, DisplayName = "Spouse", Category = RelationshipCategory.Personal,
                    ValidSourceTypes = new List<string> { "Contact" }, ValidTargetTypes = new List<string> { "Contact" } },
                new() { Value = RelationshipType.Parent, DisplayName = "Parent", Category = RelationshipCategory.Personal,
                    ValidSourceTypes = new List<string> { "Contact" }, ValidTargetTypes = new List<string> { "Contact" } },
                new() { Value = RelationshipType.Child, DisplayName = "Child", Category = RelationshipCategory.Personal,
                    ValidSourceTypes = new List<string> { "Contact" }, ValidTargetTypes = new List<string> { "Contact" } },
                new() { Value = RelationshipType.Sibling, DisplayName = "Sibling", Category = RelationshipCategory.Personal,
                    ValidSourceTypes = new List<string> { "Contact" }, ValidTargetTypes = new List<string> { "Contact" } },
                
                // Generic relationships
                new() { Value = RelationshipType.AssociatedEntity, DisplayName = "Associated Entity", Category = RelationshipCategory.Generic,
                    ValidSourceTypes = new List<string> { "Client", "Contact", "Carrier" }, ValidTargetTypes = new List<string> { "Client", "Contact", "Carrier" } },
                new() { Value = RelationshipType.RelatedContact, DisplayName = "Related Contact", Category = RelationshipCategory.Generic,
                    ValidSourceTypes = new List<string> { "Contact" }, ValidTargetTypes = new List<string> { "Contact" } }
            };
        }

        public void Dispose()
        {
            searchDebounceTimer?.Dispose();
            searchCancellationTokenSource?.Dispose();
        }

        private void HandleSearchInput(ChangeEventArgs args)
        {
            searchTerm = args.Value?.ToString() ?? "";
            
            // Reset the timer on each keystroke
            searchDebounceTimer.Stop();
            
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                searchResults.Clear();
                selectedResult = null;
                StateHasChanged();
                return;
            }
            
            // Start the timer
            searchDebounceTimer.Start();
        }
        
        private async void SearchTimerElapsed(object sender, ElapsedEventArgs e)
        {
            await InvokeAsync(async () =>
            {
                await PerformSearch();
            });
        }
        
        private async Task PerformSearch()
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            {
                searchResults.Clear();
                selectedResult = null;
                return;
            }
            
            try
            {
                isSearching = true;
                StateHasChanged();
                
                // Cancel any ongoing search
                searchCancellationTokenSource?.Cancel();
                searchCancellationTokenSource = new CancellationTokenSource();
                
                // Execute search with cancellation token
                var results = await SearchService.SearchAllUsingSPAsync(searchTerm, searchCancellationTokenSource.Token);
                
                // Filter results to only include Client, Contact, and Carrier types
                searchResults = results
                    .Where(r => r.DataType == "Client" || r.DataType == "Contact" || r.DataType == "Carrier")
                    .ToList();
                    
                // Don't allow associating with self
                searchResults = searchResults
                    .Where(r => !(r.DataType == EntityType && r.Id == EntityId))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search error: {ex.Message}");
                searchResults.Clear();
            }
            finally
            {
                isSearching = false;
                StateHasChanged();
            }
        }
        
        private void SelectSearchResult(FireSearchResultViewModel result)
        {
            selectedResult = result;
            
            // Filter relationship types based on the source and target entity types
            filteredRelationshipTypes = relationshipTypeItems
                .Where(item => item.ValidSourceTypes.Contains(EntityType) && item.ValidTargetTypes.Contains(result.DataType))
                .ToList();
            
            // Set a default relationship type based on the entity types
            selectedRelationshipType = result.DataType switch
            {
                "Client" => RelationshipType.Client,
                "Contact" => RelationshipType.RelatedContact,
                "Carrier" => RelationshipType.ServiceProvider,
                _ => RelationshipType.AssociatedEntity
            };
        }
        
        private bool CanAddAssociation()
        {
            return selectedResult != null;
        }
        
        private void OpenAddAssociationDialog()
        {
            // Reset form fields
            selectedEntityType = "All";
            searchTerm = "";
            relationshipDescription = "";
            associationNotes = "";
            selectedResult = null;
            searchResults.Clear();
            
            // Show dialog
            addAssociationDialogHidden = false;
        }
        
        private void CancelAddAssociationDialog()
        {
            addAssociationDialogHidden = true;
        }
        
        private async Task AddAssociation()
        {
            if (!CanAddAssociation() || selectedResult == null)
            {
                return;
            }
            
            try
            {
                string relationshipDescription = GetDisplayName(selectedRelationshipType);
                string reverseRelationshipDescription = GetDisplayName(GetReverseRelationship(selectedRelationshipType));
                
                // First direction: Current Entity → Selected Entity
                var result1 = await SharedService.AddLooseAssociationAsync(
                    EntityType, EntityId,
                    selectedResult.DataType, selectedResult.Id,
                    relationshipDescription, associationNotes);
                    
                // Second direction: Selected Entity → Current Entity
                var result2 = await SharedService.AddLooseAssociationAsync(
                    selectedResult.DataType, selectedResult.Id,
                    EntityType, EntityId,
                    reverseRelationshipDescription, associationNotes);
                    
                if (result1.Success && result2.Success)
                {
                    addAssociationDialogHidden = true;
                    await OnAssociationsChanged.InvokeAsync();
                }
                else
                {
                    Console.WriteLine($"Error adding associations: {result1.Message} / {result2.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding associations: {ex.Message}");
            }
        }
        
        private string GetDisplayName(RelationshipType type)
        {
            return type switch
            {
                RelationshipType.PrimaryBusiness => "Primary Business",
                RelationshipType.SecondaryBusiness => "Secondary Business",
                RelationshipType.SideHustle => "Side Hustle",
                RelationshipType.ParentCompany => "Parent Company",
                RelationshipType.Subsidiary => "Subsidiary",
                RelationshipType.Employer => "Employer",
                RelationshipType.Employee => "Employee",
                RelationshipType.Client => "Client",
                RelationshipType.ServiceProvider => "Service Provider",
                RelationshipType.Partner => "Business Partner",
                RelationshipType.Vendor => "Vendor",
                RelationshipType.Customer => "Customer",
                RelationshipType.Agent => "Agent",
                RelationshipType.Principal => "Principal",
                RelationshipType.Spouse => "Spouse",
                RelationshipType.Parent => "Parent",
                RelationshipType.Child => "Child",
                RelationshipType.Sibling => "Sibling",
                RelationshipType.AssociatedEntity => "Associated Entity",
                RelationshipType.RelatedContact => "Related Contact",
                _ => string.Join(" ", Regex.Split(type.ToString(), @"(?<!^)(?=[A-Z])"))
            };
        }
        
        private RelationshipType GetReverseRelationship(RelationshipType type)
        {
            if (relationshipMappings.TryGetValue(type, out var reverse))
            {
                return reverse;
            }
            
            // Look for it as a value and return its key
            var pair = relationshipMappings.FirstOrDefault(kvp => kvp.Value == type);
            if (pair.Key != default)
            {
                return pair.Key;
            }
            
            return type; // Fallback to same type if no mapping found
        }
        
        private void ShowDeleteAssociationConfirmation(FireSearchResultViewModel association)
        {
            associationToDelete = association;
            deleteAssociationDialogHidden = false;
        }
        
        private void CancelDeleteAssociation()
        {
            deleteAssociationDialogHidden = true;
            associationToDelete = null;
        }
        
        private async Task ConfirmDeleteAssociation()
        {
            if (associationToDelete != null)
            {
                try
                {
                    var result = await SharedService.DeleteAssociationBetweenEntities(
                        EntityType,                         // Entity Type 1
                        EntityId,                          // Entity ID 1
                        associationToDelete.DataType,      // Entity Type 2
                        associationToDelete.Id             // Entity ID 2
                    );

                    if (result.Success)
                    {
                        await OnAssociationsChanged.InvokeAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during the association deletion process for {EntityType} {EntityId} Ex: {ex}");
                }
                finally
                {
                    deleteAssociationDialogHidden = true;
                    associationToDelete = null;
                    StateHasChanged();
                }
            }
        }

        private void NavigateToEntity(FireSearchResultViewModel entity)
        {
            switch (entity.DataType.ToLower())
            {
                case "client":
                    NavigationManager.NavigateTo($"/Clients/{entity.Id}");
                    break;
                case "contact":
                    NavigationManager.NavigateTo($"/Contacts/{entity.Id}");
                    break;
                default:
                    Console.WriteLine($"Navigation not implemented for entity type: {entity.DataType}");
                    break;
            }
        }
    }
}
