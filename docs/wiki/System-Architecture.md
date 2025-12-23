This overview covers the projects that ship in the open-source solution and highlights the services/components referenced throughout the wiki.

## Solution Layout
| Project | Purpose | Key folders |
| --- | --- | --- |
| `src/Quickfire.Blazor` (`Quickfire.Blazor.csproj`) | Blazor Server host, REST endpoints, SignalR hubs | `Domain/*` feature areas, `Data` EF Core context/migrations, `App/Layout`, `wwwroot` assets |
| `src/Quickfire.Desktop` (`Quickfire.Desktop.csproj`) | .NET MAUI shell that boots the published Blazor host locally | `Services/QuickfireHostService.cs`, `Resources`, `appsettings.maui.json` |
| `src/Quickfire.Tray` (`Quickfire.Tray.csproj`) | Windows tray helper for Outlook, Word, and OS commands | `System`, `Methods`, `Resources` |

## Runtime Modes
- **Server/Web** - Standard ASP.NET Core host. SQLite by default, SQL Server when `DEFAULTCONNECTION` is provided.
- **Desktop** - MAUI shell runs the host locally. Runtime detection uses `OPENFIRE_DESKTOP` or `OPENFIRE_DIR`.
- **Tray (optional)** - Separate process that connects to `/emberHub` for Outlook/Word and desktop commands.

## Core Services
- **StateService** (`src/Quickfire.Blazor/Domain/Shared/Services/StateService.cs`) caches lookups and user preferences and drives status bar updates.
- **SearchService** (`src/Quickfire.Blazor/Domain/Shared/Services/SearchService.cs`) powers FireSearch across clients, carriers, contacts, policies, renewals, and leads.
- **AttachmentService** (`src/Quickfire.Blazor/Domain/Attachments/Services/AttachmentService.cs`) handles uploads, hashing, thumbnails, and local storage under `wwwroot/uploads`.
- **EmberService + EmberHub** (`src/Quickfire.Blazor/Domain/Ember/*`) forward tray commands and responses.

## Data and Storage
- EF Core context lives in `src/Quickfire.Blazor/Data/ApplicationDbContext*.cs`.
- Migrations live in `src/Quickfire.Blazor/Data/Migrations`.
- Attachments store metadata in the database and files on disk (default `wwwroot/uploads`).

## Request Lifecycles
1. **UI** - Razor components (e.g., `Domain/Clients/Pages/Clients.razor`) use Fluent UI + Syncfusion controls wired with conventions in [[reference/Binding-Events]].
2. **Services** - Domain services live under `Domain/*/Services` and depend on `ApplicationDbContext`, `StateService`, and helpers.
3. **Desktop Loop** - MAUI host unpacks `build/desktop`, boots `Quickfire.Blazor.exe`, and points the WebView at the local port.

## Observability
- `StateService.UpdateStatus("...", isBusy)` feeds the status bar (`src/Quickfire.Blazor/App/Layout/_statusbar.razor`).
- `ILoggingService` and `ILogger` provide structured logs in services.
- `Quickfire.Tray/System/SystemTray.cs` writes to `%LOCALAPPDATA%\Surefire\TrayLog.txt` for tray diagnostics.

Keep this page handy when updating docs: cite the relevant file paths from this map so engineers can jump from wiki to code immediately.
