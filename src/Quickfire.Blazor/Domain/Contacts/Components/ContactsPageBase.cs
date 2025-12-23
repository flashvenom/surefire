using Microsoft.AspNetCore.Components;
using Quickfire.Blazor.Domain.Contacts.Services;

namespace Quickfire.Blazor.Domain.Contacts.Components
{
    public abstract class ContactsPageBase : AppComponentBase
    {
        [Inject] protected ContactService ContactService { get; set; } = default!;
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [CascadingParameter] public Action<string> UpdateHeader { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            UpdateHeader?.Invoke("Contacts");
        }
    }
}