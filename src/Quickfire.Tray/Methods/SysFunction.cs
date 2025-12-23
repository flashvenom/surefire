using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Quickfire.Tray
{
    public static class SystemControl
    {
        private static readonly string LogFilePath = Path.Combine(AppContext.BaseDirectory, "TrayLog.txt");

        public static void BringToFront(string className)
        {
            IntPtr hwnd = FindWindow(className, null);
            if (hwnd != IntPtr.Zero)
            {
                SetForegroundWindow(hwnd);
            }
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public static void Log(string message)
        {
            try
            {
                var directory = Path.GetDirectoryName(LogFilePath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.AppendAllText(LogFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Failed to log message: {ex.Message}");
            }
        }

    }
}
