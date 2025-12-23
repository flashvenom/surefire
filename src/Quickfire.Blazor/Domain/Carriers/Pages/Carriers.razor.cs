using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.FluentUI.AspNetCore.Components;
using Quickfire.Blazor.Domain.Carriers.Models;
using Quickfire.Blazor.Domain.Shared.Services;
using Quickfire.Blazor.Domain.Contacts.Pages;

namespace Quickfire.Blazor.Domain.Carriers.Pages
{
    public partial class Carriers : CarriersPageBase, IDisposable
    {
        [Parameter] public int? CarrierId { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private StateService StateService { get; set; } = default!;

        protected override void OnInitialized()
        {
            if (CarrierId.HasValue)
            {
                selectedCarrierId = CarrierId.Value;
                currentView = ViewMode.Edit;
                StateService.CarrierId = CarrierId.Value;
            }
            else if (StateService.ContactId > 0)
            {
                // If we were on a carrier ID, navigate to it
                NavigationManager.NavigateTo($"/Carriers/{StateService.CarrierId}", false);
        }
    }

        private void HandleCarrierCreated(int carrierId)
        {
            selectedCarrierId = carrierId;
            StateService.CarrierId = carrierId;
            currentView = ViewMode.Edit;
            NavigationManager.NavigateTo($"/Carriers/Edit/{carrierId}", false);
        }

        protected override void OnParametersSet()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

            if (CarrierId.HasValue)
            {
                selectedCarrierId = CarrierId.Value;
                StateService.CarrierId = CarrierId.Value;

                // Check the URL path to determine the correct view
                if (uri.AbsolutePath.Contains("/Edit/"))
                {
                    currentView = ViewMode.Edit;
                }
                else
                {
                    currentView = ViewMode.View; // Default to View if ID is present but path is not /Edit/
                }
            }
            else // No CarrierId in URL
            {
                if (uri.AbsolutePath.Contains("/Create"))
                {
                    currentView = ViewMode.Create;
                }
                else
                {
                    currentView = ViewMode.List; ; // Default to View if ID is present but path is not /Edit/
                }

                
            }
        }

        // Toolbar and Handling Views ------------------------------------------------------------------------ /
        public enum ViewMode
        {
            List,
            Edit,
            View,
            Create
        }
        private ViewMode currentView = ViewMode.List;
        private int selectedCarrierId = 0;
        private string SearchTerm = string.Empty;

        private void HandleOnMenuChanged(MenuChangeEventArgs args)
        {
            switch (args.Id)
            {
                case "Carrier":
                    currentView = ViewMode.Create;
                    NavigationManager.NavigateTo("/Carriers/Create", false);
                    break;
                case "Contact":
                    Navigation.NavigateTo($"/Contacts/Carrier/{StateService.CarrierId}");
                    break;
            }
        }

        private void ShowListView()
        {
            currentView = ViewMode.List;
            NavigationManager.NavigateTo("/Carriers", false);
        }

        private void ShowEditView()
        {
            currentView = ViewMode.Edit;
            StateService.CarrierId = selectedCarrierId;
            NavigationManager.NavigateTo($"/Carriers/Edit/{selectedCarrierId}", false);
        }

        private void ShowViewView()
        {
            currentView = ViewMode.View;
            StateService.CarrierId = selectedCarrierId;
            NavigationManager.NavigateTo($"/Carriers/{selectedCarrierId}", false);
        }

        private void HandleSearchInput(ChangeEventArgs e)
        {
            SearchTerm = e.Value?.ToString() ?? string.Empty;
        }

        // List View ------------------------------------------------------------------------------ /
        public void HandleCarrierSelect(int carrierId)
        {
            selectedCarrierId = carrierId;
            currentView = ViewMode.Edit;
            StateService.CarrierId = carrierId;
            StateHasChanged();
        }

        public void Dispose()
        {
            //Dispose
        }
    }
}
