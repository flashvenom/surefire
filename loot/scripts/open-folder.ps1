param(
    [Parameter(Mandatory=$true, ValueFromRemainingArguments=$true)]
    [string[]]$Path
)

# Join args (handles paths with spaces) and normalize
$target = ($Path -join ' ').Trim('"')
try {
    $resolved = (Resolve-Path -LiteralPath $target -ErrorAction Stop).ProviderPath
} catch {
    $resolved = $target
}
# Normalize trailing slashes (Explorer reports X:\, not X:)
$resolved = $resolved.TrimEnd('\')
if ($resolved -match '^[A-Za-z]:$') { $resolved += '\' }

# Find existing Explorer window at that path
$shell   = New-Object -ComObject Shell.Application
$windows = $shell.Windows()
$match   = $null

foreach ($w in $windows) {
    try {
        $wPath = $w.Document.Folder.Self.Path
        if (-not $wPath) { continue }
        $wPath = $wPath.TrimEnd('\')
        if ($wPath -match '^[A-Za-z]:$') { $wPath += '\' }

        if ([StringComparer]::InvariantCultureIgnoreCase.Equals($wPath, $resolved)) {
            $match = $w
            break
        }
    } catch { continue }
}

if ($match) {
    # Bring that Explorer window to front
    $hwnd = $match.HWND
    Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
}
"@ -ErrorAction SilentlyContinue
    [Win]::ShowWindowAsync([IntPtr]$hwnd, 9) | Out-Null  # SW_RESTORE
    [Win]::SetForegroundWindow([IntPtr]$hwnd)  | Out-Null
} else {
    Start-Process explorer.exe -ArgumentList "`"$resolved`""
}
