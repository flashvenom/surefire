using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quickfire.Desktop.Interop;
using Quickfire.Tray;

namespace Quickfire.Desktop.Services;

public interface IDesktopEmberBridge : IAsyncDisposable
{
    Task EnsureConnectedAsync(Uri hostBaseUri, CancellationToken cancellationToken = default);
}

public sealed class DesktopEmberBridge : IDesktopEmberBridge
{
    private static readonly TimeSpan[] ReconnectDelays =
    [
        TimeSpan.Zero,
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(30)
    ];

    private readonly ILogger<DesktopEmberBridge> _logger;
    private readonly SemaphoreSlim _connectionGate = new(1, 1);

    private HubConnection? _connection;
    private Uri? _hubUri;
    private string? _userId;

    public DesktopEmberBridge(ILogger<DesktopEmberBridge> logger)
    {
        _logger = logger;
    }

    public async Task EnsureConnectedAsync(Uri hostBaseUri, CancellationToken cancellationToken = default)
    {
        if (hostBaseUri is null)
        {
            throw new ArgumentNullException(nameof(hostBaseUri));
        }

        var hubUri = BuildHubUri(hostBaseUri);
        await _connectionGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_connection is not null && _hubUri == hubUri)
            {
                if (_connection.State == HubConnectionState.Disconnected)
                {
                    _logger.LogInformation("Restarting Ember bridge connection.");
                    await _connection.StartAsync(cancellationToken).ConfigureAwait(false);
                    await JoinGroupAsync(cancellationToken).ConfigureAwait(false);
                }
                return;
            }

            await DisposeConnectionInternalAsync().ConfigureAwait(false);

            _hubUri = hubUri;
            _userId = ResolveUserId();
            if (string.IsNullOrWhiteSpace(_userId))
            {
                _logger.LogWarning("Unable to resolve a Windows user name for Ember bridge registration.");
                return;
            }

            _connection = new HubConnectionBuilder()
                .WithUrl(hubUri)
                .WithAutomaticReconnect(ReconnectDelays)
                .AddJsonProtocol()
                .Build();

            RegisterHandlers(_connection);

            _logger.LogInformation("Connecting to Ember hub at {HubUri}", hubUri);
            await _connection.StartAsync(cancellationToken).ConfigureAwait(false);
            await JoinGroupAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _connectionGate.Release();
        }
    }

    private static Uri BuildHubUri(Uri baseUri)
    {
        if (!baseUri.ToString().EndsWith("/", StringComparison.Ordinal))
        {
            baseUri = new Uri(baseUri.ToString() + "/");
        }

        return new Uri(baseUri, "emberHub");
    }

    private void RegisterHandlers(HubConnection connection)
    {
        connection.On<string, List<string>>("ReceiveEmberCommand", async (command, parameters) =>
        {
            await HandleEmberCommandAsync(command, parameters ?? new List<string>()).ConfigureAwait(false);
        });

        connection.Reconnecting += ex =>
        {
            _logger.LogWarning(ex, "Ember bridge connection lost, attempting to reconnect.");
            return Task.CompletedTask;
        };

        connection.Reconnected += async _ =>
        {
            _logger.LogInformation("Ember bridge connection re-established.");
            await JoinGroupAsync(CancellationToken.None).ConfigureAwait(false);
        };

        connection.Closed += ex =>
        {
            _logger.LogWarning(ex, "Ember bridge connection closed.");
            return Task.CompletedTask;
        };
    }

    private async Task JoinGroupAsync(CancellationToken cancellationToken)
    {
        if (_connection is null || _connection.State != HubConnectionState.Connected)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_userId))
        {
            _logger.LogWarning("Skipping Ember group join because user ID is missing.");
            return;
        }

        try
        {
            await _connection.InvokeAsync("JoinGroup", _userId, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Registered Ember bridge for Windows user {User}", _userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to join Ember group for user {User}", _userId);
        }
    }

    private async Task HandleEmberCommandAsync(string? emberFunction, List<string> parameters)
    {
        if (string.IsNullOrWhiteSpace(emberFunction))
        {
            _logger.LogWarning("Received an empty Ember command.");
            return;
        }

        var normalized = emberFunction.ToLowerInvariant();
        _logger.LogInformation("Processing Ember command {Command} (params: {Count})", emberFunction, parameters.Count);

            try
            {
                if (normalized.StartsWith("windows_", StringComparison.Ordinal))
                {
                    await Task.Run(() => WindowsControl.PerformWindowsFunction(emberFunction, parameters)).ConfigureAwait(false);
                }
                else if (normalized.StartsWith("outlook", StringComparison.Ordinal))
                {
                    await RunOnStaThreadAsync(() => OutlookControl.PerformOutlookFunction(emberFunction, parameters)).ConfigureAwait(false);
                }
                else if (string.Equals(normalized, "showtraynotification", StringComparison.Ordinal))
                {
                    await NotificationInterop.ShowTrayNotificationAsync(parameters, _logger).ConfigureAwait(false);
                }
                else if (string.Equals(normalized, "showstaffchat", StringComparison.Ordinal))
                {
                    await MessagingInterop.ShowStaffChatNotificationAsync(parameters, _logger).ConfigureAwait(false);
                }
                else if (normalized.StartsWith("update", StringComparison.Ordinal))
                {
                    await UpdateInterop.HandleUpdateCommandAsync(normalized, parameters, _logger).ConfigureAwait(false);
                }
                else
                {
                    _logger.LogWarning("No handler registered for Ember command {Command}", emberFunction);
                }
            }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing Ember command {Command}", emberFunction);
        }
    }

    private static Task RunOnStaThreadAsync(Action action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var tcs = new TaskCompletionSource<object?>();
        var thread = new Thread(() =>
        {
            try
            {
                action();
                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        })
        {
            IsBackground = true
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return tcs.Task;
    }

    private static string ResolveUserId()
    {
        try
        {
            return Environment.UserName;
        }
        catch
        {
            return string.Empty;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeConnectionInternalAsync().ConfigureAwait(false);
    }

    private async Task DisposeConnectionInternalAsync()
    {
        if (_connection is null)
        {
            return;
        }

        try
        {
            await _connection.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error while disposing Ember bridge connection.");
        }
        finally
        {
            _connection = null;
            _hubUri = null;
        }
    }
}
