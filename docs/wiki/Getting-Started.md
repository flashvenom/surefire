Use this guide when cloning the wiki into the repo or onboarding a new machine. It mirrors the root `README.md` but is scoped to the open-source build.

## Prerequisites
- .NET SDK 10.0.x with ASP.NET and EF Core workloads
- MAUI workload only if you build the desktop shell (`dotnet workload install maui`)
- WebView2 runtime for the MAUI desktop shell (optional)
- SQL Server (optional) or SQLite (default)
- Syncfusion license if you plan to redistribute builds

## Environment Variables / .env
Create an `.env` next to `Quickfire.sln` (optional):

```
DEFAULTCONNECTION=Server=.;Database=Quickfire;Trusted_Connection=True;TrustServerCertificate=true;
OPENFIRE_DB=Sqlite
OPENFIRE_DIR=C:\SurefireData
OPENFIRE_SEED=true
ADMIN_EMAIL=admin@quickfire.local
ADMIN_USERNAME=admin@quickfire.local
ADMIN_FIRSTNAME=Quickfire
ADMIN_LASTNAME=Admin
ADMIN_PASSWORD=Admin123!
```

Notes:
- If `DEFAULTCONNECTION` is empty, the app falls back to `ConnectionStrings:DefaultSqlite`.
- `OPENFIRE_DB` supports `Sqlite` or `SqlServer`.
- Desktop mode can auto-bootstrap an admin using the `OPENFIRE_ADMIN_*` values.

## Clone + Restore
```bash
git clone <repo-url>
cd Openfire
dotnet restore Quickfire.sln
```

## Database
```bash
dotnet ef database update --project src/Quickfire.Blazor/Quickfire.Blazor.csproj --context ApplicationDbContext
```

## Run the Blazor Server host
```bash
cd src/Quickfire.Blazor
dotnet watch run
```
- `OPENFIRE_SEED=true` seeds baseline data on startup.
- The open-source build does not include integrations or AI services.

## Desktop Publish and MAUI shell (optional)
```bash
dotnet publish src/Quickfire.Blazor/Quickfire.Blazor.csproj -c Debug -p:DesktopWebPublishDir="$PWD/build/desktop/"
dotnet msbuild src/Quickfire.Desktop/Quickfire.Desktop.csproj /t:PrepareQuickfireHostPackage /p:TargetFramework=net10.0-windows10.0.19041.0 /p:Configuration=Debug
dotnet run --project src/Quickfire.Desktop/Quickfire.Desktop.csproj -f net10.0-windows10.0.19041.0 -c Debug
```
More detail: [[guides/Quickfire-Desktop-Builds]].

## System Settings
The open-source build removes integration key storage. The System Settings page (`/System`) is informational only. If you add new integrations, update code/config directly.

## Troubleshooting Checklist
- Syncfusion components fail to render -> confirm `SyncfusionLicenseProvider.RegisterLicense` in `src/Quickfire.Blazor/Program.cs`
- Desktop publish missing -> Quickfire.Desktop warns if `build/desktop` is empty
- FireSearch results missing -> ensure migrations applied and seeded data exists

With the basics in place, continue through the feature pages or jump into [[System-Architecture]] to understand how services wire together.
