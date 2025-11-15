using Microsoft.AspNetCore.Components;
using Surefire.Domain.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;

public class AppComponentBase : ComponentBase
{
    [Inject]
    protected StateService StateService { get; set; } = default!;

    [Inject]
    protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await StateService.InitializeStateAsync(AuthStateProvider.GetAuthenticationStateAsync());
    }
}
