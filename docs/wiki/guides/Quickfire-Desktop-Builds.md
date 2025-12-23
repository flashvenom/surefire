# Quickfire Desktop Build Guide

Quickfire ships with a .NET MAUI WebView shell (`src/Quickfire.Desktop`) that bootstraps a published copy of the Blazor host. The MAUI app extracts the publish output, provisions a local data directory, launches the host inside `%LOCALAPPDATA%\flashvenom\QuickfireDesktop`, and navigates its WebView once the site reports as ready.

## Prerequisites
- .NET SDK 10.0 with the MAUI workload installed
- Windows 10 build 19041 or later (targets `net10.0-windows10.0.19041.0`)
- WebView2 runtime
- A populated `build/desktop` folder (publish step below creates this)

## Step 1 - Publish the Blazor host
```bash
dotnet publish src/Quickfire.Blazor/Quickfire.Blazor.csproj -c Release -p:DesktopWebPublishDir="$PWD/build/desktop/"
```

- Output lands in `build/desktop/`.
- Uses SQLite connection defaults unless you override configuration/env vars.

## Step 2 - Stage the host assets for MAUI
`Quickfire.Desktop.csproj` includes the `PrepareQuickfireHostPackage` target which copies the publish output and writes a manifest.

```bash
dotnet msbuild src/Quickfire.Desktop/Quickfire.Desktop.csproj /t:PrepareQuickfireHostPackage /p:Configuration=Release /p:TargetFramework=net10.0-windows10.0.19041.0
```

The target:
- Reads `QuickfirePublishDir` (defaults to `build/desktop`)
- Copies every asset into `obj/<tfm>/<config>/QuickfireHost/`
- Emits a `manifest.txt` alongside the files

## Step 3 - Run the MAUI shell
```bash
dotnet run --project src/Quickfire.Desktop/Quickfire.Desktop.csproj `
    -f net10.0-windows10.0.19041.0 `
    -c Release
```

At runtime `QuickfireHostService`:
1. Extracts packaged assets into `%LOCALAPPDATA%\flashvenom\QuickfireDesktop\openfire-host\site\`
2. Seeds the SQLite database if the wizard selects local storage and no DB exists
3. Starts `Quickfire.Blazor.exe` (or `dotnet Quickfire.Blazor.dll`) on the configured port (default 5350)
4. Streams host stdout/stderr into `%LOCALAPPDATA%\flashvenom\QuickfireDesktop\logs\quickfire.log`
5. Points the WebView at `http://127.0.0.1:<port>/` after the ready check succeeds

## Optional - Tray helper
- `dotnet run --project src/Quickfire.Tray/Quickfire.Tray.csproj` enables Outlook/Word automations and notification hooks.

## Troubleshooting
| Symptom | How to fix |
| --- | --- |
| Blank WebView | Check `%LOCALAPPDATA%\flashvenom\QuickfireDesktop\logs\quickfire.log`. Re-run Step 1 if the publish folder is empty. |
| "Quickfire executable could not be located" | Ensure the publish output contains `Quickfire.Blazor.exe` or `Quickfire.Blazor.dll`. |
| Wizard cannot find the seed database | Confirm `Resources/Raw/SeedData/local.db` exists and rerun Step 2. |
| Port conflicts | Update `src/Quickfire.Desktop/appsettings.maui.json` (`QuickfireHost:Port`) before rebuilding. |

## Customization tips
- Override data/log folders by editing `QuickfireHostOptions` defaults or the corresponding JSON section.
- Add extra files to the desktop payload by extending `PrepareQuickfireHostPackage` or adding `MauiAsset` entries.
- Tweak readiness polling (`ReadyPath`, `StartupTimeoutSeconds`) via `appsettings.maui.json` if your publish has a different startup signature.
- Delete `desktop-setup.json` under `%LOCALAPPDATA%\flashvenom\QuickfireDesktop` to replay first-run experiences.

Keep this guide updated anytime the desktop bootstrapper flow changes so contributors and installer builds stay in sync.
