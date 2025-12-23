using Microsoft.AspNetCore.Components;
using Quickfire.Blazor.Domain.Carriers.Services;

namespace Quickfire.Blazor.Domain.Carriers.Pages
{
    public abstract class CarriersPageBase : AppComponentBase
    {
        [Inject] protected CarrierService CarrierService { get; set; } = default!;
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [CascadingParameter] public Action<string>? UpdateHeader { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            UpdateHeader?.Invoke("Carriers");
        }
    }
}