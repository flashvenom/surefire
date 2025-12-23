using System;
using System.Text.Json.Serialization;

namespace Quickfire.Desktop.Services;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DesktopDatabaseMode
{
    Local,
    Remote
}

public sealed class DesktopDatabaseSettings
{
    public DesktopDatabaseMode Mode { get; set; } = DesktopDatabaseMode.Local;
    public string LocalDatabasePath { get; set; } = string.Empty;
    public string RemoteConnectionString { get; set; } = string.Empty;
}

public sealed class DesktopAdminSettings
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PictureUrl { get; set; } = "default.jpg";
}

public sealed class DesktopSetupState
{
    public bool IsConfigured { get; set; }
    public bool TermsAccepted { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    public DesktopAdminSettings Admin { get; set; } = new();
    public DesktopDatabaseSettings Database { get; set; } = new();

    [JsonIgnore]
    public string? AdminPassword { get; set; }

    public DesktopSetupState Clone()
    {
        return new DesktopSetupState
        {
            IsConfigured = IsConfigured,
            TermsAccepted = TermsAccepted,
            CompletedAt = CompletedAt,
            Admin = new DesktopAdminSettings
            {
                FirstName = Admin.FirstName,
                LastName = Admin.LastName,
                Email = Admin.Email,
                PictureUrl = string.IsNullOrWhiteSpace(Admin.PictureUrl) ? "default.jpg" : Admin.PictureUrl
            },
            Database = new DesktopDatabaseSettings
            {
                Mode = Database.Mode,
                LocalDatabasePath = Database.LocalDatabasePath,
                RemoteConnectionString = Database.RemoteConnectionString
            },
            AdminPassword = AdminPassword
        };
    }
}
