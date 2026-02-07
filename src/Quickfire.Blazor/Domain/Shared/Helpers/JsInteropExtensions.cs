using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Quickfire.Blazor.Domain.Shared.Helpers
{
    public static class JsInteropExtensions
    {
        public static async ValueTask InvokeVoidAsyncSafe(this IJSRuntime jsRuntime, string identifier, params object?[]? args)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync(identifier, args);
            }
            catch (JSDisconnectedException)
            {
            }
        }

        public static async ValueTask<T?> InvokeAsyncSafe<T>(this IJSRuntime jsRuntime, string identifier, params object?[]? args)
        {
            try
            {
                return await jsRuntime.InvokeAsync<T>(identifier, args);
            }
            catch (JSDisconnectedException)
            {
                return default;
            }
        }

        public static async ValueTask InvokeVoidAsyncSafe(this IJSObjectReference jsObjectReference, string identifier, params object?[]? args)
        {
            try
            {
                await jsObjectReference.InvokeVoidAsync(identifier, args);
            }
            catch (JSDisconnectedException)
            {
            }
        }

        public static async ValueTask<T?> InvokeAsyncSafe<T>(this IJSObjectReference jsObjectReference, string identifier, params object?[]? args)
        {
            try
            {
                return await jsObjectReference.InvokeAsync<T>(identifier, args);
            }
            catch (JSDisconnectedException)
            {
                return default;
            }
        }

        public static async ValueTask<IJSObjectReference?> ImportModuleSafeAsync(this IJSRuntime jsRuntime, string path)
        {
            return await jsRuntime.InvokeAsyncSafe<IJSObjectReference>("import", path);
        }

        public static async ValueTask DisposeAsyncSafe(this IJSObjectReference? jsObjectReference)
        {
            if (jsObjectReference is null)
            {
                return;
            }

            try
            {
                await jsObjectReference.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
            }
        }
    }
}
