using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Quickfire.Desktop.Services;

public interface IDesktopSetupService
{
    Task<DesktopSetupState> GetStateAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(DesktopSetupState state, CancellationToken cancellationToken = default);
    Task<bool> IsConfiguredAsync(CancellationToken cancellationToken = default);
    string GetDefaultDatabasePath();
}

public sealed class DesktopSetupService : IDesktopSetupService, IDisposable
{
    private const string FileName = "desktop-setup.json";
    private static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();

    private readonly string _configPath;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly QuickfireHostOptions _hostOptions;
    private DesktopSetupState? _state;
    private bool _disposed;

    public DesktopSetupService(IOptions<QuickfireHostOptions> hostOptions)
    {
        _hostOptions = hostOptions.Value ?? new QuickfireHostOptions();
        var appRoot = DesktopStorage.GetAppDataRoot();
        Directory.CreateDirectory(appRoot);
        _configPath = Path.Combine(appRoot, FileName);
    }

    public async Task<DesktopSetupState> GetStateAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_state is null)
        {
            await LoadAsync(cancellationToken).ConfigureAwait(false);
        }

        return _state!.Clone();
    }

    public async Task<bool> IsConfiguredAsync(CancellationToken cancellationToken = default)
    {
        var snapshot = await GetStateAsync(cancellationToken).ConfigureAwait(false);
        return snapshot.IsConfigured;
    }

    public async Task SaveAsync(DesktopSetupState state, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(state);

        var clone = state.Clone();
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            _state = clone;
            Directory.CreateDirectory(Path.GetDirectoryName(_configPath)!);
            var json = JsonSerializer.Serialize(clone, SerializerOptions);
            await File.WriteAllTextAsync(_configPath, json, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    public string GetDefaultDatabasePath()
    {
        var directory = GetDefaultDataDirectory();
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, "local.db");
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_state is not null)
            {
                return;
            }

            if (!File.Exists(_configPath))
            {
                _state = CreateDefaultState();
                return;
            }

            await using var stream = File.OpenRead(_configPath);
            var loaded = await JsonSerializer.DeserializeAsync<DesktopSetupState>(stream, SerializerOptions, cancellationToken).ConfigureAwait(false);
            _state = loaded ?? CreateDefaultState();
            EnsureDefaults(_state);
        }
        finally
        {
            _gate.Release();
        }
    }

    private DesktopSetupState CreateDefaultState()
    {
        return new DesktopSetupState
        {
            Admin = new DesktopAdminSettings
            {
                FirstName = "",
                LastName = "",
                Email = "",
                PictureUrl = "default.jpg"
            },
            Database = new DesktopDatabaseSettings
            {
                Mode = DesktopDatabaseMode.Local,
                LocalDatabasePath = GetDefaultDatabasePath(),
                RemoteConnectionString = ""
            },
            TermsAccepted = false,
            IsConfigured = false
        };
    }

    private void EnsureDefaults(DesktopSetupState state)
    {
        state.Admin ??= new DesktopAdminSettings();
        state.Database ??= new DesktopDatabaseSettings();
        if (string.IsNullOrWhiteSpace(state.Admin.PictureUrl))
        {
            state.Admin.PictureUrl = "default.jpg";
        }
        if (string.IsNullOrWhiteSpace(state.Database.LocalDatabasePath))
        {
            state.Database.LocalDatabasePath = GetDefaultDatabasePath();
        }
    }

    private string GetDefaultDataDirectory()
    {
        return DesktopStorage.GetDataRoot(_hostOptions.DeploymentFolderName, _hostOptions.DataDirectoryName);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _gate.Dispose();
        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private static JsonSerializerOptions CreateSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
            //IgnoreUnknownProperties = true
        };
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
