Use the Windows tray app and Ember commands to pull the active Word document into Quickfire. The tray command exists in the open-source build but is not wired to any specific UI by default.

## Architecture
| Component | Location | Notes |
| --- | --- | --- |
| Tray command router | `src/Quickfire.Tray/Methods/WordControl.cs` | Handles `GetWordDocContents`, logs to `%LOCALAPPDATA%\\Surefire\\TrayLog.txt` |
| System tray | `src/Quickfire.Tray/System/SystemTray.cs` | Registers commands and forwards responses via SignalR |
| Ember hub | `src/Quickfire.Blazor/Domain/Ember/EmberHub.cs` | Hosts SignalR endpoints for tray <-> web app communication |
| Ember client | `src/Quickfire.Blazor/Domain/Ember/EmberService.cs` | Sends commands and receives responses |

## Request flow
1. Web client calls `EmberService.RunEmberFunction("GetWordDocContents", new List<string>())`.
2. EmberHub forwards the command to the connected tray app for that user.
3. Tray executes `WordControl.GetWordDocContents`:
   - Verifies Word is running and a document is active
   - Reads the entire document body as plain text
   - Returns `ERROR: ...` strings for failure cases
4. Tray sends results back via `SystemTray.SendEmberResponse`.
5. Web client handler receives the text and processes it.

## Client-side snippet
```csharp
protected override async Task OnInitializedAsync()
{
    EmberService.RegisterResponseHandler("GetWordDocContents", HandleWordResponse);
}

private async Task HandleWordResponse(List<string> response)
{
    var text = response.FirstOrDefault();
    if (text?.StartsWith("ERROR:") == true)
    {
        // show toast, log, etc.
        return;
    }

    await ProcessWordTextAsync(text);
}

private Task RequestWordAsync()
    => EmberService.RunEmberFunction("GetWordDocContents", new List<string>());
```

## Troubleshooting
- Check `%LOCALAPPDATA%\\Surefire\\TrayLog.txt` for `[WordControl]` entries
- Ensure the tray app is connected (status icon lit) and EmberHub shows the user connected
- Word must be the active application with a document open
- SignalR connectivity issues show errors in browser console and tray logs

## Extending commands
1. Add a method in `WordControl.cs`.
2. Register it in `SystemTray.cs`.
3. Document expected parameters and responses here.
4. Add client handlers in the relevant component.

This integration keeps Word -> Quickfire flows local and fast without writing files to disk.
