using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Quickfire.Tray
{
    public static class ConsoleWindow
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        // Win32 constants
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        // Handler for console close event
        private static ConsoleEventDelegate _handler;
        private delegate bool ConsoleEventDelegate(int eventType);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        // Initialize to false - the console isn't visible by default
        private static bool _consoleVisible = false;
        private static readonly string LogFilePath = Path.Combine(AppContext.BaseDirectory, "TrayLog.txt");
        private static IntPtr _consoleHandle = IntPtr.Zero;
        private static Thread _keyListenerThread;

        public static void ShowConsoleWindow()
        {
            if (!_consoleVisible)
            {
                if (_consoleHandle == IntPtr.Zero)
                {
                    AllocConsole();
                    _consoleHandle = GetConsoleWindow();
                }
                else
                {
                    ShowWindow(_consoleHandle, SW_SHOW);
                }
                
                _consoleVisible = true;
                
                // Set up close handler
                _handler = new ConsoleEventDelegate(ConsoleEventCallback);
                SetConsoleCtrlHandler(_handler, true);
                
                // Remove close button or disable it
                DisableCloseButton();

                // Redirect the console output to ensure a valid handle
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
                Console.Title = "Quickfire Debug Console - Close with ESC key";
                
                // Setup key handling for ESC key to close console
                if (_keyListenerThread == null || !_keyListenerThread.IsAlive)
                {
                    _keyListenerThread = new Thread(() => 
                    {
                        while (_consoleVisible)
                        {
                            if (Console.KeyAvailable)
                            {
                                var key = Console.ReadKey(true);
                                if (key.Key == ConsoleKey.Escape)
                                {
                                    HideConsoleWindow();
                                    break;
                                }
                            }
                            Thread.Sleep(100);
                        }
                    })
                    { IsBackground = true };
                    _keyListenerThread.Start();
                }

                // Replay log file contents
                ReplayLog();
                
                // Add instructions for user
                Console.WriteLine();
                Console.WriteLine("==================================================");
                Console.WriteLine("Press ESC key to close this debug console window");
                Console.WriteLine("==================================================");
            }
        }

        private static void DisableCloseButton()
        {
            if (_consoleHandle != IntPtr.Zero)
            {
                // Disable the X button by removing the system menu
                int style = GetWindowLong(_consoleHandle, GWL_STYLE);
                SetWindowLong(_consoleHandle, GWL_STYLE, style & ~WS_SYSMENU);
            }
        }

        private static bool ConsoleEventCallback(int eventType)
        {
            // Intercept close event (eventType 2)
            if (eventType == 2)
            {
                HideConsoleWindow();
                return true; // Prevent default close behavior
            }
            return false;
        }

        public static void HideConsoleWindow()
        {
            if (_consoleVisible && _consoleHandle != IntPtr.Zero)
            {
                Console.WriteLine("Closing Debug Console...");
                Console.Out.Flush(); // Ensure all output is written before closing
                
                // Instead of freeing the console, just hide it
                ShowWindow(_consoleHandle, SW_HIDE);
                _consoleVisible = false;
            }
        }

        public static void ToggleConsoleWindow()
        {
            if (_consoleVisible)
            {
                HideConsoleWindow();
            }
            else
            {
                ShowConsoleWindow();
            }
        }

        private static void ReplayLog()
        {
            try
            {
                if (File.Exists(LogFilePath))
                {
                    var logContent = File.ReadAllText(LogFilePath);
                    Console.WriteLine(logContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error replaying log: {ex.Message}");
            }
        }
    }
}
