using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;

namespace Quickfire.Desktop.Interop;

public static class NotificationInterop
{
    public static async Task ShowTrayNotificationAsync(IReadOnlyList<string> parameters, ILogger logger, CancellationToken cancellationToken = default)
    {
        if (parameters is null || parameters.Count < 2)
        {
            logger.LogWarning("ShowTrayNotification skipped because not enough parameters were provided.");
            return;
        }

        var title = parameters[0];
        var message = parameters[1];
        var text = string.IsNullOrWhiteSpace(title) ? message : $"{title}: {message}";

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                await Toast.Make(text, ToastDuration.Long).Show(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to display desktop toast notification.");
            }
        });
    }
}
