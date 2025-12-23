using Microsoft.JSInterop;

namespace Quickfire.Blazor.Domain.Shared.Services;

public sealed class AppJsInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly SemaphoreSlim _moduleLock = new(1, 1);
    private IJSObjectReference? _module;

    public AppJsInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async ValueTask<IJSObjectReference?> TryGetModuleAsync()
    {
        var lockTaken = false;
        try
        {
            if (_module is not null)
            {
                return _module;
            }

            await _moduleLock.WaitAsync();
            lockTaken = true;
            if (_module is not null)
            {
                return _module;
            }

            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/app-interop.js");
            return _module;
        }
        catch (JSDisconnectedException)
        {
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
        finally
        {
            if (lockTaken)
            {
                _moduleLock.Release();
            }
        }
    }

    public async ValueTask InitializeTopBarAnimationsAsync()
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("initializeTopBarAnimations");
    }

    public async ValueTask PlayTopBarVideoAsync(string section)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("playTopBarVideo", section);
    }

    public async ValueTask PlayTopBarVideoOtherAsync()
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("playTopBarVideoOther");
    }

    public async ValueTask StartLogoLoadingAsync()
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("startLogoLoading");
    }

    public async ValueTask StopLogoLoadingAsync()
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("stopLogoLoading");
    }

    public async ValueTask BlurFieldAsync(string elementId)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("blurField", elementId);
    }

    public async ValueTask AddRenewalStatusColorsAsync(object datasource, string elementId)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("addRenewalStatusColors", datasource, elementId);
    }

    public async ValueTask DownloadFileFromStreamAsync(string fileName, DotNetStreamReference streamReference)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("downloadFileFromStream", fileName, streamReference);
    }

    public async ValueTask OpenPdfInNewWindowAsync(string base64Pdf)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("openPdfInNewWindow", base64Pdf);
    }

    public async ValueTask DownloadPdfAsync(string base64Pdf, string fileName)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("downloadPdf", base64Pdf, fileName);
    }

    public async ValueTask OpenWindowAsync(string url, string? target = null)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("openWindow", url, target);
    }

    public async ValueTask CopyToClipboardAsync(string? text)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("copyToClipboard", text ?? string.Empty);
    }

    public async ValueTask AlertAsync(string message)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("showAlert", message);
    }

    public async ValueTask<bool> ConfirmAsync(string message)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return false;
        }

        return await module.InvokeAsync<bool>("showConfirm", message);
    }

    public async ValueTask SetDragDataAsync(string filePath)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("setDragData", filePath);
    }

    public async ValueTask SetCheckboxStateAsync(int taskId, bool isIndeterminate, bool isChecked)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("setCheckboxState", taskId, isIndeterminate, isChecked);
    }

    public async ValueTask RegisterBeforeUnloadAutoSaveAsync()
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("registerBeforeUnloadAutoSave");
    }

    public async ValueTask<string?> ParallaxInitAsync()
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return null;
        }

        return await module.InvokeAsync<string?>("parColInit");
    }

    public async ValueTask ParallaxDisposeAsync(string token)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("parColDispose", token);
    }

    public async ValueTask<string?> LocalStorageGetItemAsync(string key)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return null;
        }

        return await module.InvokeAsync<string?>("localStorageGetItem", key);
    }

    public async ValueTask LocalStorageSetItemAsync(string key, string value)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("localStorageSetItem", key, value);
    }

    public async ValueTask LocalStorageRemoveItemAsync(string key)
    {
        var module = await TryGetModuleAsync();
        if (module is null)
        {
            return;
        }

        await module.InvokeVoidAsync("localStorageRemoveItem", key);
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is null)
        {
            return;
        }

        try
        {
            await _module.DisposeAsync();
            _module = null;
        }
        catch (JSDisconnectedException)
        {
        }
        catch (InvalidOperationException)
        {
        }
    }
}
