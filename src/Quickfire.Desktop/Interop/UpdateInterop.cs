using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Quickfire.Desktop.Interop;

public static class UpdateInterop
{
    public static async Task HandleUpdateCommandAsync(string command, IReadOnlyList<string> parameters, ILogger logger, CancellationToken cancellationToken = default)
    {
        var normalized = (command ?? string.Empty).ToLowerInvariant();
        if (normalized == "update_available" || normalized == "update_prompt")
        {
            var details = parameters.Count > 0
                ? parameters[0]
                : "A new Quickfire Desktop update is available.";

            await NotificationInterop.ShowTrayNotificationAsync(new[] { "Quickfire Desktop", details }, logger, cancellationToken);
            return;
        }

        logger.LogInformation("Received update command '{Command}' with {Count} parameters (no action taken).", command, parameters?.Count ?? 0);
    }
}
