using Microsoft.AspNetCore.Components;
using Quickfire.Blazor.Domain.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading;

public class AppComponentBase : ComponentBase, IDisposable
{
    [Inject]
    protected StateService StateService { get; set; } = default!;

    [Inject]
    protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    protected bool isDisposed = false;
    protected bool IsPrerendering => NavigationManager?.Uri?.Contains("_blazor") ?? false;

    // Helper method for components to use
    protected bool ShouldSkipInitialization() => IsPrerendering || isDisposed;

    protected override async Task OnInitializedAsync()
    {
        // Use proper event-driven initialization instead of polling
        if (!StateService.IsInitialized)
        {
            try
            {
                // Wait for initialization with a reasonable timeout
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                await StateService.InitializationTask.WaitAsync(cts.Token);
            }
            catch (TimeoutException)
            {
                Console.WriteLine("StateService initialization timed out after 30 seconds");
                // Continue anyway - app might still be partially functional
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("StateService initialization was cancelled");
                // Continue anyway - app might still be partially functional
            }
        }
    }

    public virtual void Dispose()
    {
        isDisposed = true;
    }
}
