using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
#if WINDOWS_TRAY
using System.Windows.Forms;
using System.Drawing;
#endif
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Quickfire.Tray
{
    public static class WindowsControl
    {
        private static readonly string ExplorerFallbackScriptPath = Path.Combine(AppContext.BaseDirectory, "open-folder.ps1");

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool OpenIcon(IntPtr hWnd);

        public static void BringWindowToFront(IntPtr handle)
        {
            try
            {
                if (handle != IntPtr.Zero)
                {
                    if (IsIconic(handle))
                    {
                        OpenIcon(handle);
                    }
                    ShowWindow(handle, 9); // SW_RESTORE = 9
                    SetForegroundWindow(handle);
                }
            }
            catch (System.Exception ex)
            {
                SystemControl.Log($"Error bringing window to front: {ex.Message}");
            }
        }

        public static void PerformWindowsFunction(string emberFunction, List<string> parameters)
        {
            switch (emberFunction)
            {
                case "Windows_OpenFolder":
                    OpenFolder(parameters.FirstOrDefault());
                    break;
                case "Windows_OpenFile":
                    OpenFile(parameters.FirstOrDefault());
                    break;
                case "Windows_ShowCallNotification":
                    ShowCallNotification(parameters);
                    break;
                default:
                    SystemControl.Log($"Unknown ember windowscontrol function: {emberFunction}");
                    break;
            }
        }

        public static void OpenFolder(string folderPath)
        {
            SystemControl.Log($"Finding to Open: {folderPath}");
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                SystemControl.Log("[Explorer] Skipping open because folderPath was empty.");
                return;
            }

            var trimmedPath = folderPath.Trim();
            SystemControl.Log($"[Explorer] Trimmed path: {trimmedPath}");

            try
            {
                if (File.Exists(trimmedPath))
                {
                    var fullPath = Path.GetFullPath(trimmedPath);
                    var directoryPath = Path.GetDirectoryName(fullPath);
                    var normalizedDirectoryPath = NormalizeExplorerPath(directoryPath);
                    var fileName = Path.GetFileName(fullPath);

                    SystemControl.Log($"[Explorer] Path resolved to a file. Directory: {directoryPath}, File: {fileName}");

                    var interopUnavailable = false;
                    var matchedExistingWindow = !string.IsNullOrEmpty(normalizedDirectoryPath) &&
                                                TryFocusExistingExplorerWindow(normalizedDirectoryPath, out interopUnavailable, fileName);

                    SystemControl.Log($"[Explorer] TryFocusExistingExplorerWindow result => matched: {matchedExistingWindow}, interopUnavailable: {interopUnavailable}");

                    if (matchedExistingWindow)
                    {
                        SystemControl.Log($"[Explorer] Selected file '{fileName}' in existing Explorer window: {normalizedDirectoryPath}");
                        return;
                    }

                    SystemControl.Log($"[Explorer] No matching Explorer window found. Opening new window for: {fullPath}");
                    var explorerStarted = TryLaunchExplorer($"/select,\"{fullPath}\"", $"file selection '{fullPath}'");

                    if (explorerStarted)
                    {
                        if (!string.IsNullOrEmpty(normalizedDirectoryPath) && !TryFocusExplorerWindowWithRetries(normalizedDirectoryPath, fileName))
                        {
                            SystemControl.Log($"[Explorer] Explorer window launched but focus could not be confirmed: {normalizedDirectoryPath}");
                        }

                        SystemControl.Log($"Opened folder and selected file: {fullPath}");
                        return;
                    }

                    var fallbackReason = interopUnavailable ? "interop unavailable" : "explorer launch failed";
                    var fallbackPath = normalizedDirectoryPath ?? directoryPath ?? fullPath;

                    SystemControl.Log($"[Explorer] Explorer launch failed ({fallbackReason}). Attempting fallback for folder: {fallbackPath}");
                    if (TryInvokeExplorerFallback(fallbackPath, interopUnavailable, fallbackReason))
                    {
                        SystemControl.Log($"Delegated folder activation to fallback script ({fallbackReason}): {fallbackPath}");
                        return;
                    }

                    SystemControl.Log($"[Explorer] Failed to activate Explorer for file: {fullPath}");
                    return;
                }

                if (Directory.Exists(trimmedPath))
                {
                    var fullPath = Path.GetFullPath(trimmedPath);
                    var normalizedPath = NormalizeExplorerPath(fullPath);
                    SystemControl.Log($"[Explorer] Path resolved to directory. FullPath: {fullPath}");
                    SystemControl.Log($"[Explorer] Normalized path: {normalizedPath}");

                    var interopUnavailable = false;
                    var matchedExistingWindow = !string.IsNullOrEmpty(normalizedPath) &&
                                                TryFocusExistingExplorerWindow(normalizedPath, out interopUnavailable);

                    SystemControl.Log($"[Explorer] TryFocusExistingExplorerWindow result => matched: {matchedExistingWindow}, interopUnavailable: {interopUnavailable}");

                    if (matchedExistingWindow)
                    {
                        SystemControl.Log($"[Explorer] Brought existing Explorer window to front: {normalizedPath}");
                        return;
                    }

                    SystemControl.Log($"[Explorer] No matching Explorer window found. Opening new window for directory: {fullPath}");
                    var explorerStarted = TryLaunchExplorer($"\"{fullPath}\"", $"directory '{fullPath}'");

                    if (explorerStarted)
                    {
                        if (!string.IsNullOrEmpty(normalizedPath) && !TryFocusExplorerWindowWithRetries(normalizedPath, null))
                        {
                            SystemControl.Log($"[Explorer] Explorer window launched but focus could not be confirmed: {normalizedPath}");
                        }

                        SystemControl.Log($"Opened folder in Explorer: {fullPath}");
                        return;
                    }

                    var fallbackReason = interopUnavailable ? "interop unavailable" : "explorer launch failed";
                    SystemControl.Log($"[Explorer] Explorer launch failed ({fallbackReason}). Attempting fallback for directory: {fullPath}");
                    if (TryInvokeExplorerFallback(fullPath, interopUnavailable, fallbackReason))
                    {
                        SystemControl.Log($"Delegated folder activation to fallback script ({fallbackReason}): {fullPath}");
                        return;
                    }

                    SystemControl.Log($"[Explorer] Failed to activate Explorer for directory: {fullPath}");
                    return;
                }
                else
                {
                    SystemControl.Log($"Path does not exist: {trimmedPath}");
                }
            }
            catch (System.Exception ex)
            {
                SystemControl.Log($"Error opening folder in Explorer: {ex.Message}");
            }
        }

        private static bool TryFocusExistingExplorerWindow(string targetPath, out bool interopUnavailable, string itemToSelect = null)
        {
            interopUnavailable = false;

            if (string.IsNullOrEmpty(targetPath))
            {
                SystemControl.Log("[Explorer] TryFocus aborted because targetPath was empty.");
                return false;
            }

            SystemControl.Log($"[Explorer] Searching for existing Explorer window for: {targetPath}");
            if (!string.IsNullOrEmpty(itemToSelect))
            {
                SystemControl.Log($"[Explorer] Item requested for selection: {itemToSelect}");
            }

            object shell = null;
            object windows = null;

            try
            {
                var shellType = Type.GetTypeFromProgID("Shell.Application");
                if (shellType == null)
                {
                    SystemControl.Log("[Explorer] Shell.Application ProgID not available.");
                    interopUnavailable = true;
                    return false;
                }

                shell = Activator.CreateInstance(shellType);
                windows = shellType.InvokeMember("Windows", BindingFlags.InvokeMethod, null, shell, null);

                if (windows == null)
                {
                    SystemControl.Log("[Explorer] Shell windows enumeration returned null.");
                    return false;
                }

                var windowsType = windows.GetType();
                var countObj = windowsType.InvokeMember("Count", BindingFlags.GetProperty, null, windows, null);
                var count = countObj is int directCount ? directCount : Convert.ToInt32(countObj);
                SystemControl.Log($"[Explorer] Found {count} shell windows to inspect.");

                for (var i = 0; i < count; i++)
                {
                    var window = windowsType.InvokeMember("Item", BindingFlags.InvokeMethod, null, windows, new object[] { i });

                    if (window == null)
                    {
                        SystemControl.Log($"[Explorer] Window index {i} was null.");
                        continue;
                    }

                    try
                    {
                        var windowPath = GetWindowPath(window);

                        if (string.IsNullOrEmpty(windowPath))
                        {
                            SystemControl.Log($"[Explorer] Window index {i} has no document path.");
                            continue;
                        }

                        windowPath = NormalizeExplorerPath(windowPath);
                        SystemControl.Log($"[Explorer] Window index {i} normalized path: {windowPath}");

                        if (!StringComparer.InvariantCultureIgnoreCase.Equals(windowPath, targetPath))
                        {
                            continue;
                        }

                        var hwndObj = window.GetType().InvokeMember("HWND", BindingFlags.GetProperty, null, window, null);

                        if (hwndObj == null)
                        {
                            SystemControl.Log($"[Explorer] Window index {i} did not expose HWND.");
                            continue;
                        }

                        var hwnd = new IntPtr(Convert.ToInt64(hwndObj));
                        SystemControl.Log($"[Explorer] Match found at index {i}. Bringing HWND {hwnd} to front.");
                        BringWindowToFront(hwnd);

                        if (!string.IsNullOrEmpty(itemToSelect))
                        {
                            SystemControl.Log($"[Explorer] Attempting to select '{itemToSelect}' in matched window.");
                            if (TrySelectItemInWindow(window, itemToSelect))
                            {
                                SystemControl.Log($"[Explorer] Successfully selected '{itemToSelect}'.");
                            }
                            else
                            {
                                SystemControl.Log($"[Explorer] Failed to select '{itemToSelect}' in matched window.");
                            }
                        }

                        return true;
                    }
                    finally
                    {
                        ReleaseComObject(window);
                    }
                }

                SystemControl.Log("[Explorer] No matching Explorer window found.");
            }
            catch (System.Exception ex)
            {
                interopUnavailable = true;
                SystemControl.Log($"[Explorer] Error while searching Explorer windows: {ex.Message}");
            }
            finally
            {
                ReleaseComObject(windows);
                ReleaseComObject(shell);
            }

            return false;
        }

        private static string GetWindowPath(object window)
        {
            object document = null;
            object folder = null;
            object self = null;

            try
            {
                document = window.GetType().InvokeMember("Document", BindingFlags.GetProperty, null, window, null);
                if (document == null)
                {
                    return null;
                }

                folder = document.GetType().InvokeMember("Folder", BindingFlags.GetProperty, null, document, null);
                if (folder == null)
                {
                    return null;
                }

                self = folder.GetType().InvokeMember("Self", BindingFlags.GetProperty, null, folder, null);
                if (self == null)
                {
                    return null;
                }

                return self.GetType().InvokeMember("Path", BindingFlags.GetProperty, null, self, null) as string;
            }
            catch
            {
                return null;
            }
            finally
            {
                ReleaseComObject(self);
                ReleaseComObject(folder);
                ReleaseComObject(document);
            }
        }

        private static bool TrySelectItemInWindow(object window, string itemName)
        {
            if (window == null || string.IsNullOrEmpty(itemName))
            {
                return false;
            }

            object document = null;
            object folder = null;
            object item = null;

            try
            {
                document = window.GetType().InvokeMember("Document", BindingFlags.GetProperty, null, window, null);
                if (document == null)
                {
                    SystemControl.Log("[Explorer] Unable to access window.Document for item selection.");
                    return false;
                }

                folder = document.GetType().InvokeMember("Folder", BindingFlags.GetProperty, null, document, null);
                if (folder == null)
                {
                    SystemControl.Log("[Explorer] Unable to access folder from window.Document for item selection.");
                    return false;
                }

                item = folder.GetType().InvokeMember("ParseName", BindingFlags.InvokeMethod, null, folder, new object[] { itemName });
                if (item == null)
                {
                    SystemControl.Log($"[Explorer] ParseName returned null for item '{itemName}'.");
                    return false;
                }

                const int SelectItemFlags = 0x1 | 0x8 | 0x10; // SVSI_SELECT | SVSI_ENSUREVISIBLE | SVSI_FOCUSED
                document.GetType().InvokeMember("SelectItem", BindingFlags.InvokeMethod, null, document, new object[] { item, SelectItemFlags });
                return true;
            }
            catch (System.Exception ex)
            {
                SystemControl.Log($"[Explorer] Exception while attempting to select '{itemName}': {ex.Message}");
                return false;
            }
            finally
            {
                ReleaseComObject(item);
                ReleaseComObject(folder);
                ReleaseComObject(document);
            }
        }

        private static bool TryFocusExplorerWindowWithRetries(string targetPath, string itemToSelect, int attemptCount = 5, int delayMilliseconds = 200)
        {
            if (string.IsNullOrEmpty(targetPath))
            {
                return false;
            }

            for (var attempt = 0; attempt < attemptCount; attempt++)
            {
                var interopUnavailable = false;
                if (TryFocusExistingExplorerWindow(targetPath, out interopUnavailable, itemToSelect))
                {
                    return true;
                }

                if (interopUnavailable)
                {
                    break;
                }

                if (attempt < attemptCount - 1)
                {
                    Thread.Sleep(delayMilliseconds);
                }
            }

            return false;
        }
        private static bool TryInvokeExplorerFallback(string targetPath, bool interopUnavailable, string fallbackReasonOverride = null)
        {
            var reason = !string.IsNullOrWhiteSpace(fallbackReasonOverride)
                ? fallbackReasonOverride
                : (interopUnavailable ? "interop unavailable" : "no matching Explorer window");
            SystemControl.Log($"[Explorer] Fallback requested ({reason}) for path: {targetPath}");

            try
            {
                if (string.IsNullOrWhiteSpace(targetPath))
                {
                    SystemControl.Log("[Explorer] Fallback aborted because targetPath was empty.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(ExplorerFallbackScriptPath))
                {
                    SystemControl.Log($"[Explorer] Fallback script path not configured ({reason}).");
                    return false;
                }

                if (!File.Exists(ExplorerFallbackScriptPath))
                {
                    SystemControl.Log($"[Explorer] Fallback script not found at {ExplorerFallbackScriptPath} ({reason}).");
                    return false;
                }

                var arguments = $"-ExecutionPolicy Bypass -NoProfile -File \"{ExplorerFallbackScriptPath}\" \"{targetPath}\"";
                SystemControl.Log($"[Explorer] Invoking fallback script: powershell.exe {arguments}");

                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(psi);
                if (process == null)
                {
                    SystemControl.Log("[Explorer] Fallback script failed to launch (Process.Start returned null).");
                    return false;
                }

                SystemControl.Log("[Explorer] Fallback script launched successfully.");
                return true;
            }
            catch (System.Exception ex)
            {
                SystemControl.Log($"[Explorer] Fallback script execution failed: {ex.Message}");
                return false;
            }
        }

        private static bool TryLaunchExplorer(string arguments, string logContext)
        {
            if (string.IsNullOrWhiteSpace(arguments))
            {
                SystemControl.Log("[Explorer] Explorer launch aborted because arguments were empty.");
                return false;
            }

            try
            {
                var process = Process.Start("explorer.exe", arguments);
                if (process == null)
                {
                    SystemControl.Log($"[Explorer] Process.Start returned null while launching Explorer for {logContext}.");
                    return false;
                }

                return true;
            }
            catch (System.Exception ex)
            {
                SystemControl.Log($"[Explorer] Failed to launch Explorer for {logContext}: {ex.Message}");
                return false;
            }
        }


        private static string NormalizeExplorerPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            var normalized = path.Trim().Trim('"').TrimEnd('\\');

            if (normalized.Length == 2 && normalized[1] == ':' && char.IsLetter(normalized[0]))
            {
                normalized += '\\';
            }

            return normalized;
        }

        private static void ReleaseComObject(object comObject)
        {
            if (comObject == null)
            {
                return;
            }

            try
            {
                if (Marshal.IsComObject(comObject))
                {
                    Marshal.ReleaseComObject(comObject);
                }
            }
            catch
            {
                // Suppress exceptions while cleaning up COM references.
            }
        }
        public static void OpenFile(string filePath)
        {
            SystemControl.Log($"Finding to Open: {filePath}");
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    // Open the file using the default application
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true // Ensures the file opens with the associated application
                    });
                    SystemControl.Log($"Opened file: {filePath}");

                    // Optionally, bring the associated application to the front
                    // Remove this if unnecessary or unclear
                    SystemControl.BringToFront("CabinetWClass"); // Adjust class name if needed for specific apps
                }
                catch (System.Exception ex)
                {
                    SystemControl.Log($"Error opening file: {ex.Message}");
                }
            }
            else
            {
                SystemControl.Log("No file path provided to open.");
            }
        }

        public static async void ShowCallNotification(List<string> parameters)
        {
#if WINDOWS_TRAY
            if (parameters == null || parameters.Count < 3)
            {
                SystemControl.Log("Error: Insufficient parameters for call notification. Required: Title, Message, ClientId");
                return;
            }

            string title = parameters[0];
            string message = parameters[1];
            string clientId = parameters[2];
            string headshotUrl = parameters.Count > 3 ? parameters[3] : null;
            string clientUrl = $"https://surefire.local/Clients/{clientId}";

            try
            {
                // Create a custom notification form
                using (var notificationForm = new Form())
                {
                    notificationForm.FormBorderStyle = FormBorderStyle.None;
                    notificationForm.StartPosition = FormStartPosition.Manual;
                    notificationForm.Size = new Size(400, 150);
                    notificationForm.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - notificationForm.Width - 10,
                                                       Screen.PrimaryScreen.WorkingArea.Height - notificationForm.Height - 10);
                    notificationForm.BackColor = Color.White;

                    // Create title label
                    var titleLabel = new Label
                    {
                        Text = title,
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        Location = new Point(10, 10),
                        AutoSize = true
                    };

                    // Create message label
                    var messageLabel = new Label
                    {
                        Text = message,
                        Font = new Font("Segoe UI", 10),
                        Location = new Point(10, 40),
                        AutoSize = true
                    };

                    // Create open client button
                    var openButton = new Button
                    {
                        Text = "Open Client",
                        Location = new Point(10, 100),
                        Size = new Size(100, 30),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.FromArgb(0, 120, 215),
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold)
                    };
                    openButton.Click += (s, e) =>
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = clientUrl,
                            UseShellExecute = true
                        });
                        notificationForm.Close();
                    };

                    // Add controls to form
                    notificationForm.Controls.Add(titleLabel);
                    notificationForm.Controls.Add(messageLabel);
                    notificationForm.Controls.Add(openButton);

                    // Add headshot if provided
                    if (!string.IsNullOrEmpty(headshotUrl))
                    {
                        try
                        {
                            using (var httpClient = new HttpClient())
                            {
                                var response = await httpClient.GetAsync(headshotUrl);
                                if (response.IsSuccessStatusCode)
                                {
                                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                                    using (var ms = new System.IO.MemoryStream(imageBytes))
                                    {
                                        var headshot = Image.FromStream(ms);
                                        var headshotPicture = new PictureBox
                                        {
                                            Image = headshot,
                                            SizeMode = PictureBoxSizeMode.Zoom,
                                            Size = new Size(80, 80),
                                            Location = new Point(notificationForm.Width - 90, 10)
                                        };
                                        notificationForm.Controls.Add(headshotPicture);
                                    }
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            SystemControl.Log($"Error loading headshot: {ex.Message}");
                        }
                    }

                    // Show the notification
                    notificationForm.Show();
                    
                    // Auto-close after 10 seconds
                    var timer = new System.Windows.Forms.Timer { Interval = 10000 };
                    timer.Tick += (s, e) =>
                    {
                        timer.Stop();
                        notificationForm.Close();
                    };
                    timer.Start();
                }
            }
            catch (System.Exception ex)
            {
                SystemControl.Log($"Error showing call notification: {ex.Message}");
            }
#else
            SystemControl.Log("Call notifications are only supported inside the dedicated tray host.");
#endif
        }
    }
}














