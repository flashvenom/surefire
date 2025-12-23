using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.FluentUI.AspNetCore.Components;
using Quickfire.Blazor.Domain.Contacts.Components;
using Quickfire.Blazor.Domain.Shared.Services;

namespace Quickfire.Blazor.Domain.Contacts.Pages
{
    public partial class Contacts : ContactsPageBase, IDisposable
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private StateService StateService { get; set; } = default!;

        [Parameter]
        public string? EntityType { get; set; }

        [Parameter]
        public int? EntityId { get; set; }

        [Parameter]
        public int? ContactId { get; set; }

        private string? ReturnUrl => EntityType?.ToLower() switch
        {
            "client" => $"/Clients/{EntityId}",
            "carrier" => $"/Carriers/{EntityId}",
            _ => null
        };

        public enum ViewMode
        {
            List,
            Edit,
            Create
        }

        private ViewMode currentView = ViewMode.List;
        private int selectedContactId = 0;
        private string SearchTerm = string.Empty;

        protected override void OnInitialized()
        {
            if (ContactId.HasValue)
            {
                selectedContactId = ContactId.Value;
                currentView = ViewMode.Edit;
                StateService.ContactId = ContactId.Value;
            }
            else if (!string.IsNullOrEmpty(EntityType) && EntityId.HasValue)
            {
                currentView = ViewMode.Create;
                StateService.ContactId = 0;
            }
            else if (StateService.ContactId > 0)
            {
                // If we have a saved contact ID, navigate to it
                NavigationManager.NavigateTo($"/Contacts/{StateService.ContactId}", false);
            }
        }

        protected override void OnParametersSet()
        {
            if (ContactId.HasValue)
            {
                selectedContactId = ContactId.Value;
                currentView = ViewMode.Edit;
                StateService.ContactId = ContactId.Value;
            }
            else if (!string.IsNullOrEmpty(EntityType) && EntityId.HasValue)
            {
                currentView = ViewMode.Create;
                StateService.ContactId = 0;
            }
            else if (currentView == ViewMode.Create)
            {
                // Keep the create view if we're already in it
                StateService.ContactId = 0;
                return;
            }
            else
            {
                currentView = ViewMode.List;
                selectedContactId = 0;
                StateService.ContactId = 0;
            }
        }

        private async Task HandleRowSelected(int contactId)
        {
            selectedContactId = contactId;
            currentView = ViewMode.Edit;
            StateService.ContactId = contactId;
            StateHasChanged();
        }

        private void ShowListView()
        {
            currentView = ViewMode.List;
            selectedContactId = 0;
            StateService.ContactId = 0;
            NavigationManager.NavigateTo("/Contacts", false);
        }

        private void ShowEditView()
        {
            if (selectedContactId != 0)
            {
                currentView = ViewMode.Edit;
                StateService.ContactId = selectedContactId;
                NavigationManager.NavigateTo($"/Contacts/{selectedContactId}", false);
            }
        }

        private void ShowCreateView()
        {
            currentView = ViewMode.Create;
            StateService.ContactId = 0;
            StateHasChanged();
        }

        private async Task HandleContactCreated(int contactId)
        {
            selectedContactId = contactId;
            currentView = ViewMode.Edit;
            StateService.ContactId = contactId;
            NavigationManager.NavigateTo($"/Contacts/{contactId}", false);
        }

        private void HandleSearchInput(ChangeEventArgs e)
        {
            SearchTerm = e.Value?.ToString() ?? string.Empty;
        }

        public void Dispose()
        {
           //Dispose
        }
    }
}